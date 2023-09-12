using Cipa.Application.Interfaces;
using Cipa.Domain.Entities;
using Cipa.Domain.Enums;
using Cipa.Domain.Exceptions;
using Cipa.Domain.Factories.Interfaces;
using Cipa.Domain.Helpers;
using Cipa.Application.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cipa.Application.Implementation
{
    public class UsuarioAppService : AppServiceBase<Usuario>, IUsuarioAppService
    {
        private readonly IFormatadorEmailServiceFactory _formatadorFactory;
        private readonly EmailConfiguration _emailConfiguration;
        public UsuarioAppService(
            IUnitOfWork unitOfWork,
            IFormatadorEmailServiceFactory formatadorFactory,
            EmailConfiguration emailConfiguration) : base(unitOfWork, unitOfWork.UsuarioRepository)
        {
            _formatadorFactory = formatadorFactory;
            _emailConfiguration = emailConfiguration;
        }

        public Usuario BuscarUsuario(string email, string senha) =>
            (_repositoryBase as IUsuarioRepository).BuscarUsuario(email, senha);

        public IEnumerable<Usuario> BuscarUsuariosPelaConta(int contaId) =>
            (_repositoryBase as IUsuarioRepository).BuscarUsuariosPelaConta(contaId);

        public IEnumerable<Usuario> BuscarUsuariosAdministradores() =>
            (_repositoryBase as IUsuarioRepository).BuscarUsuariosAdministradores();

        private void EnviarEmail(Usuario usuario, ETipoTemplateEmail tipoTemplate)
        {
            var formatador = _formatadorFactory.ObterFormatadorEmailParaAcesso(tipoTemplate, usuario);
            foreach (var email in formatador.FormatarEmails())
                _unitOfWork.EmailRepository.Adicionar(email);
        }


        public override Usuario Adicionar(Usuario usuario)
        {
            usuario.Email = usuario.Email.Trim().ToLower();
            var conta = _unitOfWork.ContaRepository.BuscarPeloId(usuario.ContaId.Value);
            if (conta == null) throw new NotFoundException("Conta não encontrada.");
            var usuarioExistente = (_repositoryBase as IUsuarioRepository).BuscarUsuario(usuario.Email);
            if (usuarioExistente != null)
            {
                if (usuarioExistente.Perfil == PerfilUsuario.SESMT || usuarioExistente.Perfil == PerfilUsuario.Administrador)
                    throw new DuplicatedException($"Já há um usuário cadastrado com o e-mail '{usuario.Email}'.");
                else
                {
                    usuarioExistente.AlterarParaPerfilSESMT(conta);
                    base.Atualizar(usuarioExistente);
                    return usuarioExistente;
                }
            }
            usuario.Conta = conta;
            usuario.ContaId = conta.Id;
            EnviarEmail(usuario, ETipoTemplateEmail.CadastroSESMT);
            return base.Adicionar(usuario);
        }

        public override void Atualizar(Usuario usuario)
        {
            var usuarioExistente = (_repositoryBase as IUsuarioRepository).BuscarPeloId(usuario.Id);
            if (usuarioExistente == null) throw new NotFoundException("Usuário não encontrado.");
            usuario.Email = usuario.Email.Trim().ToLower();
            if (usuario.Email != usuarioExistente.Email && (_repositoryBase as IUsuarioRepository).BuscarUsuario(usuario.Email) != null)
                throw new DuplicatedException($"Já há um usuário cadastrado com o e-mail '{usuario.Email}'.");
            usuarioExistente.Cargo = usuario.Cargo;
            usuarioExistente.Email = usuario.Email.Trim().ToLower();
            usuarioExistente.Nome = usuario.Nome;
            usuarioExistente.Perfil = usuario.Perfil;
            base.Atualizar(usuarioExistente);
        }


        public Usuario AdicionarAdministrador(Usuario usuario)
        {
            usuario.Email = usuario.Email.Trim().ToLower();
            Conta conta = null;
            if (usuario.ContaId.HasValue)
            {
                conta = _unitOfWork.ContaRepository.BuscarPeloId(usuario.ContaId.Value);
                if (conta == null)
                    usuario.ContaId = null;
            }

            var usuarioExistente = (_repositoryBase as IUsuarioRepository).BuscarUsuario(usuario.Email);
            if (usuarioExistente != null)
            {
                if (usuarioExistente.Perfil == PerfilUsuario.Administrador)
                    throw new DuplicatedException($"Já há um usuário cadastrado com o e-mail '{usuario.Email}'.");
                else
                {
                    usuarioExistente.AlterarParaPerfilAdministrador();
                    base.Atualizar(usuarioExistente);
                    return usuarioExistente;
                }
            }

            if (conta != null)
            {
                usuario.Conta = conta;
                usuario.ContaId = conta.Id;
            }

            EnviarEmail(usuario, ETipoTemplateEmail.CadastroSESMT);
            return base.Adicionar(usuario);
        }

        public override Usuario Excluir(int id)
        {
            var usuario = (_repositoryBase as IUsuarioRepository).BuscarPeloId(id);
            if (usuario == null) throw new NotFoundException("Usuário não encontrado.");
            if (!usuario.Eleitores.Any() && !usuario.JaCadastrouSenha)
                return base.Excluir(usuario);
            usuario.AlterarParaPerfilEleitor();
            base.Atualizar(usuario);
            return usuario;
        }

        public Usuario CadastrarNovaSenha(Guid codigoRecuperacao, string senha)
        {
            var usuario = (_repositoryBase as IUsuarioRepository).BuscarUsuarioPeloCodigoRecuperacao(codigoRecuperacao);
            if (usuario == null) throw new NotFoundException("Código de recuperação inválido.");

            usuario.CadastrarSenha(codigoRecuperacao, senha);
            base.Atualizar(usuario);
            return usuario;
        }

        public void ResetarSenha(string emailUsuario)
        {
            var usuario = (_repositoryBase as IUsuarioRepository).BuscarUsuario(emailUsuario);
            if (usuario == null) throw new NotFoundException("Usuário não encontrado.");

            usuario.ResetarSenha();

            var formatador = _formatadorFactory.ObterFormatadorEmailParaAcesso(ETipoTemplateEmail.ResetSenha, usuario);
            var email = formatador.FormatarEmails().Single();
            _unitOfWork.EmailRepository.Adicionar(email);
            base.Atualizar(usuario);
        }

        public Usuario BuscarUsuarioPeloCodigoRecuperacao(Guid codigoRecuperacao)
        {
            var usuario = (_repositoryBase as IUsuarioRepository).BuscarUsuarioPeloCodigoRecuperacao(codigoRecuperacao);
            if (usuario == null) throw new NotFoundException("Usuário não encontrado.");

            return usuario;
        }

        public async Task RedefinirSenha(string email, string novaSenha)
        {
            await (_repositoryBase as IUsuarioRepository).ResetarSenha(email, novaSenha);
        }
    }
}
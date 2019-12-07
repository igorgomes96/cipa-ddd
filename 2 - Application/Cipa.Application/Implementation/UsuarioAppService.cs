using System.Collections.Generic;
using System.Linq;
using Cipa.Application.Interfaces;
using Cipa.Domain.Entities;
using Cipa.Domain.Exceptions;
using Cipa.Domain.Helpers;
using Cipa.Domain.Interfaces.Repositories;

namespace Cipa.Application
{
    public class UsuarioAppService : AppServiceBase<Usuario>, IUsuarioAppService
    {
        public UsuarioAppService(IUnitOfWork unitOfWork) : base(unitOfWork, unitOfWork.UsuarioRepository) { }

        public Usuario BuscarUsuario(string email, string senha) =>
            (_repositoryBase as IUsuarioRepository).BuscarUsuario(email, senha);

        public IEnumerable<Usuario> BuscarUsuariosPelaConta(int contaId) =>
            (_repositoryBase as IUsuarioRepository).BuscarUsuariosPelaConta(contaId);

        public override Usuario Adicionar(Usuario usuario)
        {
            usuario.Email = usuario.Email.Trim().ToLower();
            var conta = _unitOfWork.ContaRepository.BuscarPeloId(usuario.ContaId.Value);
            if (conta == null) throw new NotFoundException("Conta não encontrada.");
            var usuarioExistente = (_repositoryBase as IUsuarioRepository).BuscarUsuario(usuario.Email);
            if (usuarioExistente != null)
            {
                if (usuarioExistente.Perfil == PerfilUsuario.SESMT)
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

        public override Usuario Excluir(int id)
        {
            var usuario = (_repositoryBase as IUsuarioRepository).BuscarPeloId(id);
            if (usuario == null) throw new NotFoundException("Usuário não encontrado.");
            if (!usuario.Eleitores.Any() && string.IsNullOrWhiteSpace(usuario.Senha))
                return base.Excluir(usuario);
            usuario.AlterarParaPerfilEleitor();
            base.Atualizar(usuario);
            return usuario;
        }
    }
}
using Cipa.Application.Interfaces;
using Cipa.Domain.Entities;
using Cipa.Domain.Exceptions;
using Cipa.Domain.Helpers;
using Cipa.WebApi.ViewModels;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;

namespace Cipa.WebApi.Authentication
{
    public class LoginService : ILoginService
    {
        private readonly IUsuarioAppService _usuarioAppService;
        private readonly SigningConfigurations _signingConfigurations;
        private readonly TokenConfigurations _tokenConfigurations;
        private readonly IContaAppService _contaAppService;

        public LoginService(
            IUsuarioAppService usuarioAppService,
            SigningConfigurations signingConfigurations,
            TokenConfigurations tokenConfigurations,
            IContaAppService contaAppService)
        {
            _usuarioAppService = usuarioAppService;
            _signingConfigurations = signingConfigurations;
            _tokenConfigurations = tokenConfigurations;
            _contaAppService = contaAppService;
        }

        private Usuario ValidaUsuario(string email, string senha)
        {
            senha = CryptoService.ComputeSha256Hash(senha);
            var usuario = _usuarioAppService.BuscarUsuario(email, senha);
            if (usuario == null) throw new CustomException("Credenciais inválidas!");
            return usuario;
        }

        public ClaimsIdentity GeraIdentity(Usuario usuario, Conta conta)
        {
            ClaimsIdentity identity = new ClaimsIdentity(
                    new GenericIdentity(usuario.Email, "Login"),
                    new[] {
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
                        new Claim(CustomClaimTypes.NomeUsuario, usuario.Nome),
                        new Claim(ClaimTypes.Role, usuario.Perfil),
                        new Claim(CustomClaimTypes.UsuarioId, usuario.Id.ToString())
                    }
                );
            if (conta != null)
            {
                identity.AddClaims(
                    new[] {
                        new Claim(CustomClaimTypes.DataExpiracaoConta, conta.DataFim.ToString()),
                        new Claim(CustomClaimTypes.QtdaEstabelecimentos, conta.QtdaEstabelecimentos.ToString()),
                        new Claim(CustomClaimTypes.CodigoConta, conta.Id.ToString()),
                        new Claim(CustomClaimTypes.ContaValida, conta.Ativa.ToString().ToLower())
                    }
                );
            }
            return identity;
        }

        public AuthInfoViewModel AlterarContaTokenAdministrador(int usuarioId, int contaId)
        {
            var usuario = _usuarioAppService.BuscarPeloId(usuarioId);
            var conta = _contaAppService.BuscarPeloId(contaId);
            if (usuario == null) throw new NotFoundException("Usuário não encontrado.");
            if (conta == null) throw new NotFoundException("Conta não encontrada.");

            var identity = GeraIdentity(usuario, conta);
            return GerarToken(usuario, identity);
        }

        public AuthInfoViewModel Login(string email, string senha)
        {
            var usuarioBanco = ValidaUsuario(email, senha);
            var identity = GeraIdentity(usuarioBanco, usuarioBanco.Conta);
            return GerarToken(usuarioBanco, identity);
        }

        private AuthInfoViewModel GerarToken(Usuario usuario, ClaimsIdentity identity)
        {
            DateTime dataCriacao = DateTime.Now.HorarioBrasilia();
            DateTime dataExpiracao = dataCriacao + TimeSpan.FromSeconds(_tokenConfigurations.Seconds);

            var handler = new JwtSecurityTokenHandler();
            var securityToken = handler.CreateToken(new SecurityTokenDescriptor
            {
                Issuer = _tokenConfigurations.Issuer,
                Audience = _tokenConfigurations.Audience,
                SigningCredentials = _signingConfigurations.SigningCredentials,
                Subject = identity,
                NotBefore = dataCriacao,
                Expires = dataExpiracao
            });
            var token = handler.WriteToken(securityToken);

            return new AuthInfoViewModel
            {
                AccessToken = token,
                Criacao = dataCriacao,
                Expiracao = dataExpiracao,
                Roles = identity.Claims.Where(x => x.Type == ClaimTypes.Role).Select(x => x.Value).ToArray(),
                UsuarioEmail = usuario.Email
            };
        }

        public AuthInfoViewModel CadastrarNovaSenha(Guid codigoRecuperacao, string senha)
        {
            var usuario = _usuarioAppService.CadastrarNovaSenha(codigoRecuperacao, CryptoService.ComputeSha256Hash(senha));
            return Login(usuario.Email, senha);
        }

        public void ResetarSenha(string email)
            => _usuarioAppService.ResetarSenha(email);

        public Usuario BuscarUsuarioPeloCodigoRecuperacao(Guid codigoRecuperacao)
            => _usuarioAppService.BuscarUsuarioPeloCodigoRecuperacao(codigoRecuperacao);
    }
}
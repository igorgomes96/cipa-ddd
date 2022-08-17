using System;
using System.Linq;
using System.Security.Claims;
using Cipa.Domain.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace Cipa.WebApi.Authentication
{
    public static class PoliticasAutorizacao
    {
        public const string UsuarioSESMT = "UsuarioSESMT";
        public const string UsuarioSESMTContaValida = "UsuarioSESMTContaValida";

    }

    public static class AuthorizationPolicies
    {

        private static Func<ClaimsPrincipal, string, bool> hasIntClaim = (ClaimsPrincipal user, string claimType) =>
            {
                var claim = user.Claims.FirstOrDefault(c => c.Type == claimType);
                if (claim == null) return false;
                return int.TryParse(claim.Value, out int claimValue);
            };

        private static Func<ClaimsPrincipal, string, bool> hasClaim = (ClaimsPrincipal user, string claimType) =>
            user.Claims.Any(c => c.Type == claimType);

        private static Func<ClaimsPrincipal, string, bool> hasBooleanClaim = (ClaimsPrincipal user, string claimType) =>
        {
            var claim = user.Claims.FirstOrDefault(c => c.Type == claimType);
            if (claim == null) return false;
            return bool.TryParse(claim.Value, out bool claimValue) && claimValue;
        };

        private static Func<ClaimsPrincipal, string, DateTime> dataExpiracao = (ClaimsPrincipal user, string claimType) =>
        {
            var claim = user.Claims.FirstOrDefault(c => c.Type == claimType);
            if (claim == null) return DateTime.MinValue;
            if (user.IsInRole(PerfilUsuario.Administrador)) return DateTime.UtcNow.AddMonths(1); // Garante que o adm sempre terá acesso
            bool dataValida = DateTime.TryParse(claim.Value, out DateTime data);
            return dataValida ? data : DateTime.MinValue;
        };

        private static Func<AuthorizationHandlerContext, bool> usuarioSESMTPossuiContaExpression = (context) =>
            (context.User.IsInRole(PerfilUsuario.SESMT) || context.User.IsInRole(PerfilUsuario.Administrador))
            && hasIntClaim(context.User, CustomClaimTypes.CodigoConta)
            && hasIntClaim(context.User, CustomClaimTypes.UsuarioId)
            && hasIntClaim(context.User, CustomClaimTypes.QtdaEstabelecimentos)
            && hasClaim(context.User, CustomClaimTypes.DataExpiracaoConta);

        private static Func<AuthorizationHandlerContext, bool> usuarioSESMTPossuiContaValidaExpression = (context) =>
            usuarioSESMTPossuiContaExpression(context)
            && hasBooleanClaim(context.User, CustomClaimTypes.ContaValida)
            && dataExpiracao(context.User, CustomClaimTypes.DataExpiracaoConta) > DateTime.UtcNow;


        public static AuthorizationPolicy UsuarioSESMTAuthorizationPolicy =>
            new AuthorizationPolicyBuilder()
                .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                .RequireAuthenticatedUser()
                .RequireAssertion(usuarioSESMTPossuiContaExpression).Build();

        public static AuthorizationPolicy UsuarioSESMTPossuiContaValidaAuthorizationPolicy =>
            new AuthorizationPolicyBuilder()
                .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                .RequireAuthenticatedUser()
                .RequireAssertion(usuarioSESMTPossuiContaValidaExpression).Build();
    }
}
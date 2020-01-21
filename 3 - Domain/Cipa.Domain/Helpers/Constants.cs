namespace Cipa.Domain.Helpers
{
    public static class PerfilUsuario
    {
        public const string Eleitor = "Eleitor";
        public const string SESMT = "SESMT";
        public const string Administrador = "Administrador";

    }

    public static class CustomClaimTypes
    {
        public const string CodigoConta = "accid";
        public const string UsuarioId = "userid";
        public const string DataExpiracaoConta = "accexp";
        public const string QtdaEstabelecimentos = "estqty";
        public const string ContaValida = "accvalid";
        public const string NomeUsuario = "username";
    }

    public static class ColunasArquivo
    {
        public const string Matricula = "Matrícula";
        public const string Email = "Email";
        public const string Nome = "Nome";
        public const string Cargo = "Cargo";
        public const string Area = "Área";
        public const string DataAdmissao = "Data de Admissão";
        public const string DataNascimento = "Data de Nascimento";
        public const string NomeGestor = "Nome do Gestor";
        public const string EmailGestor = "Email do Gestor";
    }


    public static class Links
    {
        public const string URL = "https://cipabeta.solucoesti.online";
        public const string Login = "/autenticacao/login";
        public const string Cadastro = "/autenticacao/cadastro";
        public const string Reset = "/autenticacao/reset";
        public const string Votacao = "/eleicoes/{id}/votacao";
        public const string Inscricoes = "/eleicoes/{id}/candidaturas/nova";
    }


    public static class FusosHorarios
    {
        public const string Brasilia = "E. South America Standard Time";
    }

}
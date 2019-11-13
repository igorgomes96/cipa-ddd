namespace Cipa.Domain.Helpers
{
    public static class Perfil
    {
        public const string Eleitor = "Eleitor";
        public const string SESMT = "SESMT";

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

    public enum PosicaoEtapa
    {
        Passada,
        Atual,
        Futura
    }

    public enum CodigoEtapaObrigatoria
    {
        Convocacao = 1,
        FormacaoComissao = 2,
        EditalInscricao = 3,
        Inscricao = 4,
        Votacao = 5,
        Ata = 6
    }

    public static class EtapaProcessamentoArquivo
    {
        public const string ValidacaoDados = "Validação lógica dos dados";
        public const string Insercao = "Inserção dos dados na base";
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

    public static class DataTypeImportacao
    {
        public const string String = "String";
        public const string Email = "Email";
        public const string Date = "Date";
        public const string Int = "Int";
        public const string Decimal = "Decimal";
    }


    public static class ArquivosEstaticos
    {
        public const string PATH = "StaticFiles";
    }

    public static class ArquivosEmails
    {
        public const string EditalConvocacao = @"AppData/Emails/EditalConvocacao.html";
        public const string ConviteInscricao = @"AppData/Emails/ConviteInscricao.html";
        public const string ConviteVotacao = @"AppData/Emails/ConviteVotacao.html";
        public const string InscricaoRealizada = @"AppData/Emails/InscricaoRealizada.html";
        public const string InscricaoAprovada = @"AppData/Emails/InscricaoAprovada.html";
        public const string InscricaoReprovada = @"AppData/Emails/InscricaoReprovada.html";
        public const string InscricaoReanalise = @"AppData/Emails/InscricaoReanalise.html";
        public const string CadastroSESMT = @"AppData/Emails/CadastroSESMT.html";
        public const string MudancaEtapaSucesso = @"AppData/Emails/MudancaEtapaSucesso.html";
        public const string MudancaEtapaErro = @"AppData/Emails/MudancaEtapaErro.html";
        public const string ResetSenha = @"AppData/Emails/ResetSenha.html";

    }

    public static class AssuntosEmails
    {
        public const string EditalConvocacao = "[CIPA] Edital de Convocação";
        public const string ConviteInscricao = "[CIPA] Inscrições Abertas";
        public const string ConviteVotacao = "[CIPA] Início da Votação";
        public const string InscricaoRealizada = @"[CIPA] Inscrição Realizada";
        public const string InscricaoAprovada = @"[CIPA] Inscrição Aprovada";
        public const string InscricaoReprovada = @"[CIPA] Inscrição Reprovada";
        public const string InscricaoReanalise = @"[CIPA] Inscrição - Solicitação de Reanálise";
        public const string CadastroSESMT = @"[CIPA] Cadastro no Sistema";
        public const string MudancaEtapaSucesso = @"[CIPA] Mudança de Etapa com Sucesso";
        public const string MudancaEtapaErro = @"[CIPA] Erro ao Realizar Mudança de Etapa";
        public const string ResetSenha = @"[CIPA] Reset de Senha";
    }

    public static class Links
    {
        public const string URL = "http://localhost:4200";
        public const string Login = "/autenticacao/login";
        public const string Cadastro = "/autenticacao/cadastro";
        public const string Reset = "/autenticacao/reset";
        public const string Votacao = "/eleicoes/{id}/votacao";
        public const string Inscricoes = "/eleicoes/{id}/candidaturas/nova";
    }

    public static class PoliticasAutorizacao
    {
        public const string Bearer = "Bearer";
        public const string PossuiConta = "PossuiConta";
        public const string PossuiContaValida = "PossuiContaValida";
    }

    public static class FusosHorarios
    {
        public const string Brasilia = "E. South America Standard Time";
    }

}
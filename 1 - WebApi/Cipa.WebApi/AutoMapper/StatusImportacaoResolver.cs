
using Cipa.Domain.Entities;

namespace Cipa.WebApi.AutoMapper {
    public static class StatusImportacaoResolver {
        public static string MapStatus(StatusImportacao status) {
            switch (status) {
                case StatusImportacao.Aguardando:
                    return "Pendente";
                case StatusImportacao.Processando:
                    return "Em processamento";
                case StatusImportacao.FinalizadoComFalha:
                    return "Falha";
                case StatusImportacao.FinalizadoComSucesso:
                    return "Processado";
                default:
                    return "";
            }
        }
    }
}
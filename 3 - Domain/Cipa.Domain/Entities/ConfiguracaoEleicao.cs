using System.Collections.Generic;

namespace Cipa.Domain.Entities
{
    public class ConfiguracaoEleicao : ValueObject
    {
        public ConfiguracaoEleicao(bool envioEditalConvocao, bool envioConviteInscricao, bool envioConviteVotacao)
        {
            EnvioEditalConvocao = envioEditalConvocao;
            EnvioConviteInscricao = envioConviteInscricao;
            EnvioConviteVotacao = envioConviteVotacao;
        }

        public int Id { get; set; }
        public bool EnvioEditalConvocao { get; set; }
        public bool EnvioConviteInscricao { get; set; }
        public bool EnvioConviteVotacao { get; set; }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return EnvioEditalConvocao;
            yield return EnvioConviteInscricao;
            yield return EnvioConviteVotacao;
        }
    }
}

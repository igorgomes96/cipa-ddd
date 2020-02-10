using Cipa.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cipa.Infra.Data.EntityConfig
{
    public class ConfiguracaoEleicaoConfiguration : IEntityTypeConfiguration<ConfiguracaoEleicao>
    {
        public void Configure(EntityTypeBuilder<ConfiguracaoEleicao> builder)
        {
            builder.ToTable("Eleicoes");

            builder.Property(c => c.EnvioEditalConvocao).IsRequired();
            builder.Property(c => c.EnvioConviteInscricao).IsRequired();
            builder.Property(c => c.EnvioConviteVotacao).IsRequired();

        }
    }
}
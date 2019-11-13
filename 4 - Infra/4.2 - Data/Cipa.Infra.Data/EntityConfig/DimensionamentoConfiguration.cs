using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Cipa.Domain.Entities;

namespace Cipa.Infra.Data.EntityConfig {
    public class DimensionamentoConfiguration : DimensionamentoBaseConfiguration<Dimensionamento>
    {
        public override void Configure(EntityTypeBuilder<Dimensionamento> builder)
        {
            base.Configure(builder);
            
            builder.ToTable("Eleicoes");

            builder.Ignore(e => e.QtdaEleitores);
            builder.Ignore(e => e.QtdaVotos);
            builder.Ignore(e => e.QtdaInscricoesAprovadas);
            builder.Ignore(e => e.QtdaInscricoesPendentes);
            builder.Ignore(e => e.QtdaInscricoesReprovadas);

            builder.Property(e => e.Minimo).HasColumnName("DimensionamentoMinEleitores");
            builder.Property(e => e.Maximo).HasColumnName("DimensionamentoMaxEleitores");
            builder.Property(e => e.QtdaEfetivos).HasColumnName("DimensionamentoQtdaEfetivos");
            builder.Property(e => e.QtdaSuplentes).HasColumnName("DimensionamentoQtdaSuplentes");

        }
    }
}
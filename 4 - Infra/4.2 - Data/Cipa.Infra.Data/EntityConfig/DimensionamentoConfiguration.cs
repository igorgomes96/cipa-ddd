using Cipa.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cipa.Infra.Data.EntityConfig
{
    public class DimensionamentoConfiguration : DimensionamentoBaseConfiguration<Dimensionamento>
    {
        public override void Configure(EntityTypeBuilder<Dimensionamento> builder)
        {
            base.Configure(builder);

            builder.ToTable("Eleicoes");

            builder.Property(e => e.Minimo).HasColumnName("DimensionamentoMinEleitores");
            builder.Property(e => e.Maximo).HasColumnName("DimensionamentoMaxEleitores");
            builder.Property(e => e.QtdaEfetivos).HasColumnName("DimensionamentoQtdaEfetivos");
            builder.Property(e => e.QtdaSuplentes).HasColumnName("DimensionamentoQtdaSuplentes");

        }
    }
}
using Cipa.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cipa.Infra.Data.EntityConfig {
    public class DimensionamentoBaseConfiguration<TDimensionamento> : IEntityTypeConfiguration<TDimensionamento> where TDimensionamento: DimensionamentoBase
    {
        public virtual void Configure(EntityTypeBuilder<TDimensionamento> builder)
        {

            builder.Property(d => d.Minimo)
                .IsRequired();

            builder.Property(d => d.Maximo)
                .IsRequired();

            builder.Property(d => d.QtdaEfetivos)
                .IsRequired();

            builder.Property(d => d.QtdaSuplentes)
                .IsRequired();
        }
    }
}
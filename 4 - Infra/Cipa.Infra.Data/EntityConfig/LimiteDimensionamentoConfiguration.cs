using Cipa.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cipa.Infra.Data.EntityConfig
{
    public class LimiteDimensionamentoConfiguration : IEntityTypeConfiguration<LimiteDimensionamento>
    {
        public void Configure(EntityTypeBuilder<LimiteDimensionamento> builder)
        {
            builder.ToTable("Grupos");

            builder.Property(g => g.Limite)
                .IsRequired();

            builder.Property(g => g.IntervaloAcrescimo)
                .IsRequired();

            builder.Property(g => g.AcrescimoEfetivos)
                .IsRequired();

            builder.Property(g => g.AcrescimoSuplentes)
                .IsRequired();
        }
    }
}
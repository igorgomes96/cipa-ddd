using Cipa.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cipa.Infra.Data.EntityConfig {
    public class GrupoConfiguration : IEntityTypeConfiguration<Grupo>
    {
        public void Configure(EntityTypeBuilder<Grupo> builder)
        {
            builder.HasKey(g => g.Id);

            builder.Property(g => g.CodigoGrupo)
                .HasMaxLength(10)
                .IsRequired();

            builder.HasOne(g => g.LimiteDimensionamento)
                .WithOne()
                .HasForeignKey<LimiteDimensionamento>(g => g.Id)
                .IsRequired();

            builder.HasIndex(g => g.CodigoGrupo).IsUnique();
        }
    }
}
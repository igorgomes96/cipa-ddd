using Cipa.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cipa.Infra.Data.EntityConfig {
    public class EtapaBaseConfiguration<TEtapa> : IEntityTypeConfiguration<TEtapa> where TEtapa: EtapaBase
    {
        public virtual void Configure(EntityTypeBuilder<TEtapa> builder)
        {
            builder.Property(e => e.Nome)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(e => e.Descricao)
                .HasMaxLength(4000);

            builder.Property(e => e.Ordem)
                .IsRequired();
        }
    }
}
using Cipa.Domain.Entities;
using Cipa.Domain.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cipa.Infra.Data.EntityConfig {
    public class EtapaObrigatoriaConfiguration : IEntityTypeConfiguration<EtapaObrigatoria>
    {
        public void Configure(EntityTypeBuilder<EtapaObrigatoria> builder)
        {  
            builder.HasKey(e => e.Id);            
            builder.Property(e => e.Id).ValueGeneratedNever();

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
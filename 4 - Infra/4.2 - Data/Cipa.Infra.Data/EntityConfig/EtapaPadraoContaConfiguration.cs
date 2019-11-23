using Cipa.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cipa.Infra.Data.EntityConfig {
    public class EtapaPadraoContaConfiguration : IEntityTypeConfiguration<EtapaPadraoConta>
    {
        public void Configure(EntityTypeBuilder<EtapaPadraoConta> builder)
        {
            builder.HasKey(e => e.Id);

             builder.Property(e => e.Nome)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(e => e.Descricao)
                .HasMaxLength(4000);

            builder.Property(e => e.Ordem)
                .IsRequired();

            builder.HasOne(e => e.Conta)
                .WithMany(c => c.EtapasPadroes)
                .HasForeignKey(e => e.ContaId)
                .IsRequired();

            builder.HasOne(e => e.EtapaObrigatoria)
                .WithMany()
                .HasForeignKey(e => e.EtapaObrigatoriaId);

            builder.Property(e => e.DuracaoPadrao)
                .IsRequired();

            builder.HasIndex(e => new { e.ContaId, e.Ordem }).IsUnique();
        }
    }
}
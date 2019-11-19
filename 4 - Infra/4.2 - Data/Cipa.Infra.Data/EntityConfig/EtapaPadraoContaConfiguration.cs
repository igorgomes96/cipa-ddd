using Cipa.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cipa.Infra.Data.EntityConfig {
    public class EtapaPadraoContaConfiguration : EtapaBaseConfiguration<EtapaPadraoConta, int>
    {
        public override void Configure(EntityTypeBuilder<EtapaPadraoConta> builder)
        {
            base.Configure(builder);

            builder.HasKey(e => e.Id);

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
using Cipa.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cipa.Infra.Data.EntityConfig {
    public class EtapaCronogramaConfiguration : IEntityTypeConfiguration<EtapaCronograma>
    {
        public void Configure(EntityTypeBuilder<EtapaCronograma> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Nome)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(e => e.Descricao)
                .HasMaxLength(4000);

            builder.Property(e => e.Ordem)
                .IsRequired();

            builder.HasOne(e => e.Eleicao)
                .WithMany(e => e.Cronograma)
                .HasForeignKey(e => e.EleicaoId)
                .IsRequired();

            builder.Property(e => e.DataPrevista)
                .IsRequired();

            builder.HasOne(e => e.EtapaObrigatoria)
                .WithMany()
                .HasForeignKey(e => e.EtapaObrigatoriaId);

            builder.Property(e => e.PosicaoEtapa)
                .IsRequired();

            builder.Property(e => e.ErroMudancaEtapa)
                .HasMaxLength(10000);

            builder.HasIndex(e => new { e.EleicaoId, e.Ordem }).IsUnique();
            
        }
    }
}
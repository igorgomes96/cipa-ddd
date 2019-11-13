using Cipa.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cipa.Infra.Data.EntityConfig {
    public class EtapaCronogramaConfiguration : EtapaBaseConfiguration<EtapaCronograma>
    {
        public override void Configure(EntityTypeBuilder<EtapaCronograma> builder)
        {
            base.Configure(builder);

            builder.HasKey(e => e.Id);

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
            
        }
    }
}
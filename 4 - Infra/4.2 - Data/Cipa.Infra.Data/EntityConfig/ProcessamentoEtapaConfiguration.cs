using Cipa.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cipa.Infra.Data.EntityConfig
{
    public class ProcessamentoEtapaConfiguration : IEntityTypeConfiguration<ProcessamentoEtapa>
    {
        public void Configure(EntityTypeBuilder<ProcessamentoEtapa> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.HorarioMudancaEtapa)
                .IsRequired();
            
            builder.Property(p => p.StatusProcessamentoEtapa)
                .IsRequired();

            builder.Property(p => p.DataCadastro)
                .IsRequired();

            builder.HasOne(p => p.EtapaCronograma)
                .WithOne()
                .HasForeignKey<ProcessamentoEtapa>(p => p.EtapaCronogramaId)
                .IsRequired();

            builder.HasOne(p => p.EtapaCronogramaAnterior)
                .WithOne()
                .HasForeignKey<ProcessamentoEtapa>(p => p.EtapaCronogramaAnteriorId);

            builder.HasOne(p => p.Eleicao)
                .WithMany(e => e.ProcessamentosEtapas)
                .IsRequired();

            builder.HasIndex("EtapaCronogramaId").IsUnique();
        }
    }
}
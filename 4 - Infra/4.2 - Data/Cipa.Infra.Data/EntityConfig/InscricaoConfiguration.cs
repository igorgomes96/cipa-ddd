using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Cipa.Domain.Entities;

namespace Cipa.Infra.Data.EntityConfig
{
    public class InscricaoConfiguration : IEntityTypeConfiguration<Inscricao>
    {
        public void Configure(EntityTypeBuilder<Inscricao> builder)
        {
            builder.HasKey(i => i.Id);

            builder.Property(i => i.Votos)
                .IsRequired();

            builder.Property(i => i.StatusInscricao)
                .IsRequired();

            builder.HasOne(i => i.Eleitor)
                .WithOne(e => e.Inscricao)
                .HasForeignKey<Inscricao>(i => i.EleitorId)
                .IsRequired();

            builder.HasOne(i => i.Eleicao)
                .WithMany(e => e.Inscricoes)
                .HasForeignKey(i => i.EleicaoId)
                .IsRequired();

            builder.Property(i => i.Foto)
                .HasMaxLength(255);

            builder.Property(i => i.Objetivos)
                .HasMaxLength(255); 

            builder.Property(i => i.EmailAprovador)
                .HasMaxLength(100); 

            builder.Property(i => i.NomeAprovador)
                .HasMaxLength(255); 
        }
    }
}
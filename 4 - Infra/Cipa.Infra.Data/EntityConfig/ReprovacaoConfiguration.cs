using Cipa.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cipa.Infra.Data.EntityConfig
{
    public class ReprovacaoConfiguration : IEntityTypeConfiguration<Reprovacao>
    {
        public void Configure(EntityTypeBuilder<Reprovacao> builder)
        {
            builder.HasKey(r => r.Id);

            builder.HasOne(r => r.Inscricao)
                .WithMany(i => i.Reprovacoes)
                .HasForeignKey(r => r.InscricaoId)
                .IsRequired();

            builder.Property(r => r.MotivoReprovacao)
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(r => r.DataCadastro)
                .IsRequired();

            builder.Property(r => r.EmailAprovador)
                .HasMaxLength(100);

            builder.Property(r => r.EmailAprovador)
                .HasMaxLength(255);
        }
    }
}
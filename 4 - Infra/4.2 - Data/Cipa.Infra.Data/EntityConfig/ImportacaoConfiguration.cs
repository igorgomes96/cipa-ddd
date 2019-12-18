using Cipa.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cipa.Infra.Data.EntityConfig
{
    public class ImportacaoConfiguration : IEntityTypeConfiguration<Importacao>
    {
        public void Configure(EntityTypeBuilder<Importacao> builder)
        {
            builder.HasKey(i => i.Id);

            builder.HasOne(i => i.Arquivo)
                .WithMany()
                // .HasForeignKey(i => i.ArquivoId)
                .IsRequired();

            builder.Property(i => i.Status)
                .IsRequired();

            builder.HasOne(i => i.Eleicao)
                .WithMany(e => e.Importacoes)
                .HasForeignKey(i => i.EleicaoId)
                .IsRequired();

            builder.Property(i => i.DataCadastro)
                .IsRequired();
        }
    }
}
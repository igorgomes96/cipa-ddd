using Cipa.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cipa.Infra.Data.EntityConfig
{
    public class EmailConfiguration : IEntityTypeConfiguration<Email>
    {
        public void Configure(EntityTypeBuilder<Email> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Assunto)
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(e => e.Mensagem)
                .IsRequired();

            builder.Property(e => e.StatusEnvio)
                .IsRequired();

            builder.Property(e => e.DataCadastro)
                .IsRequired();

           builder.Ignore(e => e.DestinatariosLista)
                .Ignore(e => e.CopiasLista);
        }
    }
}
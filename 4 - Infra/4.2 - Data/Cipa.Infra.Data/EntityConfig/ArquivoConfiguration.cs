using Cipa.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cipa.Infra.Data.EntityConfig
{
    public class ArquivoConfiguration : IEntityTypeConfiguration<Arquivo>
    {
        public void Configure(EntityTypeBuilder<Arquivo> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(a => a.Path)
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(a => a.Nome)
                .HasMaxLength(100)
                .IsRequired();
            
            builder.Property(a => a.Tamanho)
                .IsRequired();

            builder.Property(a => a.ContentType)
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(a => a.EmailUsuario)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(a => a.NomeUsuario)
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(a => a.DependencyType)
                .IsRequired();
            
            builder.Property(a => a.DependencyId)
                .IsRequired();

            builder.Property(c => c.DataCadastro)
                .IsRequired();
        }
    }
}
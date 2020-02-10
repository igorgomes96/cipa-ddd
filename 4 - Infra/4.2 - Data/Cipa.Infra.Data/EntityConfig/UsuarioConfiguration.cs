using Cipa.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cipa.Infra.Data.EntityConfig
{
    public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
    {
        public void Configure(EntityTypeBuilder<Usuario> builder)
        {
            builder.HasKey(u => u.Id);

            builder.Property(u => u.Email)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(u => u.Nome)
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(u => u.Senha)
                .HasMaxLength(255);

            builder.HasOne(u => u.Conta)
                .WithMany(c => c.Usuarios)
                .HasForeignKey(u => u.ContaId);

            builder.Property(u => u.Perfil)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(u => u.Cargo)
                .HasMaxLength(255);

            builder.HasIndex(u => u.Email).IsUnique();
        }
    }
}
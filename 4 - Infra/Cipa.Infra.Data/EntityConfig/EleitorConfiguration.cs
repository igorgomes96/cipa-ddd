using Cipa.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cipa.Infra.Data.EntityConfig
{
    public class EleitorConfiguration : IEntityTypeConfiguration<Eleitor>
    {
        public void Configure(EntityTypeBuilder<Eleitor> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Nome)
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(e => e.Email)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(e => e.Matricula)
               .HasMaxLength(50);

            builder.Property(e => e.Area)
               .HasMaxLength(255);

            builder.Property(e => e.Cargo)
                .HasMaxLength(255);

            builder.HasOne(e => e.Eleicao)
                .WithMany(e => e.Eleitores)
                .HasForeignKey(e => e.EleicaoId)
                .IsRequired();

            builder.HasOne(e => e.Usuario)
                .WithMany(u => u.Eleitores)
                .HasForeignKey(e => e.UsuarioId)
                .IsRequired()
                .Metadata.PrincipalToDependent.SetPropertyAccessMode(PropertyAccessMode.Field);

            builder.Property(e => e.NomeGestor)
                .HasMaxLength(255);

            builder.Property(e => e.EmailGestor)
                .HasMaxLength(100);

            builder.HasIndex(e => new { e.UsuarioId, e.EleicaoId }).IsUnique();
        }
    }
}
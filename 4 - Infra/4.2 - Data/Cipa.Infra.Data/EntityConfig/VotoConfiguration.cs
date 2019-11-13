using Cipa.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cipa.Infra.Data.EntityConfig {
    public class VotoConfiguration : IEntityTypeConfiguration<Voto>
    {
        public void Configure(EntityTypeBuilder<Voto> builder)
        {
            builder.HasKey(v => v.Id);

            builder.HasOne(v => v.Eleitor)
                .WithOne()
                .HasForeignKey<Voto>(v => v.EleitorId)
                .IsRequired();

            builder.HasOne(v => v.Eleicao)
                .WithMany(e => e.Votos)
                .HasForeignKey(v => v.EleicaoId)
                .IsRequired();

            builder.Property(v => v.IP)
                .HasMaxLength(15)
                .IsRequired();

            builder.Property(e => e.DataCadastro)
                .IsRequired();
        }
    }
}
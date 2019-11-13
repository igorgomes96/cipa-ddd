using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Cipa.Domain.Entities;

namespace Cipa.Infra.Data.EntityConfig
{
    public class EstabelecimentoConfiguration : IEntityTypeConfiguration<Estabelecimento>
    {
        public void Configure(EntityTypeBuilder<Estabelecimento> builder)
        {
            builder.HasKey(e => e.Id);

            builder.HasOne(e => e.Empresa)
                .WithMany(e => e.Estabelecimentos)
                .HasForeignKey(e => e.EmpresaId)
                .IsRequired();

            builder.Property(e => e.Cidade)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(e => e.Endereco)
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(e => e.Descricao)
                .HasMaxLength(255);

            builder.HasOne(e => e.Grupo)
                .WithMany()
                .HasForeignKey(e => e.GrupoId);

            builder.Property(e => e.Ativo)
                .IsRequired();

            builder.Property(e => e.DataCadastro)
                .IsRequired();
        }
    }
}
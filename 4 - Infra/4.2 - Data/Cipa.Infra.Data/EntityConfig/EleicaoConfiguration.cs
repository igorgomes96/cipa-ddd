using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Cipa.Domain.Entities;

namespace Cipa.Infra.Data.EntityConfig
{
    public class EleicaoConfiguration : IEntityTypeConfiguration<Eleicao>
    {
        public void Configure(EntityTypeBuilder<Eleicao> builder)
        {

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Gestao)
                .IsRequired();

            builder.Property(e => e.DuracaoGestao)
                .IsRequired();
            
            builder.HasOne(e => e.Estabelecimento)
                .WithMany(e => e.Eleicoes)
                .HasForeignKey(e => e.EstabelecimentoId)
                .IsRequired();

            builder.Property(e => e.DataInicio)
                .IsRequired();

            builder.HasOne(e => e.Usuario)
                .WithMany()
                .HasForeignKey(e => e.UsuarioCriacaoId)
                .IsRequired();

            builder.HasOne(e => e.Conta)
                .WithMany()
                .HasForeignKey(e => e.ContaId)
                .IsRequired();
            
            builder.Property(e => e.DataCadastro)
                .IsRequired();

            builder.HasOne(e => e.Grupo)
                .WithMany()
                .HasForeignKey(e => e.GrupoId)
                .IsRequired()
                .Metadata.DependentToPrincipal.SetPropertyAccessMode(PropertyAccessMode.Field);

            builder.HasOne(e => e.Dimensionamento)
                .WithOne()
                .HasForeignKey<Dimensionamento>(d => d.Id);

            builder.HasIndex(e => new { e.EstabelecimentoId, e.Gestao }).IsUnique();
            
        }
    }
}
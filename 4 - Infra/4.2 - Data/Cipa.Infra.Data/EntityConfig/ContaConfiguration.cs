using Cipa.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cipa.Infra.Data.EntityConfig {
    public class ContaConfiguration : IEntityTypeConfiguration<Conta>
    {
        public void Configure(EntityTypeBuilder<Conta> builder)
        {
            builder.HasKey(c => c.Id);

            builder.HasOne(c => c.Plano)
                .WithMany()
                .HasForeignKey(c => c.PlanoId);

            builder.Property(c => c.Ativa)
                .IsRequired();

            builder.Property(c => c.QtdaEstabelecimentos)
                .IsRequired();

            builder.Property(c => c.DataInicio)
                .IsRequired();

            builder.Property(c => c.DataFim)
                .IsRequired();

            builder.Property(c => c.DataCadastro)
                .IsRequired();  
        }
    }
}
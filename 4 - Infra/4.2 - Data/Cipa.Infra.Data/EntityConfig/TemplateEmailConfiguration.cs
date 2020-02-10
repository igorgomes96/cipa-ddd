using Cipa.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cipa.Infra.Data.EntityConfig
{
    public class TemplateEmailConfiguration : IEntityTypeConfiguration<TemplateEmail>
    {
        public void Configure(EntityTypeBuilder<TemplateEmail> builder)
        {
            builder.HasKey(t => t.Id);

            builder.Property(t => t.TipoTemplateEmail)
                .IsRequired();
            
            builder.Property(t => t.Assunto)
                .HasMaxLength(255)
                .IsRequired();

            builder.HasOne(t => t.Conta)
                .WithMany(c => c.TemplatesEmails)
                .IsRequired()
                .Metadata.PrincipalToDependent.SetPropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
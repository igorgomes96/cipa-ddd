using Cipa.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cipa.Infra.Data.EntityConfig {
    public class LinhaDimensionamentoConfiguration : DimensionamentoBaseConfiguration<LinhaDimensionamento>
    {
        public override void Configure(EntityTypeBuilder<LinhaDimensionamento> builder)
        {
            base.Configure(builder);

            builder.HasKey(d => d.Id);

            builder.HasOne(d => d.Grupo)
                .WithMany(g => g.Dimensionamentos)
                .HasForeignKey(d => d.GrupoId)
                .IsRequired();
        }
    }
}
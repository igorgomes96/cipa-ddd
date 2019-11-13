using Cipa.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cipa.Infra.Data.EntityConfig {
    public class EtapaObrigatoriaConfiguration : EtapaBaseConfiguration<EtapaObrigatoria>
    {
        public override void Configure(EntityTypeBuilder<EtapaObrigatoria> builder)
        {            
            base.Configure(builder);
            
            builder.HasKey(e => e.Id);
        }
    }
}
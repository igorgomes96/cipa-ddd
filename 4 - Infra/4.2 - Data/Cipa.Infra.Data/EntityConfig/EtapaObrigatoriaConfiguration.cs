using Cipa.Domain.Entities;
using Cipa.Domain.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cipa.Infra.Data.EntityConfig {
    public class EtapaObrigatoriaConfiguration : EtapaBaseConfiguration<EtapaObrigatoria, CodigoEtapaObrigatoria>
    {
        public override void Configure(EntityTypeBuilder<EtapaObrigatoria> builder)
        {  
            base.Configure(builder);
            builder.HasKey(e => e.Id);            
            builder.Property(e => e.Id).ValueGeneratedNever();
        }
    }
}
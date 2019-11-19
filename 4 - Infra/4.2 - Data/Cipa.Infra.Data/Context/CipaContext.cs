using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Cipa.Domain.Entities;
using Cipa.Infra.Data.EntityConfig;

namespace Cipa.Infra.Data.Context
{
    public class CipaContext : DbContext
    {
        public CipaContext(DbContextOptions<CipaContext> options)
            : base(options)
        { }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new EleicaoConfiguration());
            modelBuilder.ApplyConfiguration(new EstabelecimentoConfiguration());
            modelBuilder.ApplyConfiguration(new EmpresaConfiguration());
            modelBuilder.ApplyConfiguration(new EtapaCronogramaConfiguration());
            modelBuilder.ApplyConfiguration(new EleitorConfiguration());
            modelBuilder.ApplyConfiguration(new InscricaoConfiguration());
            modelBuilder.ApplyConfiguration(new ReprovacaoConfiguration());
            modelBuilder.ApplyConfiguration(new VotoConfiguration());
            modelBuilder.ApplyConfiguration(new GrupoConfiguration());
            modelBuilder.ApplyConfiguration(new LimiteDimensionamentoConfiguration());
            modelBuilder.ApplyConfiguration(new LinhaDimensionamentoConfiguration());
            modelBuilder.ApplyConfiguration(new EtapaObrigatoriaConfiguration());
            modelBuilder.ApplyConfiguration(new EtapaPadraoContaConfiguration());
            modelBuilder.ApplyConfiguration(new ContaConfiguration());
            modelBuilder.ApplyConfiguration(new DimensionamentoConfiguration());
            modelBuilder.ApplyConfiguration(new UsuarioConfiguration());
            modelBuilder.Seed();
            base.OnModelCreating(modelBuilder);
        }

        public virtual DbSet<Conta> Contas { get; set; }
        public virtual DbSet<Eleicao> Eleicoes { get; set; }
        public virtual DbSet<Eleitor> Eleitores { get; set; }
        public virtual DbSet<Inscricao> Inscricoes { get; set; }
        public virtual DbSet<Reprovacao> Reprovacoes { get; set; }
        public virtual DbSet<Empresa> Empresas { get; set; }
        public virtual DbSet<Estabelecimento> Estabelecimentos { get; set; }
        public virtual DbSet<EtapaObrigatoria> EtapasObrigatorias { get; set; }
        public virtual DbSet<EtapaCronograma> EtapasCronogramas { get; set; }
        public virtual DbSet<Usuario> Usuarios { get; set; }
        // public virtual DbSet<Arquivo> Arquivos { get; set; }
        // public virtual DbSet<Importacao> Importacoes { get; set; }
        // public virtual DbSet<Inconsistencia> Inconsistencias { get; set; }
        public virtual DbSet<Voto> Votos { get; set; }
        public virtual DbSet<Grupo> Grupos { get; set; }
        public virtual DbSet<LinhaDimensionamento> LinhasDimensionamentos { get; set; }
        public virtual DbSet<LimiteDimensionamento> LimitesDimensionamentos { get; set; }
        public virtual DbSet<Dimensionamento> Dimensionamentos { get; set; }

        // public virtual DbSet<Email> Emails { get; set; }
        // public virtual DbSet<ProcessamentoEtapa> ProcessamentoEtapas { get; set; }
        public virtual DbSet<EtapaPadraoConta> EtapasPadroesContas { get; set; }


        public override int SaveChanges()
        {
            foreach (var entry in ChangeTracker.Entries().Where(entry => entry.Entity.GetType().GetProperty("DataCadastro") != null))
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Property("DataCadastro").CurrentValue = DateTime.Now;
                }
                if (entry.State == EntityState.Modified)
                {
                    entry.Property("DataCadastro").IsModified = false;
                }
            }
            return base.SaveChanges();
        }
    }
}
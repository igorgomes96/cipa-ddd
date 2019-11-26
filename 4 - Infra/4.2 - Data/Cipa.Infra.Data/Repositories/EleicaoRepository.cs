using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Cipa.Domain.Interfaces.Repositories;
using Cipa.Domain.Entities;
using Cipa.Infra.Data.Context;

namespace Cipa.Infra.Data.Repositories
{
    public class EleicaoRepository : RepositoryBase<Eleicao>, IEleicaoRepository
    {
        public EleicaoRepository(CipaContext db) : base(db) { }
        
        public IEnumerable<Eleicao> BuscarPelaConta(int contaId) => DbSet.Where(e => e.ContaId == contaId);

        public IEnumerable<Eleicao> BuscarPeloUsuario(int usuarioId) => DbSet.Where(e => e.UsuarioCriacaoId == usuarioId);

        public IEnumerable<Inscricao> BuscarInscricoes(int eleicaoId, StatusInscricao? status = null)
        {
            var eleicoes = _db.Inscricoes.Where(i => i.EleicaoId == eleicaoId);
            if (!status.HasValue) return eleicoes;
            return eleicoes.Where(i => i.StatusInscricao == status);
        }

        public IEnumerable<Eleitor> BuscarEleitores(int id) => _db.Eleitores.Where(e => e.EleicaoId == id);

        public IEnumerable<Voto> BuscarVotos(int id) => _db.Votos.Where(v => v.EleicaoId == id);
        
    }
}
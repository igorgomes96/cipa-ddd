using System.Linq;
using Cipa.Domain.Entities;
using Cipa.Application.Repositories;
using Cipa.Infra.Data.Context;

namespace Cipa.Infra.Data.Repositories
{
    public class ImportacaoRepository : RepositoryBase<Importacao>, IImportacaoRepository
    {
        public ImportacaoRepository(CipaContext db) : base(db) { }

        public Importacao BuscarPrimeiraImportacaoPendenteDaFila() =>
            _db.Importacoes
                .Where(i => i.Status == StatusImportacao.Aguardando)
                .OrderBy(i => i.DataCadastro)
                .FirstOrDefault();
    }
}
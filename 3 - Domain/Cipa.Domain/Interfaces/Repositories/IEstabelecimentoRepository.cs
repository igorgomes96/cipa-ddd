using Cipa.Domain.Entities;

namespace Cipa.Domain.Interfaces.Repositories {
    public interface IEstabelecimentoRepository: IRepositoryBase<Estabelecimento>
    {
        int QuantidadeEleicoesAno(int estabelecimentoId, int ano);
    }
}
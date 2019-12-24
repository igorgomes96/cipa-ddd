using Cipa.Application.Interfaces;
using Cipa.Domain.Entities;
using Cipa.Domain.Exceptions;
using Cipa.Application.Repositories;
using System.Collections.Generic;

namespace Cipa.Application
{
    public class ContaAppService : AppServiceBase<Conta>, IContaAppService
    {
        public ContaAppService(IUnitOfWork unitOfWork) : base(unitOfWork, unitOfWork.ContaRepository)
        {
        }

        public EtapaPadraoConta AdicionarEtapaPadrao(int contaId, EtapaPadraoConta etapaPadrao)
        {
            var conta = _repositoryBase.BuscarPeloId(contaId);
            if (conta == null) throw new NotFoundException("Conta não encontrada.");
            conta.AdicionarEtapaPadrao(etapaPadrao);
            base.Atualizar(conta);
            return etapaPadrao;
        }

        public EtapaPadraoConta AtualizarEtapaPadrao(int contaId, EtapaPadraoConta etapaPadrao)
        {
            var conta = _repositoryBase.BuscarPeloId(contaId);
            if (conta == null) throw new NotFoundException("Conta não encontrada.");
            var etapaPadraoExistente = conta.BuscarEtapaPadraoPeloId(etapaPadrao.Id);
            if (etapaPadraoExistente == null) throw new NotFoundException("Etapa não encontrada.");

            etapaPadraoExistente.Nome = etapaPadrao.Nome;
            etapaPadraoExistente.Descricao = etapaPadrao.Descricao;
            etapaPadraoExistente.DuracaoPadrao = etapaPadrao.DuracaoPadrao;

            conta.AtualizarEtapaPadrao(etapaPadraoExistente);
            base.Atualizar(conta);
            return etapaPadraoExistente;
        }

        public IEnumerable<EtapaPadraoConta> BuscarCronogramaPadrao(int contaId)
        {
            var conta = _repositoryBase.BuscarPeloId(contaId);
            if (conta == null) throw new NotFoundException("Conta não encontrada.");
            return conta.EtapasPadroes;
        }

        public EtapaPadraoConta RemoverEtapaPadrao(int contaId, int etapaPadraoId)
        {
            var conta = _repositoryBase.BuscarPeloId(contaId);
            if (conta == null) throw new NotFoundException("Conta não encontrada.");
            var etapaPadrao = conta.BuscarEtapaPadraoPeloId(etapaPadraoId);
            if (etapaPadrao == null) throw new NotFoundException("Etapa não encontrada.");
            conta.RemoverEtapaPadrao(etapaPadrao);
            base.Atualizar(conta);
            return etapaPadrao;
        }
    }
}
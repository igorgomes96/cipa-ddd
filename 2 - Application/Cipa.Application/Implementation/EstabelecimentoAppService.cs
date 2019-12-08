using Cipa.Application.Interfaces;
using Cipa.Domain.Entities;
using Cipa.Domain.Exceptions;
using Cipa.Domain.Interfaces.Repositories;
using System.Collections.Generic;
using System.Linq;

namespace Cipa.Application
{
    public class EstabelecimentoAppService : AppServiceBase<Estabelecimento>, IEstabelecimentoAppService
    {
        public EstabelecimentoAppService(IUnitOfWork unitOfWork) : base(unitOfWork, unitOfWork.EstabelecimentoRepository) { }

        public IEnumerable<Estabelecimento> BuscarEstabelecimentosPorConta(int contaId) =>
            (_repositoryBase as IEstabelecimentoRepository).BuscarEstabelecimentosPorConta(contaId);

        public IEnumerable<Estabelecimento> BuscarEstabelecimentosPorEmpresa(int empresaId) =>
            (_repositoryBase as IEstabelecimentoRepository).BuscarEstabelecimentosPorEmpresa(empresaId);

        public override Estabelecimento Adicionar(Estabelecimento estabalecimento)
        {
            var empresa = _unitOfWork.EmpresaRepository.BuscarPeloId(estabalecimento.EmpresaId);
            if (empresa == null) throw new NotFoundException("Empresa não encontrada.");

            estabalecimento.Empresa = empresa;
            estabalecimento.GrupoId = estabalecimento.GrupoId == 0 ? null : estabalecimento.GrupoId;
            estabalecimento.Grupo = BuscarGrupo(estabalecimento.GrupoId);
            return base.Adicionar(estabalecimento);
        }

        private Grupo BuscarGrupo(int? grupoId) =>
            !grupoId.HasValue || grupoId == 0 ? null : _unitOfWork.GrupoRepository.BuscarPeloId(grupoId.Value);

        public override void Atualizar(Estabelecimento estabelecimento)
        {
            var estabelecimentoExistente = (_repositoryBase as IEstabelecimentoRepository).BuscarPeloId(estabelecimento.Id);
            if (estabelecimentoExistente == null) throw new NotFoundException("Estabelecimento não encontrado.");

            estabelecimentoExistente.GrupoId = estabelecimento.GrupoId == 0 ? null : estabelecimento.GrupoId;
            estabelecimentoExistente.Grupo = BuscarGrupo(estabelecimento.GrupoId);
            estabelecimentoExistente.Descricao = estabelecimento.Descricao;
            estabelecimentoExistente.Cidade = estabelecimento.Cidade;
            estabelecimentoExistente.Endereco = estabelecimento.Endereco;

            base.Atualizar(estabelecimentoExistente);
        }

        public override Estabelecimento Excluir(int id)
        {
            var estabelecimento = (_repositoryBase as IEstabelecimentoRepository).BuscarPeloId(id); //.BuscarPeloIdCarregarEleicoes(id);
            if (estabelecimento == null) throw new NotFoundException("Estabelecimento não encontrado.");

            if (estabelecimento.Eleicoes.Any())
            {
                estabelecimento.Inativar();
                base.Atualizar(estabelecimento);
            }
            else
                base.Excluir(estabelecimento);

            return estabelecimento;
        }
    }
}
using Cipa.Application.Interfaces;
using Cipa.Domain.Entities;
using Cipa.Domain.Exceptions;
using Cipa.Application.Repositories;
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

        public override Estabelecimento Adicionar(Estabelecimento estabelecimento)
        {
            var empresa = _unitOfWork.EmpresaRepository.BuscarPeloId(estabelecimento.EmpresaId);
            if (empresa == null) throw new NotFoundException("Empresa não encontrada.");

            estabelecimento.Empresa = empresa;
            estabelecimento.GrupoId = estabelecimento.GrupoId == 0 ? null : estabelecimento.GrupoId;
            estabelecimento.Grupo = BuscarGrupo(estabelecimento.GrupoId);
            return base.Adicionar(estabelecimento);
        }

        private Grupo BuscarGrupo(int? grupoId) =>
            !grupoId.HasValue || grupoId == 0 ? null : _unitOfWork.GrupoRepository.BuscarPeloId(grupoId.Value);

        public override void Atualizar(Estabelecimento estabelecimento)
        {
            var estabelecimentoExistente = _repositoryBase.BuscarPeloId(estabelecimento.Id);
            if (estabelecimentoExistente == null) throw new NotFoundException("Estabelecimento não encontrado.");

            var empresa = _unitOfWork.EmpresaRepository.BuscarPeloId(estabelecimento.EmpresaId);
            if (empresa == null) throw new NotFoundException("Empresa não encontrada.");

            estabelecimentoExistente.Empresa = empresa;
            estabelecimentoExistente.EmpresaId = empresa.Id;
            estabelecimentoExistente.GrupoId = estabelecimento.GrupoId == 0 ? null : estabelecimento.GrupoId;
            estabelecimentoExistente.Grupo = BuscarGrupo(estabelecimento.GrupoId);
            estabelecimentoExistente.Descricao = estabelecimento.Descricao;
            estabelecimentoExistente.Cidade = estabelecimento.Cidade;
            estabelecimentoExistente.Endereco = estabelecimento.Endereco;

            base.Atualizar(estabelecimentoExistente);
        }

        public override Estabelecimento Excluir(int id)
        {
            var estabelecimento = _repositoryBase.BuscarPeloId(id);
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
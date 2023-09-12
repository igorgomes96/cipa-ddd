using Cipa.Application.Interfaces;
using Cipa.Domain.Entities;
using Cipa.Domain.Exceptions;
using Cipa.Application.Repositories;
using System.Collections.Generic;

namespace Cipa.Application.Implementation
{
    public class EmpresaAppService : AppServiceBase<Empresa>, IEmpresaAppService
    {

        public EmpresaAppService(IUnitOfWork unitOfWork) : base(unitOfWork, unitOfWork.EmpresaRepository) { }

        public IEnumerable<Empresa> BuscaEmpresasPorConta(int contaId, bool? ativa = true) =>
            (_repositoryBase as IEmpresaRepository).BuscarEmpresasPorConta(contaId, ativa);

        public override Empresa Excluir(int id)
        {
            var empresa = _repositoryBase.BuscarPeloId(id);
            if (empresa == null) throw new NotFoundException("Empresa não encontrada.");

            if (empresa.Estabelecimentos.Count > 0)
            {
                empresa.Inativar();
                base.Atualizar(empresa);
            }
            else
            {
                _repositoryBase.Excluir(empresa);
                _unitOfWork.Commit();
            }

            return empresa;
        }

        public override void Atualizar(Empresa empresa)
        {
            var empresaExistente = _repositoryBase.BuscarPeloId(empresa.Id);
            if (empresaExistente == null) throw new NotFoundException("Empresa não encontrada.");

            empresaExistente.InformacoesGerais = empresa.InformacoesGerais;
            empresaExistente.Cnpj = empresa.Cnpj;
            empresaExistente.RazaoSocial = empresa.RazaoSocial;

            base.Atualizar(empresaExistente);
        }
    }
}
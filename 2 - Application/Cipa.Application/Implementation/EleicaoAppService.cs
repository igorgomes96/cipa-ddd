using System.Collections.Generic;
using Cipa.Application.Interfaces;
using Cipa.Domain.Entities;
using Cipa.Domain.Exceptions;
using Cipa.Domain.Interfaces.Services;

namespace Cipa.Application
{
    public class EleicaoAppService : AppServiceBase<Eleicao>, IEleicaoAppService
    {   
        private readonly IEleicaoService _eleicaoService;
        private readonly IUsuarioService _usuarioService;
        private readonly IEstabelecimentoService _estabelecimentoService;
        private readonly IContaService _contaService;
        private readonly IGrupoService _grupoService;
        public EleicaoAppService(
            IEleicaoService eleicaoService,
            IEstabelecimentoService estabelecimentoService,
            IUsuarioService usuarioService,
            IContaService contaService,
            IGrupoService grupoService) : base(eleicaoService)
        {
            _eleicaoService = eleicaoService;
            _usuarioService = usuarioService;
            _estabelecimentoService = estabelecimentoService;
            _contaService = contaService;
            _grupoService = grupoService;
        }

        public override Eleicao Adicionar(Eleicao eleicao)
        {
            if (eleicao.Gestao == 0)
                eleicao.Gestao = eleicao.TerminoMandatoAnterior?.Year ?? eleicao.DataInicio.Year;
            eleicao.Estabelecimento = _estabelecimentoService.BuscarPeloId(eleicao.EstabelecimentoId);
            if (eleicao.Estabelecimento == null)
                throw new NotFoundException($"Estabelecimento {eleicao.EstabelecimentoId} não encontrado.");
            eleicao.Conta = _contaService.BuscarPeloId(eleicao.ContaId);
            eleicao.Grupo = _grupoService.BuscarPeloId(eleicao.GrupoId);
            if (eleicao.Grupo == null)
                throw new NotFoundException($"Grupo {eleicao.GrupoId} não encontrado.");
            return base.Adicionar(eleicao);
        }

        public IEnumerable<Eleicao> BuscarPelaConta(int contaId)
        {
            var conta = _contaService.BuscarPeloId(contaId);
            if (conta == null) throw new CustomException("Conta inválida!");
            return _eleicaoService.BuscarPelaConta(conta);
        }

        public IEnumerable<Eleicao> BuscarPeloUsuario(int usuarioId)
        {
            var usuario = _usuarioService.BuscarPeloId(usuarioId);
            if (usuario == null) throw new CustomException("Usuário inválido!");
            return _eleicaoService.BuscarPeloUsuario(usuario);
        }
    }
}
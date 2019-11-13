using System.Collections.Generic;
using System.Linq;
using Cipa.Domain.Entities;
using Cipa.Domain.Exceptions;
using Cipa.Domain.Helpers;
using Cipa.Domain.Interfaces.Repositories;
using Cipa.Domain.Interfaces.Services;

namespace Cipa.Domain.Services
{
    public class EleicaoService : ServiceBase<Eleicao>, IEleicaoService
    {
        private readonly IEleicaoRepository _eleicaoRepository;
        private readonly IEstabelecimentoRepository _estabelecimentoRepository;

        public EleicaoService(
            IEleicaoRepository eleicaoRepository,
            IEstabelecimentoRepository estabelecimentoRepository
        ) : base(eleicaoRepository)
        {
            _eleicaoRepository = eleicaoRepository;
            _estabelecimentoRepository = estabelecimentoRepository;
        }

        public override Eleicao Adicionar(Eleicao eleicao)
        {
            int qtdaEleicoes = _estabelecimentoRepository.QuantidadeEleicoesAno(eleicao.Estabelecimento, eleicao.Gestao);
            if (qtdaEleicoes > 0)
                throw new CustomException($"Já há uma eleição cadastrada para este estabelecimento no ano de {eleicao.Gestao}.");

            eleicao.GerarCronograma();
            return _repository.Adicionar(eleicao);
        }

        public override void Atualizar(Eleicao eleicao)
        {
            Eleicao eleicaoOld = BuscarPeloId(eleicao.Id);
            (eleicaoOld.Inscricoes as List<Inscricao>).AddRange(_eleicaoRepository.BuscarInscricoes(eleicaoOld));
            if (eleicaoOld == null) throw new NotFoundException("Código de eleição não encontrado.");
            bool jaPassouPeriodoInscricao = eleicaoOld.JaUltrapassouEtapa(CodigoEtapaObrigatoria.Inscricao);
            if (jaPassouPeriodoInscricao && eleicaoOld.GrupoId != eleicao.GrupoId)
            {
                var dimensionamentoOld = CalcularDimensionamentoEleicao(eleicaoOld);
                var novoDimensionamento = eleicao.Grupo.CalcularDimensionamento(dimensionamentoOld.QtdaEleitores);
                if (novoDimensionamento.TotalCipeiros > dimensionamentoOld.QtdaInscricoesAprovadas)
                {
                    throw new CustomException($"Para o grupo {eleicao.Grupo.CodigoGrupo}, o mínimo de inscrições necessária é {novoDimensionamento.TotalCipeiros}, e só houveram {dimensionamentoOld.QtdaInscricoesAprovadas} inscrições aprovadas nessa eleição.");
                }
                eleicao.AtualizarDimensionamento(novoDimensionamento);
            }
            base.Atualizar(eleicao);
        }

        public IEnumerable<Eleicao> BuscarPelaConta(Conta conta)
        {
            return _eleicaoRepository.BuscarPelaConta(conta);
        }

        public IEnumerable<Eleicao> BuscarPeloUsuario(Usuario usuario)
        {
            return _eleicaoRepository.BuscarPeloUsuario(usuario);
        }

        public Dimensionamento CalcularDimensionamentoEleicao(Eleicao eleicao)
        {
            var qtdaEleitores = _eleicaoRepository.QtdaEleitores(eleicao);
            var dimensionamento = eleicao.Grupo.CalcularDimensionamento(qtdaEleitores);
            var qtdaVotos = _eleicaoRepository.QtdaVotos(eleicao);
            return new Dimensionamento(dimensionamento) {
                QtdaEleitores = qtdaEleitores,
                QtdaVotos = qtdaVotos,
                QtdaInscricoesAprovadas = eleicao.QtdaInscricoes(StatusInscricao.Aprovada),
                QtdaInscricoesPendentes = eleicao.QtdaInscricoes(StatusInscricao.Pendente),
                QtdaInscricoesReprovadas = eleicao.QtdaInscricoes(StatusInscricao.Reprovada),
            };
        }
    }
}
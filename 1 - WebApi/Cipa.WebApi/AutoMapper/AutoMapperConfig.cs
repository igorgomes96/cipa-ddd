using AutoMapper;
using Cipa.Application.Events.EventsArgs;
using Cipa.Domain.Entities;
using Cipa.Domain.Helpers;
using Cipa.WebApi.ViewModels;
using System.Collections.Generic;

namespace Cipa.WebApi.AutoMapper
{
    public class AutoMapperConfig
    {
        public static IMapper MapperConfig()
        {
            var config = new MapperConfiguration(cfg =>
            {

                cfg.CreateMap<Eleicao, EleicaoViewModel>()
                 .ForMember(dest => dest.Grupo, opt => opt.MapFrom(e => e.Grupo.CodigoGrupo))
                 .ReverseMap()
                 .ConvertUsing(dest => new Eleicao(
                     dest.Id, dest.DataInicio.Value, dest.DuracaoGestao.Value, dest.EstabelecimentoId.Value, dest.GrupoId.Value,
                     dest.UsuarioCriacaoId, dest.ContaId, dest.TerminoMandatoAnterior));

                cfg.CreateMap<Eleicao, EleicaoDetalheViewModel>()
                    .ForMember(dest => dest.Grupo, opt => opt.MapFrom(src => src.Grupo.CodigoGrupo))
                    .ForMember(dest => dest.InscricoesFinalizadas, opt => opt.MapFrom(src => src.JaUltrapassouEtapa(CodigoEtapaObrigatoria.Inscricao)))
                    .ForMember(dest => dest.VotacaoFinalizada, opt => opt.MapFrom(src => src.JaUltrapassouEtapa(CodigoEtapaObrigatoria.Votacao)))
                    .ForMember(dest => dest.InicioInscricao, opt => opt.MapFrom(src => src.BuscarEtapaObrigatoria(CodigoEtapaObrigatoria.Inscricao).Data))
                    .ForMember(dest => dest.InicioVotacao, opt => opt.MapFrom(src => src.BuscarEtapaObrigatoria(CodigoEtapaObrigatoria.Votacao).Data))
                    .ForMember(dest => dest.TerminoInscricao, opt => opt.MapFrom(src => src.DataTerminoEtapa(src.BuscarEtapaObrigatoria(CodigoEtapaObrigatoria.Inscricao))))
                    .ForMember(dest => dest.TerminoVotacao, opt => opt.MapFrom(src => src.DataTerminoEtapa(src.BuscarEtapaObrigatoria(CodigoEtapaObrigatoria.Votacao))));

                cfg.CreateMap<Empresa, EmpresaViewModel>().ReverseMap();
                cfg.CreateMap<Estabelecimento, EstabelecimentoViewModel>()
                    .ForMember(dest => dest.Grupo, opt => opt.MapFrom(e => e.Grupo.CodigoGrupo))
                    .ReverseMap()
                    .ForPath(src => src.Empresa, opt => opt.MapFrom(dest => (Empresa)null))
                    .ForPath(src => src.Grupo, opt => opt.MapFrom(dest => (Grupo)null));
                cfg.CreateMap<EtapaCronograma, EtapaCronogramaViewModel>().ReverseMap();
                cfg.CreateMap<Eleitor, EleitorViewModel>().ReverseMap();
                cfg.CreateMap<Inscricao, InscricaoViewModel>()
                    .ForMember(dest => dest.StatusAprovacao, opt => opt.MapFrom(src => src.StatusInscricao.ToString("g")));
                cfg.CreateMap<Inscricao, InscricaoDetalhesViewModel>()
                    .ForMember(dest => dest.StatusAprovacao, opt => opt.MapFrom(src => src.StatusInscricao.ToString("g")));
                cfg.CreateMap<Reprovacao, ReprovacaoViewModel>()
                    .ForMember(dest => dest.Horario, opt => opt.MapFrom(r => r.DataCadastro)).ReverseMap();
                cfg.CreateMap<Voto, VotoViewModel>().ForMember(dest => dest.Horario, opt => opt.MapFrom(src => src.DataCadastro));
                cfg.CreateMap<Dimensionamento, DimensionamentoViewModel>();
                cfg.CreateMap<Inscricao, ApuracaoViewModel>().ConvertUsing(src => new ApuracaoViewModel
                {
                    EleicaoId = src.EleicaoId,
                    Area = src.Eleitor.Area,
                    Cargo = src.Eleitor.Cargo,
                    DataAdmissao = src.Eleitor.DataAdmissao,
                    DataNascimento = src.Eleitor.DataNascimento,
                    Email = src.Eleitor.Email,
                    InscricaoId = src.Id,
                    Matricula = src.Eleitor.Matricula,
                    Nome = src.Eleitor.Nome,
                    ResultadoApuracao = ConverteResultadoApuracao(src.ResultadoApuracao),
                    Votos = src.Votos
                });
                cfg.CreateMap<LimiteDimensionamento, LimiteDimensionamentoViewModel>();
                cfg.CreateMap<LinhaDimensionamento, LinhaDimensionamentoViewModel>();
                cfg.CreateMap<Grupo, GrupoViewModel>();
                cfg.CreateMap<Grupo, GrupoDetalhesViewModel>();
                cfg.CreateMap<EtapaPadraoConta, EtapaPadraoContaViewModel>().ReverseMap();
                cfg.CreateMap<Usuario, UsuarioViewModel>().ReverseMap()
                    .ConvertUsing(dest => new Usuario(dest.Email, dest.Nome, dest.Cargo));
                cfg.CreateMap<Conta, ContaViewModel>();
                cfg.CreateMap<IEnumerable<Inscricao>, ResultadoApuracaoViewModel>()
                    .ConvertUsing<ResultadoApuracaoTypeConverter>();
                cfg.CreateMap<Inconsistencia, InconsistenciaViewModel>();
                cfg.CreateMap<Importacao, ImportacaoViewModel>()
                    .ForMember(dest => dest.Horario, opt => opt.MapFrom(src => src.DataCadastro))
                    .ForMember(dest => dest.Status, opt => opt.MapFrom(src => StatusImportacaoResolver.MapStatus(src.Status)));
                cfg.CreateMap<FinalizacaoImportacaoStatusEventArgs, FinalizacaoImportacaoStatusViewModel>()
                    .ForMember(dest => dest.Status, opt => opt.MapFrom(src => StatusImportacaoResolver.MapStatus(src.Status)));
                cfg.CreateMap<Arquivo, ArquivoViewModel>()
                    .ForMember(dest => dest.DataUpload, opt => opt.MapFrom(src => src.DataCadastro));
            });
            return config.CreateMapper();
        }

        private static string ConverteResultadoApuracao(ResultadoApuracao resultado)
        {
            switch (resultado)
            {
                case ResultadoApuracao.Efetivo:
                    return "Efetivo";
                case ResultadoApuracao.Suplente:
                    return "Suplente";
                case ResultadoApuracao.NaoEleito:
                    return "Não eleito";
                default:
                    return "";
            }
        }
    }
}
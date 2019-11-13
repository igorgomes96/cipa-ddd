using AutoMapper;
using Cipa.Domain.Entities;
using Cipa.Domain.Helpers;
using Cipa.WebApi.ViewModels;

namespace Cipa.WebApi.AutoMapper {
    public class AutoMapperConfig {
        public static IMapper MapperConfig() {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Eleicao, EleicaoViewModel>()
                 .ForMember(dest => dest.Grupo, opt => opt.MapFrom(e => e.Grupo.CodigoGrupo))
                 .ReverseMap();
                cfg.CreateMap<Eleicao, EleicaoDetalheViewModel>()
                    .ForMember(dest => dest.Grupo, opt => opt.MapFrom(src => src.Grupo.CodigoGrupo))
                    .ForMember(dest => dest.InscricoesFinalizadas, opt => opt.MapFrom(src => src.JaUltrapassouEtapa(CodigoEtapaObrigatoria.Inscricao)))
                    .ForMember(dest => dest.VotacaoFinalizada, opt => opt.MapFrom(src => src.JaUltrapassouEtapa(CodigoEtapaObrigatoria.Votacao)))
                    .ForMember(dest => dest.InicioInscricao, opt => opt.MapFrom(src => src.BuscaEtapaObrigatoria(CodigoEtapaObrigatoria.Inscricao).Data))
                    .ForMember(dest => dest.InicioVotacao, opt => opt.MapFrom(src => src.BuscaEtapaObrigatoria(CodigoEtapaObrigatoria.Votacao).Data))
                    .ForMember(dest => dest.TerminoInscricao, opt => opt.MapFrom(src => src.DataTerminoEtapa(src.BuscaEtapaObrigatoria(CodigoEtapaObrigatoria.Inscricao))))
                    .ForMember(dest => dest.TerminoVotacao, opt => opt.MapFrom(src => src.DataTerminoEtapa(src.BuscaEtapaObrigatoria(CodigoEtapaObrigatoria.Votacao))));
                cfg.CreateMap<Empresa, EmpresaViewModel>().ReverseMap();
                cfg.CreateMap<Estabelecimento, EstabelecimentoViewModel>()
                 .ForMember(dest => dest.Grupo, opt => opt.MapFrom(e => e.Grupo.CodigoGrupo))
                 .ReverseMap();
                cfg.CreateMap<EtapaCronograma, EtapaCronogramaViewModel>().ReverseMap();
            });
            return config.CreateMapper();
        }
    }
}
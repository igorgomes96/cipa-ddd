using AutoMapper;
using AutoMapper.QueryableExtensions;
using Cipa.Domain.Entities;
using Cipa.WebApi.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace Cipa.WebApi.AutoMapper
{
    public class ResultadoApuracaoTypeConverter : ITypeConverter<IEnumerable<Inscricao>, ResultadoApuracaoViewModel>
    {
        public ResultadoApuracaoViewModel Convert(IEnumerable<Inscricao> source, ResultadoApuracaoViewModel destination, ResolutionContext context)
        {
            if (context == null || source == null)
                return null;

            return new ResultadoApuracaoViewModel
            {
                Efetivos = source
                    .Where(s => s.ResultadoApuracao == ResultadoApuracao.Efetivo).AsQueryable()
                    .ProjectTo<ApuracaoViewModel>(context.ConfigurationProvider),
                Suplentes = source
                    .Where(s => s.ResultadoApuracao == ResultadoApuracao.Suplente).AsQueryable()
                    .ProjectTo<ApuracaoViewModel>(context.ConfigurationProvider),
                NaoEleitos = source
                    .Where(s => s.ResultadoApuracao == ResultadoApuracao.NaoEleito).AsQueryable()
                    .ProjectTo<ApuracaoViewModel>(context.ConfigurationProvider)
            };
        }
    }
}

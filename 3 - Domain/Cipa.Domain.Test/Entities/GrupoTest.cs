using System;
using Cipa.Domain.Entities;
using Cipa.Domain.Exceptions;
using Cipa.Domain.Helpers;
using Xunit;

namespace Cipa.Domain.Test.Entities
{
    public class GrupoTest
    {

        [Theory]
        [InlineData(2501, 5000, 5000, 12, 9)]
        [InlineData(5001, 10000, 5001, 15, 12)]
        [InlineData(5001, 10000, 10000, 15, 12)]
        [InlineData(10001, 12500, 10001, 17, 15)]
        [InlineData(10001, 12500, 12500, 17, 15)]
        [InlineData(12501, 15000, 12501, 19, 18)]
        public void CalculaDimensionamento_LimiteDoDimensionamento_RetornaDimensionamento(
            int minimo, int maximo, int qtdaEleitores, int qtdaEfetivosEsperado, int qtdaSuplentesEsperado)
        {
            var grupo = new Grupo("C-Teste");
            grupo.Dimensionamentos.Add(new LinhaDimensionamento(5000, 2501, 12, 9));
            grupo.Dimensionamentos.Add(new LinhaDimensionamento( 10000, 5001, 15, 12));
            grupo.LimiteDimensionamento = new LimiteDimensionamento(10000, 2500, 2, 3);

            var dimensionamento = grupo.CalcularDimensionamento(qtdaEleitores);

            Assert.Equal(minimo, dimensionamento.Minimo);
            Assert.Equal(maximo, dimensionamento.Maximo);
            Assert.Equal(qtdaEfetivosEsperado, dimensionamento.QtdaEfetivos);
            Assert.Equal(qtdaSuplentesEsperado, dimensionamento.QtdaSuplentes);
            
        }

    }
}
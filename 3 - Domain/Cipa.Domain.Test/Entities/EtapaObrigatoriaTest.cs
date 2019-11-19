using System;
using Cipa.Domain.Entities;
using Cipa.Domain.Exceptions;
using Cipa.Domain.Helpers;
using Xunit;

namespace Cipa.Domain.Test.Entities
{
    public class EtapaObrigatoriaTest {

        [Fact]
        public void Equals_IdIgual_RetornaTrue() {
            var etapa1 = new EtapaObrigatoria {
                Id = CodigoEtapaObrigatoria.Ata,
                Nome = "Etapa 1"
            };
            var etapa2 = new EtapaObrigatoria {
                Id = CodigoEtapaObrigatoria.Ata,
                Nome = "Etapa 2"
            };

            Assert.Equal(etapa1, etapa2);
        }

        [Fact]
        public void Equals_IdDiferente_RetornaFalse() {
            var etapa1 = new EtapaObrigatoria {
                Id = CodigoEtapaObrigatoria.Ata,
                Nome = "Etapa 1"
            };
            var etapa2 = new EtapaObrigatoria {
                Id = CodigoEtapaObrigatoria.Convocacao,
                Nome = "Etapa 1"
            };

            Assert.NotEqual(etapa1, etapa2);
        }
    }
}
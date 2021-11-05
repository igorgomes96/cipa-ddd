using System.Linq;
using Cipa.Domain.Entities;
using Xunit;


namespace Cipa.Domain.Test.Entities {
    #pragma warning disable xUnit2013
    public class EmailTest {
        [Fact]
        public void DividirDestinatarios_AcimaDoMaximo_DivideArray()
        {
            // Arrange
            var qtda = 21;
            var destinatarios = "";
            for (var i = 1; i <= qtda; i++)
                destinatarios += $"dest{i}@email.com,";
            var email = new Email(destinatarios, null, "Teste", "Mensagem")
            {
                MaxDestinatarios = 10
            };

            // Act
            var emails = email.DividirDestinatarios().ToArray();

            // Assert
            Assert.Equal(3, emails.Count());
            Assert.Equal(10, emails[0].DestinatariosLista.Count());
            Assert.Equal(10, emails[1].DestinatariosLista.Count());
            Assert.Equal(1, emails[2].DestinatariosLista.Count());

            for (var i = 1; i <= 10; i++)
                Assert.Equal($"dest{i}@email.com", emails[0].DestinatariosLista.ToArray()[i - 1]);

            for (var i = 11; i <= 20; i++)
                Assert.Equal($"dest{i}@email.com", emails[1].DestinatariosLista.ToArray()[i - 11]);

            for (var i = 21; i <= 21; i++)
                Assert.Equal($"dest{i}@email.com", emails[2].DestinatariosLista.ToArray()[i - 21]);
        }
    }
}
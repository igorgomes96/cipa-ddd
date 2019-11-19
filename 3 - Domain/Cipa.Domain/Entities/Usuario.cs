
using System;

namespace Cipa.Domain.Entities
{
    public class Usuario: Entity<int>
    {
        public string Email { get; set; }
        public string Nome { get; set; }
        public string Senha { get; set; }
        public int? ContaId { get; set; }
        public string Perfil { get; set; }
        // Código utilizado para criar uma conta (sem data de expiração) ou para recuperação de senha esquecida
        public Guid? CodigoRecuperacao { get; set; }
        public DateTime? ExpiracaoCodigoRecuperacao { get; set; }
        public string Cargo { get; set; }
        public DateTime DataCadastro { get; set; }

        public virtual Conta Conta { get; set; }
    }
}

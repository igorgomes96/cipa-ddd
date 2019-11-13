using System;

namespace Cipa.Domain.Entities
{
    public class Eleitor
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Matricula { get; set; }
        public string Area { get; set; }
        public string Cargo { get; set; }
        public DateTime? DataNascimento { get; set; }
        public DateTime? DataAdmissao { get; set; }
        public int EleicaoId { get; set; }
        public int UsuarioId { get; set; }
        public string NomeGestor { get; set; }
        public string EmailGestor { get; set; }

        public virtual Eleicao Eleicao { get; set; }
        public virtual Usuario Usuario { get; set; }
        public virtual Inscricao Inscricao { get; set; }
    }
}
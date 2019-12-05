using System;

namespace Cipa.Domain.Entities
{
    public class Eleitor : Entity<int>
    {
        public Eleitor(string nome, string email)
        {
            Nome = nome;
            Email = email;
        }

        public Eleitor(Usuario usuario)
        {
            Usuario = usuario;
            UsuarioId = usuario.Id;
            Nome = usuario.Nome;
            Email = usuario.Email;
        }

        public string Nome { get; set; }
        public string Email { get; set; }
        public string Matricula { get; set; }
        public string Area { get; set; }
        public string Cargo { get; set; }
        public DateTime? DataNascimento { get; set; }
        public DateTime? DataAdmissao { get; set; }
        public int EleicaoId { get; private set; }
        public int UsuarioId { get; private set; }
        public string NomeGestor { get; set; }
        public string EmailGestor { get; set; }

        public virtual Eleicao Eleicao { get; private set; }
        public virtual Usuario Usuario { get; set; }
        public virtual Inscricao Inscricao { get; private set; }
        public virtual Voto Voto { get; private set; }

        internal void Atualizar(Eleitor eleitor)
        {
            Nome = eleitor.Nome;
            Email = eleitor.Email;
            Matricula = eleitor.Matricula;
            Area = eleitor.Area;
            Cargo = eleitor.Cargo;
            DataNascimento = eleitor.DataNascimento;
            DataAdmissao = eleitor.DataAdmissao;
            NomeGestor = eleitor.NomeGestor;
            EmailGestor = eleitor.EmailGestor;
            Usuario = eleitor.Usuario;
            UsuarioId = eleitor.UsuarioId == 0 ? eleitor.Usuario.Id : eleitor.UsuarioId;
        }
    }
}
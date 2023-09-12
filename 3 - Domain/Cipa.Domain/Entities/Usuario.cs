using Cipa.Domain.Exceptions;
using Cipa.Domain.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Cipa.Domain.Entities
{
    public class Usuario : Entity<int>
    {
        public Usuario(string email, string nome, string cargo)
        {
            Email = email.Trim().ToLower();
            Nome = nome;
            Cargo = cargo;
            CodigoRecuperacao = Guid.NewGuid();
            ExpiracaoCodigoRecuperacao = DateTime.Now.HorarioBrasilia().AddDays(1);
            Perfil = PerfilUsuario.Eleitor;
        }

        public Usuario(string email, string nome, string cargo, string senha)
            : this(email, nome, cargo)
        {
            Senha = senha;
        }

        public string Email { get; set; }
        public string Nome { get; set; }
        public string Senha { get; set; }
        public int? ContaId { get; set; }
        public string Perfil { get; set; }
        // Código utilizado para criar uma conta (sem data de expiração) ou para recuperação de senha esquecida
        public Guid? CodigoRecuperacao { get; private set; }
        public DateTime? ExpiracaoCodigoRecuperacao { get; private set; }
        public string Cargo { get; set; }
        public DateTime DataCadastro { get; private set; }

        public virtual Conta Conta { get; set; }
        private List<Eleitor> _eleitores = new List<Eleitor>();
        public virtual IReadOnlyCollection<Eleitor> Eleitores { get => new ReadOnlyCollection<Eleitor>(_eleitores); }

        public void AlterarParaPerfilEleitor()
        {
            Perfil = PerfilUsuario.Eleitor;
            ContaId = null;
            Conta = null;
        }

        public void AlterarParaPerfilSESMT(Conta conta)
        {
            Perfil = PerfilUsuario.SESMT;
            ContaId = conta.Id;
            Conta = conta;
        }

        public void AlterarParaPerfilAdministrador()
        {
            Perfil = PerfilUsuario.Administrador;
        }

        public bool JaCadastrouSenha => !string.IsNullOrWhiteSpace(Senha);

        public void CadastrarSenha(Guid codigoRecuperacao, string senha)
        {
            if (codigoRecuperacao != CodigoRecuperacao)
                throw new CustomException("Código de recuperação inválido.");

            if (ExpiracaoCodigoRecuperacao.HasValue && DateTime.Now.HorarioBrasilia() > ExpiracaoCodigoRecuperacao)
                throw new CustomException("Código de recuperação expirado.");

            CodigoRecuperacao = null;
            ExpiracaoCodigoRecuperacao = null;
            Senha = senha;
        }

        public void CadastrarSenha(string senha)
        {
            Senha = senha;
        }

        public void ResetarSenha()
        {
            CodigoRecuperacao = Guid.NewGuid();
            ExpiracaoCodigoRecuperacao = DateTime.Now.HorarioBrasilia().AddDays(1);
            Senha = null;
        }
    }
}

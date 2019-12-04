using System;
using System.ComponentModel.DataAnnotations;

namespace Cipa.WebApi.ViewModels {
    public class EleitorViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "O nome do eleitor deve ser informado.")]
        [StringLength(255, ErrorMessage = "O nome do eleitor não pode conter mais de {1} caracteres.")]
        public string Nome { get; set; }
        [Required(ErrorMessage = "O e-mail do eleitor deve ser informado.")]
        [EmailAddress(ErrorMessage = "O e-mail está em formato inválido.")]
        [StringLength(100, ErrorMessage = "O e-mail do eleitor não pode conter mais de {1} caracteres.")]
        public string Email { get; set; }
        [StringLength(50, ErrorMessage = "A matrícula do eleitor não pode conter mais de {1} caracteres.")]
        public string Matricula { get; set; }
        [StringLength(255, ErrorMessage = "A área/departamento do eleitor não pode conter mais de {1} caracteres.")]
        public string Area { get; set; }
        [StringLength(255, ErrorMessage = "O cargo do eleitor não pode conter mais de {1} caracteres.")]
        public string Cargo { get; set; }
        public DateTime? DataNascimento { get; set; }
        public DateTime? DataAdmissao { get; set; }
        public int EleicaoId { get; set; }
        [StringLength(255, ErrorMessage = "O nome do gestor do eleitor não pode conter mais de {1} caracteres.")]
        public string NomeGestor { get; set; }
        [StringLength(255, ErrorMessage = "O e-mail do gestor do eleitor não pode conter mais de {1} caracteres.")]
        public string EmailGestor { get; set; }
        
    }
}
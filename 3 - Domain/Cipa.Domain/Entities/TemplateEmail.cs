using Cipa.Domain.Enums;

namespace Cipa.Domain.Entities
{
    public class TemplateEmail : Entity<int>
    {
        public TemplateEmail(ETipoTemplateEmail tipoTemplateEmail, string assunto)
        {
            TipoTemplateEmail = tipoTemplateEmail;
            Assunto = assunto;
        }

        public ETipoTemplateEmail TipoTemplateEmail { get; set; }
        public string Assunto { get; set; }
        public string Template { get; set; }

        public virtual Conta Conta { get; set; }
    }
}
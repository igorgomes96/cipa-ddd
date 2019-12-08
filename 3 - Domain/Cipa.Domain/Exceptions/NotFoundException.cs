namespace Cipa.Domain.Exceptions
{
    public class NotFoundException : CustomException
    {
        public NotFoundException() : base("Registro não encontrado!") { }
        public NotFoundException(string message) : base(message) { }
    }
}

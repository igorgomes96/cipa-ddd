namespace Cipa.Domain.Exceptions
{
    public class DuplicatedException : CustomException
    {
        public DuplicatedException() : base("Registro duplicado!") { }

        public DuplicatedException(string message): base(message) { }
    }
}

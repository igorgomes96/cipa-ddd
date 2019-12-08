namespace Cipa.Domain.Exceptions
{
    public class UnauthorizedException : CustomException
    {
        public UnauthorizedException() : base("Usuário sem permissão de acesso!") { }
        public UnauthorizedException(string message) : base(message) { }
    }
}

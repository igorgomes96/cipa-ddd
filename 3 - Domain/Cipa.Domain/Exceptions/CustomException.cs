using System;

namespace Cipa.Domain.Exceptions
{
    public class CustomException : Exception
    {
        public CustomException(string message): base(message) { }
    }
}

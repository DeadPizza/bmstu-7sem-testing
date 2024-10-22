using System.Runtime.Serialization;

namespace BasedGram.Services.AuthService.Exceptions;

[Serializable]
public class IncorrectPasswordException : Exception
{
    public IncorrectPasswordException() { }
    public IncorrectPasswordException(string message) : base(message) { }
    public IncorrectPasswordException(string message, System.Exception inner) : base(message, inner) { }
    // protected IncorrectPasswordExceptionException(SerializationInfo info,StreamingContext context) : base(info, context) { }
}
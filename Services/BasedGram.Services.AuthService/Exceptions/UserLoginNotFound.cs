using System.Runtime.Serialization;

namespace BasedGram.Services.AuthService.Exceptions;

[Serializable]
public class UserLoginNotFoundException : Exception
{
    public UserLoginNotFoundException() { }
    public UserLoginNotFoundException(string message) : base(message) { }
    public UserLoginNotFoundException(string message, Exception inner) : base(message, inner) { }
    // protected UserLoginNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context) { }
}
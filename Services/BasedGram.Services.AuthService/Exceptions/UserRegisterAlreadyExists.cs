using System.Runtime.Serialization;

namespace BasedGram.Services.AuthService.Exceptions;

[Serializable]
public class UserRegisterAlreadyExistsException : Exception
{
    public UserRegisterAlreadyExistsException() { }
    public UserRegisterAlreadyExistsException(string message) : base(message) { }
    public UserRegisterAlreadyExistsException(string message, Exception inner) : base(message, inner) { }
    // protected UserRegisterAlreadyExistsException(SerializationInfo info, StreamingContext context) : base(info, context) { }
}
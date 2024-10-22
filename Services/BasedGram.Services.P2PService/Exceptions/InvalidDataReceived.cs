using System.Runtime.Serialization;

namespace BasedGram.Services.P2PService.Exceptions;

[Serializable]
public class InvalidDataReceivedException : Exception
{
    public InvalidDataReceivedException() { }
    public InvalidDataReceivedException(string message) : base(message) { }
    public InvalidDataReceivedException(string message, Exception inner) : base(message, inner) { }
    // protected InvalidDataReceivedException(SerializationInfo info,StreamingContext context) : base(info, context) { }
}
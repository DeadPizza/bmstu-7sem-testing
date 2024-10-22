using System.Runtime.Serialization;

namespace BasedGram.Services.DialogService.Exceptions;

[Serializable]
public class CreateDialogExistsException : Exception
{
    public CreateDialogExistsException() { }
    public CreateDialogExistsException(string message) : base(message) { }
    public CreateDialogExistsException(string message, Exception inner) : base(message, inner) { }
    // protected CreateDialogExistsException(SerializationInfo info,StreamingContext context) : base(info, context) { }
}
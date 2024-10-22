namespace BasedGram.Services.DialogService.Exceptions;

[System.Serializable]
public class DeleteDialogNotExistsException : System.Exception
{
    public DeleteDialogNotExistsException() { }
    public DeleteDialogNotExistsException(string message) : base(message) { }
    public DeleteDialogNotExistsException(string message, System.Exception inner) : base(message, inner) { }
    // protected DeleteDialogNotExistsException(
    //     System.Runtime.Serialization.SerializationInfo info,
    //     System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}
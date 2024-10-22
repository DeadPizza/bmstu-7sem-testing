[System.Serializable]
public class UserCreateException : System.Exception
{
    public UserCreateException() { }
    public UserCreateException(string message) : base(message) { }
    public UserCreateException(string message, System.Exception inner) : base(message, inner) { }
    // protected UserCreateException(
    //     System.Runtime.Serialization.SerializationInfo info,
    //     System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}
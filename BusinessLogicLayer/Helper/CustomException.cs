namespace BusinessLogicLayer.Helper;

public class CustomException : Exception
{
    public CustomException(string message) : base(message)
    {
    }
}
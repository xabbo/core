namespace Xabbo.Core.Events;

public sealed class LoginErrorEventArgs
{
    public bool HasErrorCode { get; }
    public int ErrorCode { get; }

    public LoginErrorEventArgs()
    {
        HasErrorCode = false;
        ErrorCode = -1;
    }

    public LoginErrorEventArgs(int errorCode)
    {
        HasErrorCode = true;
        ErrorCode = errorCode;
    }
}

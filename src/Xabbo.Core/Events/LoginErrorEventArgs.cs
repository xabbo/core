using System;

namespace Xabbo.Core.Events;

public class LoginErrorEventArgs : EventArgs
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

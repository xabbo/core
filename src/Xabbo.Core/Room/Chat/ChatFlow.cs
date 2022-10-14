using System;
using System.ComponentModel;

namespace Xabbo.Core;

public enum ChatFlow
{
    [Description("Free flow")]
    FreeFlow = 0,
    [Description("Line by line")]
    LineByLine = 1
}

public static partial class XabboEnumExtensions
{
    public static string ToFriendlyString(this ChatFlow chatFlow)
    {
        switch (chatFlow)
        {
            case ChatFlow.FreeFlow: return "Free flow";
            case ChatFlow.LineByLine: return "Line by line";
            default: return chatFlow.ToString();
        }
    }
}

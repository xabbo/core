using System;

namespace Xabbo.Core.Messages
{
    public enum Destination { Unknown, Client, Server }

    public static partial class XabboEnumExtensions
    {
        public static string ToDirection(this Destination destination)
        {
            switch (destination)
            {
                case Destination.Client: return "Incoming";
                case Destination.Server: return "Outgoing";
                default: return "Unknown";
            }
        }

        public static Destination ToOpposite(this Destination destination)
        {
            switch (destination)
            {
                case Destination.Client: return Destination.Server;
                case Destination.Server: return Destination.Client;
                default: return Destination.Unknown;
            }
        }
    }
}

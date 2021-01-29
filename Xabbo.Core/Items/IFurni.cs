using System;

using Xabbo.Core.Game;
using Xabbo.Core.Protocol;

namespace Xabbo.Core
{
    public interface IFurni : IItem, IPacketData
    {
        /// <summary>
        /// Gets the owner of the furni's ID
        /// </summary>
        long OwnerId { get; }
        /// <summary>
        /// Gets the owner of the furni's name
        /// </summary>
        string OwnerName { get; }

        /// <summary>
        /// Gets the state of the furni from its data if the data string is an integer, otherwise returns <c>-1</c>.
        /// </summary>
        int State { get; }

        /// <summary>
        /// Gets the number of seconds until the furni expires.
        /// </summary>
        int SecondsToExpiration { get; }
        /// <summary>
        /// Gets the usage policy of the furni.
        /// </summary>
        FurniUsage Usage { get; }

        /// <summary>
        /// Gets whether the furni is hidden client-side or not by the <see cref="FurniManager"/>
        /// </summary>
        bool IsHidden { get; }

        void Write(IPacket packet, bool writeOwnerName);
    }
}

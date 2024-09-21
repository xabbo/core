using System.Threading.Tasks;
using Xabbo.Interceptor;
using Xabbo.Interceptor.Tasks;
using Xabbo.Messages;

namespace Xabbo.Core.Messages.Incoming;

public static class Asdf
{

}

public sealed record RoomDataMsg(RoomData Data) : IMessage<RoomDataMsg>
{
    public static Identifier Identifier => throw new System.NotImplementedException();

    public static RoomDataMsg Parse(in PacketReader p)
    {
        throw new System.NotImplementedException();
    }

    public void Compose(in PacketWriter p)
    {
        throw new System.NotImplementedException();
    }

}
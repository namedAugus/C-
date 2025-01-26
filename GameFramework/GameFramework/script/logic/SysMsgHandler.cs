using gprotocol;

public partial class MsgHandler
{
    public static void MsgPing(ClientState c, ProtoBuf.IExtensible msg)
    {
        Console.WriteLine("MsgPing handle in MsgHandler");
        c.lastPingTime = NetManager.GetTimeStamp();
        MsgPong msgPong = new MsgPong();
        NetManager.Send(c, msgPong);
    }
}

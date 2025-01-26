using System.Net.Sockets;
/// <summary>
/// 客户端连接状态类
/// </summary>
public class ClientState
{
    public Socket socket;
    public ByteArray readBuffer = new ByteArray(1024);
    //Ping
    public long lastPingTime = NetManager.GetTimeStamp();
    //玩家
    public Player player;
}

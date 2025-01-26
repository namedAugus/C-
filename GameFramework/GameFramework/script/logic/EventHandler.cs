
using System;
using EchoServer.script.logic;

public partial class EventHandler
{
    public static void OnDisconnect(ClientState c)
    {
        Console.WriteLine("Close...");
        //Player下线处理
        if (c.player != null)
        {
            //先判断是否加入了房间（玩家加入房间后网络断线的情况处理）
            int roomId = c.player.roomId;
            if (roomId >= 0)
            {
                Room room = RoomManager.GetRoom(roomId);
                room.RemovePlayer(c.player.id); //从房间中移除该玩家
            }
            //保存数据
            DBManager.UpdatePlayerData(c.player.id, c.player.data);
            //移除player（从服务器玩家全局变量中）
            PlayerManager.RemovePlayer(c.player.id);
        }
    }

    public static void OnTimer()
    {
        CheckPing();
        //房间管理器的ticker  用于执行每个room的ticker（update)
        RoomManager.Update();
    }
    //Ping检查
    public static void CheckPing()
    {
        //现在的时间戳
        long timeNow = NetManager.GetTimeStamp();
        //遍历，删除
        foreach (ClientState s in NetManager.clients.Values)
        {
            // Console.WriteLine("timeNow="+timeNow+"  lastPingTime="+s.lastPingTime);
            if (timeNow - s.lastPingTime > NetManager.pingInterval * 4)
            {
                Console.WriteLine("timeNow="+timeNow+"  lastPingTime="+s.lastPingTime);
                Console.WriteLine("Ping Close " + s.socket.RemoteEndPoint.ToString());
                NetManager.Close(s);
                return; //由于Close会改变clients的内存数量，所以每次都return，每次CheckPing最多断开一个连接
            }
        }
    }
}

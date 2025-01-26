using System;
using EchoServer.script.logic;
using gprotocol;

public partial class MsgHandler
{
    //处理查询战绩协议
    public static void MsgGetAchieve(ClientState c, ProtoBuf.IExtensible msgBase)
    {
        MsgGetAchieve msg = (MsgGetAchieve)msgBase;
        Player player = c.player;
        if(player == null) return;
        msg.win = player.data.win;
        msg.lost = player.data.lost;
        
        player.Send(msg);
    }
    
    //处理请求房间列表协议
    public static void MsgGetRoomList(ClientState c, ProtoBuf.IExtensible msgBase)
    {
        MsgGetRoomList msg = (MsgGetRoomList)msgBase;
        Player player = c.player;
        if(player == null) return;
        
        player.Send(RoomManager.ToMsg()); //这块直接用房间管理的方法返回房间列表信息给客户端
    }
    
    //处理玩家创建房间协议
    public static void MsgCreateRoom(ClientState c, ProtoBuf.IExtensible msgBase)
    {
        MsgCreateRoom msg = (MsgCreateRoom)msgBase;
        Player player = c.player;
        if(player == null) return;
        //已经在房间
        if (player.roomId >= 0)
        {
            msg.result = 0; //创建失败
            player.Send(msg);
            return;
        }
        //创建
        Room room = RoomManager.AddRoom();
        room.AddPlayer(player.id);

        msg.result = 1;
        player.Send(msg);
    }
    
    //处理玩家进入房间协议
    public static void MsgEnterRoom(ClientState c, ProtoBuf.IExtensible msgBase)
    {
        MsgEnterRoom msg = (MsgEnterRoom)msgBase;
        Player player = c.player;
        if(player == null) return;
        //已经在房间里
        if (player.roomId >= 0)
        {
            msg.result = 0;
            player.Send(msg);
            return;
        }
        //获取房间
        Room room = RoomManager.GetRoom(msg.id);
        if (room == null)
        {
            msg.result = 0;
            player.Send(msg);
            return;
        }
        //进入
        if (!room.AddPlayer(player.id))
        {
            msg.result = 0;
            player.Send(msg);
            return;
        }
        //返回协议
        msg.result = 1; //只有这个成功 1
        player.Send(msg);
    }
    
    //处理获取房间协议
    public static void MsgGetRoomInfo(ClientState c, ProtoBuf.IExtensible msgBase)
    {
        MsgGetRoomInfo msg = (MsgGetRoomInfo)msgBase;
        Player player = c.player;
        if(player == null) return;
        
        Room room = RoomManager.GetRoom(player.roomId);
        if (room == null)
        {
            player.Send(msg); //直接返回空
            return;
        }
        player.Send(room.ToMsg()); //返回消息列表
    }
    
    //处理玩家离开房间协议
    public static void MsgLeaveRoom(ClientState c, ProtoBuf.IExtensible msgBase)
    {
        MsgLeaveRoom msg = (MsgLeaveRoom)msgBase;
        Player player = c.player;
        if(player == null) return;
        
        Room room = RoomManager.GetRoom(player.roomId);
        if (room == null) //玩家没有（不在）房间
        {
            msg.result = 0;
            player.Send(msg);
            return;
        }
        room.RemovePlayer(player.id);
        //返回协议
        msg.result = 1;
        player.Send(msg);
        
    }
    
    //处理房间战斗===================================
    //请求开始战斗
    public static void MsgStartBattle(ClientState c, ProtoBuf.IExtensible msgBase)
    {
        MsgStartBattle msg = (MsgStartBattle)msgBase;
        Player player = c.player;
        if(player == null) return;
        //room
        Room room = RoomManager.GetRoom(player.roomId);
        if (room == null)
        {
            msg.result = 0;
            player.Send(msg);
            return;
        }
        //是否房主
        if (!room.IsOwner(player))
        {
            msg.result = 0;
            player.Send(msg);
            return;
        }
        //是否能开战
        if (!room.StartBattle())
        {
            msg.result = 0;
            player.Send(msg);
            return;
        }
        //成功
        msg.result = 1;
        player.Send(msg);
    }
    
   
}
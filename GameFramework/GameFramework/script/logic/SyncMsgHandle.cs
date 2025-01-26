using System;
using EchoServer.script.logic;
using gprotocol;

//处理同步
public partial class MsgHandler
{
    //同步位置协议
    public static void MsgSyncTank(ClientState c, ProtoBuf.IExtensible msgBase)
    {
        // return;
        MsgSyncTank msg = (MsgSyncTank)msgBase;
        Player player = c.player;
        if (player == null)
        {
            return;
        }
        
        //room
        Room room = RoomManager.GetRoom(player.roomId);
        if (room == null)
        {
            return;
        }
        //status
        if (room.status != Room.Status.FIGHT)
        {
            return;
        }
        //是否作弊
        if(Math.Abs(player.x - msg.x) > 5 ||
           Math.Abs(player.y - msg.y) > 5 || 
           Math.Abs(player.z - msg.z) > 5)
            {
                Console.WriteLine("该玩家疑似作弊：" + player.id);
            }
        //更新信息
        player.x = msg.x;
        player.y = msg.y;
        player.z = msg.z;
        player.ex = msg.ex;
        player.ey = msg.ey;
        player.ez = msg.ez;
        //广播
        msg.id = player.id; //填充id
        room.Broadcast(msg);
    }
    
    //处理开火协议
    public static void MsgFire(ClientState c, ProtoBuf.IExtensible msgBase)
    {
        MsgFire msg = (MsgFire)msgBase;
        Player player = c.player;
        if(player == null){return;}
        //room
        Room room = RoomManager.GetRoom(player.roomId);
        if(room == null){return;}
        //status
        if(room.status != Room.Status.FIGHT){return;}
        
        //广播
        msg.id = player.id;
        room.Broadcast(msg);
    }
    
    //处理击中协议
    public static void MsgHit(ClientState c, ProtoBuf.IExtensible msgBase)
    {
        MsgHit msg = (MsgHit)msgBase;
        Player player = c.player;
        if (player == null)
        {
            return;
        }

        //targetPlayer
        Player targetPlayer = PlayerManager.GetPlayer(msg.targetId);
        if (targetPlayer == null)
        {
            return;
        }

        //room
        Room room = RoomManager.GetRoom(player.roomId);
        if (room == null)
        {
            return;
        }

        //status
        if (room.status != Room.Status.FIGHT)
        {
            return;
        }

        //校验是否是发送者(可能收到多次，只处理炮弹发送者)
        if (player.id != msg.id)
        {
            return;
        }

        //状态
        int damage = 100;
        targetPlayer.hp = targetPlayer.hp - (damage - targetPlayer.def);
        //广播
        msg.id = player.id;
        msg.hp = targetPlayer.hp;
        msg.damage = damage;
        room.Broadcast(msg);
    }
}
using System;
using GameFramework.script.system;
using gprotocol;
using LitJson;

public partial class MsgHandler
{
    public static void MsgMove(ClientState c, ProtoBuf.IExtensible msgBase)
    {
        MsgMove msgMove = (MsgMove)msgBase;
        Console.WriteLine("x: " + msgMove.x);
        msgMove.x++;
        // NetManager.Send(c, msgMove);
        string json = JsonMapper.ToJson(msgMove);
        Console.WriteLine(json);
        foreach (Player player in PlayerManager.players.Values)
        {
            if (player.state == c)
            {
                Console.WriteLine("相等");
                if (player != null)
                {
                    Console.WriteLine("player is not null");
                }

                if (player.transform != null)
                {
                    Console.WriteLine("player transform is not null");
                }
                // Console.WriteLine(msgMove.transform == null);
                Console.WriteLine("x: " + msgMove.ox + " y: " + msgMove.oy
                                  + " tx: " + msgMove.tx +" = " +msgMove.ty);
                player.animState = (int)AnimState.Run;
                player.transform = new TransformComponent();
                player.transform.ox = msgMove.ox;
                player.transform.oy = msgMove.oy;
                player.transform.tx = msgMove.tx;
                player.transform.ty = msgMove.ty;
                
                // player.astarSystem = new AStarSystem();
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "map/yunhaixian_d.json");
                // player.astarSystem.Init(filePath);
                // player.astarSystem.NaviTo(player.transform);  //TODO:move start 测试
                
            }
        }
        
    }
}
using gprotocol;

namespace EchoServer.script.logic;

/// <summary>
/// 房间管理器
/// </summary>
public class RoomManager
{
    //最大id（自增唯一id)
    private static int maxId = 0;
    //房间列表
    public static Dictionary<int, Room> rooms = new Dictionary<int, Room>();
    
    //获取房间
    public static Room GetRoom(int roomId)
    {
        if (rooms.ContainsKey(roomId))
        {
            return rooms[roomId];
        }
        return null;
    }
    
    //添加房间
    public static Room AddRoom()
    {
        
        Room room = new Room();
        room.id = maxId;
        rooms.Add(room.id, room);
        maxId++; //从0开始自增
        return room;
    }
    
    //删除房间
    public static bool RemoveRoom(int roomId)
    {
        return rooms.Remove(roomId);
    }
    
    //生成MsgGetRoomList协议
    public static ProtoBuf.IExtensible ToMsg()
    {
        MsgGetRoomList msg = new MsgGetRoomList();
        int count = rooms.Count;
        // msg.rooms = new RoomInfo[count];
        msg.rooms.AddRange(new RoomInfo[count]); //test修改
        //rooms
        int i = 0;
        foreach (Room room in rooms.Values)
        {
            RoomInfo roomInfo = new RoomInfo();
            //赋值
            roomInfo.id = room.id;
            roomInfo.count = room.playerIds.Count;
            roomInfo.status = (int)room.status;
            
            msg.rooms[i] = roomInfo;
            i++;
        }
        return msg;
    }

    public static void Update()
    {
        foreach (Room room in rooms.Values)
        {
            room.Update();
        }
    }
}
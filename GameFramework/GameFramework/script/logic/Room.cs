using gprotocol;

namespace EchoServer.script.logic;

public class Room
{
    //房间id
    public int id = 0;
    //最大玩家数
    public int maxPlayer = 6;
    //玩家列表
    public Dictionary<string, bool> playerIds = new Dictionary<string, bool>();
    //房主id
    public string ownerId = "";
    //状态
    public enum Status
    {
        PRIPARE = 0, //准备中
        FIGHT = 1, //战斗中
    }
    public Status status = Status.PRIPARE;
    
    //添加玩家
    public bool AddPlayer(string username)
    {
        //获取玩家
        Player player = PlayerManager.GetPlayer(username);
        if (player == null){
            Console.WriteLine($"Player {username} not found in room AddPlayer Function");
            return false;
        }
        //房间人数
        if (playerIds.Count >= maxPlayer)
        {
            Console.WriteLine("addPlayer fail, room is full");
            return false;
        }
        //准备状态才能加入
        if (status != Status.PRIPARE)
        {
            Console.WriteLine("addPlayer fail, room status is not PRIPARE");
            return false;
        }
        //已经在房间里
        if (playerIds.ContainsKey(username))
        {
            Console.WriteLine($"Player {username} already exists in room ");
            return false;
        }
        
        //加入房间玩家列表
        playerIds.Add(username, true);
        player.roomId = this.id; //玩家记录房间号
        player.camp = SwitchCamp(); //随机一个阵营
        //设置房主
        if (ownerId == "")
        {
            ownerId = player.id;
        }
        //广播给其他玩家
        Broadcast(ToMsg());
        return true;
    }
    
    //分配阵营
    public int SwitchCamp()
    {
        //计数
        int count1 = 0;
        int count2 = 0;
        foreach (string id in playerIds.Keys)
        {
            Player player = PlayerManager.GetPlayer(id);
            if(player.camp == 1){count1++;}
            if(player.camp == 2){count2++;}
        }
        //选择
        if (count1 <= count2)
        {
            return 1; //红
        }
        else
        {
            return 2; //蓝
        }
    }
    
    //删除玩家
    public bool RemovePlayer(string username)
    {
        //获取玩家（玩家是否在线）
        Player player = PlayerManager.GetPlayer(username);
        if (player == null)
        {
            Console.WriteLine($"Player {username} is null,Remove fail! in room RemovePlayer Function");
            return false;
        }
        //没有在房间里
        if (!playerIds.ContainsKey(username))
        {
            Console.WriteLine($"Player {username} not found in room RemovePlayer Function");
        }
        
        //在房间列表中删除玩家
        playerIds.Remove(username);
        //设置玩家数据
        player.camp = 0; //0 代表无阵营
        player.roomId = -1; //-1代表无房间
        //设置新房主
        if (IsOwner(player))
        {
            ownerId = SwitchOwner();
        }
        //战斗状态退出(remove) 服务器断线处理（因为RemovePlayer会在断线时也会调用）
        if (status == Status.FIGHT)
        {
            player.data.lost++; //不管什么原因，玩家在战斗中退出，视为输
            MsgLeaveBattle msg = new MsgLeaveBattle();
            msg.id = player.id;
            Broadcast(msg);
        }
        
        //房间为空
        if (playerIds.Count == 0)
        {
            RoomManager.RemoveRoom(this.id); 
        }
        //广播
        Broadcast(ToMsg());
        return true;
    }
    //判断房主
    public bool IsOwner(Player player)
    {
        return player.id == ownerId;
    }
    
    //选择新房主
    public string SwitchOwner()
    {
        //选择第一个玩家 TODO:探究dictionary结构排序
        foreach (string id in playerIds.Keys)
        {
            return id;
        }
        //房间没人
        return "";
    }
    
    //广播消息
    public void Broadcast(ProtoBuf.IExtensible msg)
    {
        foreach (string id in playerIds.Keys)
        {
            Player player = PlayerManager.GetPlayer(id);
            player.Send(msg);
        }
    }
    
    //生成MsgGetRoomInfo协议
    public ProtoBuf.IExtensible ToMsg()
    {
        MsgGetRoomInfo msg = new MsgGetRoomInfo();
        int count = playerIds.Count;
        // msg.players = new PlayerInfo[count];
        msg.players.AddRange(new PlayerInfo[count]); //TODO:test
        //players
        int i = 0;
        foreach (string id in playerIds.Keys)
        {
            Player player = PlayerManager.GetPlayer(id);
            PlayerInfo playerInfo = new PlayerInfo();
            //赋值
            playerInfo.id = player.id;
            playerInfo.camp = player.camp;
            playerInfo.win = player.data.win;
            playerInfo.lost = player.data.lost;
            playerInfo.isOwner = 0;
            if (IsOwner(player))
            {
                playerInfo.isOwner = 1;
            }
            msg.players[i] = playerInfo; //填充房间协议的玩家列表
            i++;
        }

        return msg;
    }
    
    //能否开战
    public bool CanStartBattle()
    {
        //已经是战斗状态
        if (status != Status.PRIPARE)
        {
            return false;
        }
        //统计每个阵营玩家数
        int count1 = 0;
        int count2 = 0;
        foreach (string id in playerIds.Keys)
        {
            Player player = PlayerManager.GetPlayer(id);
            if(player.camp == 1){count1++;}
            else if(player.camp == 2){count2++;}
        }
        //每个阵营至少要有一个玩家
        if (count1 < 1 || count2 < 1)
        {
            return false;
        }
        
        //可以开战
        return true;
    }
    
    //出生点位置配置 
    private static float[,,] birthConfig = new float[2, 3, 6]
    {
        {
            {-6f, 1.0f, 1.0f, 0.0f,0.0f,0.0f }, 
            {-6f, 1.0f, 1.0f, 0.0f,0.0f,0.0f  }, 
            {-6f, 1.0f, 1.0f, 0.0f,0.0f,0.0f  }
        },
        {
            {-6f, 1.0f, 100.0f, 0.0f,0.0f,0.0f }, 
            {-6f, 1.0f, 100.0f, 0.0f,0.0f,0.0f  }, 
            {-6f, 1.0f, 100.0f, 0.0f,0.0f,0.0f  }
        }
    };
    
    //初始化位置
    private void SetBirthPos(Player player, int index)
    {
        int camp = player.camp;
        
        player.x = birthConfig[camp-1, index, 0];
        player.y = birthConfig[camp-1, index, 1];
        player.z = birthConfig[camp-1, index, 2];
        player.ex = birthConfig[camp-1, index, 3];
        player.ey = birthConfig[camp-1, index, 4];
        player.ez = birthConfig[camp-1, index, 5];
    }
    
    //重置玩家属性
    private void ResetPlayers()
    {
        //位置和旋转
        int count1 = 0;
        int count2 = 0;
        foreach (string id in playerIds.Keys)
        {
            Player player = PlayerManager.GetPlayer(id);
            if (player.camp == 1)
            {
                SetBirthPos(player, count1++);
            }
            else
            {
                SetBirthPos(player, count2++);
            }
        }
        
        
        //属性
        foreach (string id in playerIds.Keys)
        {
            Player player = PlayerManager.GetPlayer(id);
            player.hp = 100;
            player.att = 100;
            player.def = 80;
        }
    }
    
    //玩家数据转成TankInfo
    public TankInfo PlayerToTankInfo(Player player)
    {
        TankInfo tankInfo = new TankInfo();
        tankInfo.camp = player.camp;
        tankInfo.id = player.id;
        tankInfo.hp = player.hp;
        tankInfo.att = player.att;
        tankInfo.def = player.def;
        
        tankInfo.x = player.x;
        tankInfo.y = player.y;
        tankInfo.z = player.z;
        tankInfo.ex = player.ex;
        tankInfo.ey = player.ey;
        tankInfo.ez = player.ez;

        return tankInfo;
    }
    
    //房间开战
    public bool StartBattle()
    {
        if (!CanStartBattle())
        {
            return false;
        }
        //状态
        status = Status.FIGHT;
        //玩家战斗属性
        ResetPlayers();
        //返回数据
        MsgEnterBattle msg = new MsgEnterBattle();
        msg.mapId = 1;
        msg.tanks.AddRange(new TankInfo[playerIds.Count]);

        int i = 0;
        foreach (string id in playerIds.Keys)
        {
            Player player = PlayerManager.GetPlayer(id);
            msg.tanks[i] = PlayerToTankInfo(player);
            i++;
        }
        Broadcast(msg);
        return true;
    }
    
    //是否死亡
    public bool IsDie(Player player)
    {
        return player.hp <= 0;
    }
    //胜负判定
    public int Judgment()
    {
        //存活人数
        int count1 = 0;
        int count2 = 0;
        foreach (string id in playerIds.Keys)
        {
            Player player = PlayerManager.GetPlayer(id);
            if (!IsDie(player))
            {
                if(player.camp == 1){count1++;}
                if(player.camp == 2){count2++;}
            }
        }
        //判段
        if (count1 <= 0)
        {
            return 2; //阵容2胜利
        }else if (count2 <= 0)
        {
            return 1; //阵容1胜利
        }
        return 0; //战斗中...
    }
    
    //房间的ticker
    //上一次判定结果的时间
    private long lastJudgeTime = NetManager.GetTimeStamp();
    
    //定时更新
    public void Update()
    {
        //状态判断
        if (status != Status.FIGHT)
        {
            return;
        }
        //时间判定
        if (NetManager.GetTimeStamp() - lastJudgeTime < 3f)
        {
            return;
        }
        lastJudgeTime = NetManager.GetTimeStamp(); //超过3秒刷新时间
        
        //胜负判定
        int winCamp = Judgment();
        //尚未分出胜负
        if (winCamp == 0)
        {
            return;
        }
        Console.WriteLine("赢家  "+winCamp);
        //某一方胜利，结束战斗
        status = Status.PRIPARE;
        //统计信息 TODO:数据库持久化
        foreach (string id in playerIds.Keys)
        {
            Player player = PlayerManager.GetPlayer(id);
            if (player.camp == winCamp)
            {
                player.data.win++;
            }
            else
            {
                player.data.lost++;
            }
        }
        
        //最后发送结果
        MsgBattleResult msg = new MsgBattleResult();
        msg.winCamp = winCamp;
        Broadcast(msg);
        
    }
}
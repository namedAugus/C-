using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
//移动协议
public class MsgMove : MsgBase
{
    public int x = 0;
    public int y = 0;
    public int z = 0;
    public TransformComponent transform;
    public float ox = 0;
    public float oy = 0;
    public float tx = 0;
    public float ty = 0;

    public MsgMove()
    {
        protoName = "MsgMove"; //规定协议名和类名一致，方便后续编解码
    }
}

//攻击协议
public class MsgAttack : MsgBase
{
    public string desc = "127.0.0.1:6543";

    public MsgAttack()
    {
        protoName = "MsgAttack"; //规定协议名和类名一致，方便后续编解码
    }
}

//坦克信息
[System.Serializable]
public class TankInfo
{
    public string id = ""; //玩家id
    public int camp = 0; //阵营
    public int hp = 0; //生命值
    public int att = 0; //攻击力
    public int def = 0; //防御值

    public float x = 0; //位置
    public float y = 0;
    public float z = 0;
    public float ex = 0; //旋转
    public float ey = 0;
    public float ez = 0;
}

//进入战场协议(服务端推送)
public class MsgEnterBattle : MsgBase
{
    public MsgEnterBattle() { protoName = "MsgEnterBattle"; }
    //服务端返回
    public TankInfo[] tanks;
    public int mapId = 1; //地图id  目前只有一张，暂无实际意义
    public int result = 0;
}
//战斗结果协议(服务端推送)
public class MsgBattleResult : MsgBase
{
    public MsgBattleResult() { protoName = "MsgBattleResult"; }
    //服务端回
    public int winCamp = 0; //获胜的阵营 0 无； 1 红； 2 蓝
}

//退出战斗协议(主动、或者掉线）
public class MsgLeaveBattle : MsgBase
{
    public MsgLeaveBattle() { protoName = "MsgLeaveBattle"; }
    //服务端回
    public string id = ""; //掉线的玩家id（or username)
}
*/

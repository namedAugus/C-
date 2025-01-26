using UnityEngine;
using System.Collections.Generic;
using System;
using gprotocol;
/// <summary>
/// 战斗管理器
/// </summary>
public class BattleManager
{
    //战场中的坦克
    public static Dictionary<string, BaseTank> tanks = new Dictionary<string, BaseTank>();

    //初始化
    public static void Init()
    {
        //添加监听
        NetManager.AddMsgListener("MsgEnterBattle", OnMsgEnterBattle);
        NetManager.AddMsgListener("MsgBattleResult", OnMsgBattleResult);
        NetManager.AddMsgListener("MsgLeaveBattle", OnMsgLeaveBattle);
        //同步协议监听
        NetManager.AddMsgListener("MsgSyncTank", OnMsgSyncTank);
        NetManager.AddMsgListener("MsgFire", OnMsgFire);
        NetManager.AddMsgListener("MsgHit", OnMsgHit);
    }

    //收到击中协议
    private static void OnMsgHit(ProtoBuf.IExtensible msgBase)
    {
        MsgHit msg = (MsgHit)msgBase;
        if (msg.id == GameMain.id)
        {
            return;
        }
        //查找坦克
        SyncTank tank = (SyncTank)GetTank(msg.targetId);
        if (tank == null)
        {
            return;
        }
        //开火
        tank.Attacked(msg.damage); //传入伤害
    }

    //收到开火协议
    private static void OnMsgFire(ProtoBuf.IExtensible msgBase)
    {
        MsgFire msg = (MsgFire)msgBase;
        if (msg.id == GameMain.id)
        {
            return;
        }
        //查找坦克
        SyncTank tank = (SyncTank)GetTank(msg.id);
        if (tank == null)
        {
            return;
        }
        //开火
        tank.SyncFire(msg);
    }

    //收到位置同步协议
    private static void OnMsgSyncTank(ProtoBuf.IExtensible msgBase)
    {
        MsgSyncTank msg = (MsgSyncTank)msgBase;
        //不同步自己
        if (msg.id == GameMain.id)
        {
            return;
        }
        //查找坦克
        SyncTank tank = (SyncTank)GetTank(msg.id);
        if (tank == null)
        {
            return;
        }
        //移动同步
        tank.SyncPos(msg);
    }

    //收到进入战斗协议
    private static void OnMsgEnterBattle(ProtoBuf.IExtensible msgBase)
    {
        MsgEnterBattle msg = (MsgEnterBattle)msgBase;
        EnterBattle(msg);
    }
    //开始战斗
    public static void EnterBattle(MsgEnterBattle msg)
    {
        //重置战场
        BattleManager.Reset();
        //关闭界面（如房间)
        PanelManager.Close("RoomPanel"); //可以放到房间系统的监听中
        PanelManager.Close("ResultPanel");
        //产生坦克
        for (int i = 0; i < msg.tanks.Count; i++)
        {
            GenerateTank(msg.tanks[i]);
        }
    }

    //产生坦克
    public static void GenerateTank(TankInfo tankInfo)
    {

        //GameObject
        string objName = "Tank_" + tankInfo.id;
        GameObject tankObj = new GameObject(objName);
        //AddComponent
        BaseTank tank = null;
        if (tankInfo.id == GameMain.id)
        {
            tank = tankObj.AddComponent<CtrlTank>();
        }
        else
        {
            tank = tankObj.AddComponent<SyncTank>();
        }
        //Add Camera
        if (tankInfo.id == GameMain.id)
        {
            CameraFollow cf = tankObj.AddComponent<CameraFollow>();
        }
        //属性
        tank.camp = tankInfo.camp;
        tank.id = tankInfo.id;
        tank.hp = tankInfo.hp;
        tank.att = tankInfo.att;
        tank.def = tankInfo.def;
        //pos ration
        Vector3 pos = new Vector3(tankInfo.x, tankInfo.y, tankInfo.z);
        Vector3 rot = new Vector3(tankInfo.ex, tankInfo.ey, tankInfo.ez);
        tank.transform.position = pos;
        tank.transform.eulerAngles = rot;
        Debug.Log("玩家:" + tankInfo.id + "位置:" + tankInfo.x + "==" + tankInfo.y + "==" + tankInfo.z);
        Debug.Log("玩家:" + tankInfo.id + "旋转:" + tankInfo.ex + "==" + tankInfo.ey + "==" + tankInfo.ez);
        //init
        if (tankInfo.camp == 1)
        {
            tank.Init("TankPrefab/tankPrefab"); //阵营
        }
        else
        {
            tank.Init("TankPrefab/tankPrefab");
        }

        //最后记得将tank加入管理器的tanks列表
        AddTank(tankInfo.id, tank);
    }

    //收到战斗结束协议
    private static void OnMsgBattleResult(ProtoBuf.IExtensible msgBase)
    {
        MsgBattleResult msg = (MsgBattleResult)msgBase;
        //判断显示胜利还是失败
        bool isWin = false;
        BaseTank tank = GetCtrlTank();
        Debug.Log("Final ====" + tank.id);
        Debug.Log("胜利阵营：" + msg.winCamp);
        Debug.Log("我的阵营：" + msg.winCamp);
        if (tank != null && tank.camp == msg.winCamp)
        {
            isWin = true;

        }
        //显示界面
        PanelManager.Open<ResultPanel>(isWin);
    }

    //收到玩家退出游戏协议
    private static void OnMsgLeaveBattle(ProtoBuf.IExtensible msgBase)
    {
        MsgLeaveBattle msg = new MsgLeaveBattle();
        //查找坦克
        BaseTank tank = GetTank(msg.id);
        if (tank == null)
        {
            return;
        }
        //删除坦克
        RemoveTank(msg.id);
        MonoBehaviour.Destroy(tank.gameObject);
    }

    //添加坦克
    public static void AddTank(string id, BaseTank tank)
    {
        tanks[id] = tank;
    }
    //删除坦克
    public static void RemoveTank(string id)
    {
        tanks.Remove(id);
    }
    //获取坦克
    public static BaseTank GetTank(string id)
    {
        if (tanks.ContainsKey(id))
        {
            return tanks[id];
        }
        return null;
    }
    //获取玩家控制的坦克
    public static BaseTank GetCtrlTank()
    {
        return GetTank(GameMain.id);
    }

    //重置战场
    public static void Reset()
    {

        //清除场景
        foreach (BaseTank tank in tanks.Values)
        {
            if (tank != null)
            {
                MonoBehaviour.Destroy(tank.gameObject);
            }

        }
        //清除列表
        tanks.Clear();
    }


}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMain : MonoBehaviour
{
    public static string id = "";
    public string ipAdr = "192.168.4.123";
    public int port = 8888;

    private void Awake()
    {
        //网络监听
        NetManager.AddEventListener(NetEvent.Close, this.OnConnectClose);
        NetManager.AddEventListener(NetEvent.ConnectSucc, this.OnConnecSucc);
        NetManager.AddEventListener(NetEvent.ConnectFail, this.OnConnecFail);

        NetManager.Connect(ipAdr, port);
        //TODO: 开始转圈圈，提示连接中

        NetManager.AddMsgListener("MsgKick", OnMsgKick);

    }
    void Start()
    {

        //初始化面板管理器
        PanelManager.Init();
        //初始化战斗管理器
        BattleManager.Init();

        //打开登录面板
        PanelManager.Open<LoginPanel>();

    }

    private void OnConnecFail(string err)
    {
        PanelManager.Open<TipPanel>("网络连接失败，请重试！");
    }

    private void OnConnecSucc(string err)
    {

    }

    private void OnMsgKick(ProtoBuf.IExtensible msgBase)
    {
        PanelManager.Open<TipPanel>("您已被踢下线！");

    }

    private void OnConnectClose(string err)
    {
        Debug.Log("断开连接...");
        PanelManager.Open<TipPanel>("网络连接意外断开...");
    }

    // Update is called once per frame
    void Update()
    {
        NetManager.Update();
    }
}

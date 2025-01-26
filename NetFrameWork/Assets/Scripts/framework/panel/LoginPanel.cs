using gprotocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginPanel : BasePanel
{
    //账号输入框
    private InputField idInput;
    //密码输入框
    private InputField pwInput;
    //登录btn
    private Button loginBtn;
    //注册btn
    private Button regBtn;


    //初始化
    public override void OnInit()
    {
        skinPath = "LoginPanel";
        layer = PanelManager.Layer.Panel;
    }
    //显示
    public override void OnShow(params object[] para)
    {
        //寻找组件
        idInput = skin.transform.Find("IdInput").GetComponent<InputField>();
        pwInput = skin.transform.Find("PwInput").GetComponent<InputField>();
        loginBtn = skin.transform.Find("LoginBtn").GetComponent<Button>();
        regBtn = skin.transform.Find("RegisterBtn").GetComponent<Button>();
        //监听
        loginBtn.onClick.AddListener(OnLoginClick);
        regBtn.onClick.AddListener(OnRegClick);

        //网络协议监听
        NetManager.AddEventListener(NetEvent.ConnectSucc, OnConnectSucc);
        NetManager.AddEventListener(NetEvent.ConnectFail, OnConnectFail);
        NetManager.AddMsgListener("MsgLogin", OnMsgLogin);
    }



    private void OnConnectFail(string err)
    {
        PanelManager.Open<TipPanel>(err);
    }

    private void OnConnectSucc(string err)
    {
        Debug.Log("连接成功");
    }

    private void OnRegClick()
    {
        PanelManager.Open<RegisterPanel>();
    }

    private void OnLoginClick()
    {
        if (idInput.text == "" || pwInput.text == "")
        {
            PanelManager.Open<TipPanel>("用户名和密码不能为空");
            return;
        }
        //发送
        MsgLogin msgLogin = new MsgLogin();
        msgLogin.username = idInput.text;
        msgLogin.password = pwInput.text;
        NetManager.Send(msgLogin);
    }
    private void OnMsgLogin(ProtoBuf.IExtensible msgBase)
    {
        MsgLogin msg = (MsgLogin)msgBase;
        if (msg.result == 1)
        {
            Debug.Log("登录成功");
            //进入游戏
            /*
            //添加坦克  开战时再添加就好
            GameObject tankObj = new GameObject(" myTank");
            CtrlTank ctrlTank = tankObj.AddComponent<CtrlTank>();
            ctrlTank.Init("TankPrefab/tankPrefab");
            //设置相机
            tankObj.AddComponent<CameraFollow>(); */
            //设置id（username)
            GameMain.id = msg.username;
            Debug.Log("start ====" + GameMain.id);

            //打开房间列表界面
            PanelManager.Open<RoomListPanel>();
            //关闭界面
            Close();
        }
        else
        {
            PanelManager.Open<TipPanel>("登录失败");
        }
    }

    //关闭
    public override void OnClose()
    {
        NetManager.RemoveMsgListener("MsgLogin", OnMsgLogin);
    }
}

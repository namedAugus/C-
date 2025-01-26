using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using gprotocol;

public class RoomPanel : BasePanel
{
    //开战按钮
    private Button startButton;
    //退出btn
    private Button exitButton;
    //列表容器
    private Transform content;
    //玩家信息项
    private GameObject playerObj;

    //初始化
    public override void OnInit()
    {
        skinPath = "RoomPanel";
        layer = PanelManager.Layer.Panel;
    }

    //显示
    public override void OnShow(params object[] para)
    {
        //寻找组件
        startButton = skin.transform.Find("StartButton").GetComponent<Button>();
        exitButton = skin.transform.Find("ExitButton").GetComponent<Button>();
        content = skin.transform.Find("Scroll View/Viewport/Content");
        playerObj = skin.transform.Find("Player").gameObject;
        //不激活玩家
        playerObj.SetActive(false);
        //监听按钮
        startButton.onClick.AddListener(OnStartClick);
        exitButton.onClick.AddListener(OnExitClick);

        //协议监听
        NetManager.AddMsgListener("MsgGetRoomInfo", OnMsgGetRoomInfo);
        NetManager.AddMsgListener("MsgLeaveRoom", OnMsgLeaveRoom);
        NetManager.AddMsgListener("MsgStartBattle", OnMsgStartBattle);

        //发送查询
        MsgGetRoomInfo msg = new MsgGetRoomInfo();
        NetManager.Send(msg);
    }


    //收到玩家列表协议
    private void OnMsgGetRoomInfo(ProtoBuf.IExtensible msgBase)
    {
        MsgGetRoomInfo msg = (MsgGetRoomInfo)msgBase;
        //清除玩家列表
        for (int i = content.childCount - 1; i >= 0; i--)
        {
            GameObject o = content.GetChild(i).gameObject;
            Destroy(o);
        }
        //重新生成列表
        if (msg.players == null)
        {
            return;
        }
        for (int i = 0; i < msg.players.Count; i++)
        {
            GeneratePlayerInfo(msg.players[i]);
        }
    }

    //创建一个玩家项
    public void GeneratePlayerInfo(PlayerInfo playerInfo)
    {
        //创建物体
        GameObject o = Instantiate(playerObj);
        o.transform.SetParent(content);
        o.SetActive(true);
        //o.transform.localScale = Vector3.zero;
        //获取组件
        Transform trans = o.transform;
        Text idText = trans.Find("IdText").GetComponent<Text>();
        Text campText = trans.Find("CampText").GetComponent<Text>();
        Text scoreText = trans.Find("ScoreText").GetComponent<Text>();
        //填充信息
        idText.text = playerInfo.id;
        if (playerInfo.camp == 1)
        {
            campText.text = " 红 ";
        }
        else
        {
            campText.text = " 蓝 ";
        }
        if (playerInfo.isOwner == 1)
        {
            campText.text = campText.text + "!";
        }
        scoreText.text = playerInfo.win + " 胜 " + playerInfo.lost + " 负 ";
    }

    //点击退出
    private void OnExitClick()
    {
        MsgLeaveRoom msg = new MsgLeaveRoom();
        NetManager.Send(msg);
    }
    //退出房间协议
    private void OnMsgLeaveRoom(ProtoBuf.IExtensible msgBase)
    {
        MsgLeaveRoom msg = (MsgLeaveRoom)msgBase;
        //成功退出房间
        if (msg.result == 1)
        {
            PanelManager.Open<TipPanel>("退出房间");
            PanelManager.Open<RoomListPanel>();
        }
        else
        {
            PanelManager.Open<TipPanel>("退出房间失败");
        }
    }

    //点击开战按钮
    private void OnStartClick()
    {
        MsgStartBattle msg = new MsgStartBattle();
        NetManager.Send(msg);
    }
    //开战返回
    private void OnMsgStartBattle(ProtoBuf.IExtensible msgBase)
    {
        MsgStartBattle msg = (MsgStartBattle)msgBase;
        //开战
        if (msg.result == 1)
        {
            //关闭界面，进入战场
            Close();
        }
        else
        {
            PanelManager.Open<TipPanel>("开战失败！两队都需要至少一名玩家，只有队长可以开始战斗！");
        }
    }

    //关闭
    public override void OnClose()
    {
        //移除协议监听
        NetManager.RemoveMsgListener("MsgGetRoomInfo", OnMsgGetRoomInfo);
        NetManager.RemoveMsgListener("MsgLeaveRoom", OnMsgLeaveRoom);
        NetManager.RemoveMsgListener("MsgStartBattle", OnMsgStartBattle);
    }
}

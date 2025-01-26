using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using gprotocol;

public class RoomListPanel : BasePanel
{
    //账号文本
    private Text idText;
    //战绩文本
    private Text scoreText;
    //创建房间按钮
    private Button createButton;
    //刷新列表按钮
    private Button refreshButton;
    //列表容器
    private Transform content;
    //列表项（房间)
    private GameObject roomObj;

    //初始化
    public override void OnInit()
    {
        skinPath = "RoomListPanel";
        layer = PanelManager.Layer.Panel;
    }

    //显示
    public override void OnShow(params object[] para)
    {
        //寻找组件
        idText = skin.transform.Find("InfoPanel/IdText").GetComponent<Text>();
        scoreText = skin.transform.Find("InfoPanel/ScoreText").GetComponent<Text>();
        createButton = skin.transform.Find("CtrlPanel/CreateButton").GetComponent<Button>();
        refreshButton = skin.transform.Find("CtrlPanel/RefreshButton").GetComponent<Button>();
        content = skin.transform.Find("ListPanel/Scroll View/Viewport/Content");
        roomObj = skin.transform.Find("Room").gameObject;
        //按钮事件
        createButton.onClick.AddListener(OnCreateClick);
        refreshButton.onClick.AddListener(OnRefreshClick);
        //不激活房间
        roomObj.SetActive(false);
        //显示id
        idText.text = GameMain.id;

        //协议监听
        NetManager.AddMsgListener("MsgGetAchieve", OnMsgGetAchieve);
        NetManager.AddMsgListener("MsgGetRoomList", OnMsgGetRoomList);
        NetManager.AddMsgListener("MsgCreateRoom", OnMsgCreateRoom);
        NetManager.AddMsgListener("MsgEnterRoom", OnMsgEnterRoom);

        //发送查询
        MsgGetAchieve msgGetAchieve = new MsgGetAchieve();
        NetManager.Send(msgGetAchieve);
        MsgGetRoomList msgRoomList = new MsgGetRoomList();
        NetManager.Send(msgRoomList);
    }

    //点击创建房间
    private void OnCreateClick()
    {
        MsgCreateRoom msg = new MsgCreateRoom();
        NetManager.Send(msg);
    }

    //创建房间回调
    private void OnMsgCreateRoom(ProtoBuf.IExtensible msgBase)
    {
        MsgCreateRoom msg = (MsgCreateRoom)msgBase;
        //成功创建房间
        if (msg.result == 1)
        {
            PanelManager.Open<TipPanel>("创建成功");
            PanelManager.Open<RoomPanel>();
            Close();
        }
        else
        {
            PanelManager.Open<TipPanel>("创建房间失败");
        }
    }

    //进入房间回调
    private void OnMsgEnterRoom(ProtoBuf.IExtensible msgBase)
    {
        MsgEnterRoom msg = (MsgEnterRoom)msgBase;
        if (msg.result == 1)
        {
            PanelManager.Open<RoomPanel>();
            Close();
        }
        else
        {
            PanelManager.Open<TipPanel>("进入房间失败");
        }
    }

    //房间列表回调
    private void OnMsgGetRoomList(ProtoBuf.IExtensible msgBase)
    {
        MsgGetRoomList msg = (MsgGetRoomList)msgBase;

        if (content != null && content.childCount > 0) //新加判断 
                                                       //清除房间列表
            for (int i = content.childCount - 1; i >= 0; i--)
            {
                GameObject o = content.GetChild(i).gameObject;
                DestroyImmediate(o);
            }
        //如果没有房间，不需要进一步处理
        if (msg.rooms == null)
        {
            return;
        }
        for (int i = 0; i < msg.rooms.Count; i++)
        {
            GenerateRoom(msg.rooms[i]);
        }
    }

    //创建一个房间项
    public void GenerateRoom(RoomInfo roomInfo)
    {
        //创建物体
        GameObject o = Instantiate(roomObj);
        o.transform.SetParent(content);
        o.SetActive(true);
        //o.transform.localScale = Vector3.one;

        //获取组件
        Transform trans = o.transform;
        Text idText = trans.Find("IdText").GetComponent<Text>();
        Text countText = trans.Find("CountText").GetComponent<Text>();
        Text statusText = trans.Find("StatusText").GetComponent<Text>();
        Button btn = trans.Find("JoinButton").GetComponent<Button>();
        //填充信息
        idText.text = roomInfo.id.ToString();
        countText.text = roomInfo.count.ToString();
        if (roomInfo.status == 0)
        {
            statusText.text = "准备中";
        }
        else
        {
            statusText.text = "战斗中";
        }
        //按钮事件
        btn.name = idText.text; //房间号
        btn.onClick.AddListener(delegate ()
        {
            OnJoinClick(btn.name);
        });
    }

    //加入房间点击
    public void OnJoinClick(string idString)
    {
        MsgEnterRoom msg = new MsgEnterRoom();
        msg.id = int.Parse(idString);
        NetManager.Send(msg);
    }

    //战绩查询回调
    private void OnMsgGetAchieve(ProtoBuf.IExtensible msgBase)
    {
        MsgGetAchieve msg = (MsgGetAchieve)msgBase;
        scoreText.text = msg.win + " 胜 " + msg.lost + " 负";
    }

    //刷新房间列表点击
    private void OnRefreshClick()
    {
        MsgGetRoomList msg = new MsgGetRoomList();
        NetManager.Send(msg);
    }





    //关闭
    public override void OnClose()
    {
        //移除协议监听
        NetManager.RemoveMsgListener("MsgGetAchieve", OnMsgGetAchieve);
        NetManager.RemoveMsgListener("MsgGetRoomList", OnMsgGetRoomList);
        NetManager.RemoveMsgListener("MsgCreateRoom", OnMsgCreateRoom);
        NetManager.RemoveMsgListener("MsgEnterRoom", OnMsgEnterRoom);
    }
}

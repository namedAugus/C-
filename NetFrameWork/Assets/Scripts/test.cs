using gprotocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class test : MonoBehaviour
{
    public InputField usernameInput;
    public InputField passwordInput;
    public InputField textInput;
    // Start is called before the first frame update
    void Start()
    {
        NetManager.AddEventListener(NetEvent.ConnectSucc, OnConnectSucc);
        NetManager.AddEventListener(NetEvent.ConnectFail, OnConnectFail);
        NetManager.AddEventListener(NetEvent.Close, OnConnectClose);


        //NetManager.AddMsgListener("TransformComponent", OnTransform);
        NetManager.AddMsgListener("MsgMove", OnMsgMove);
        NetManager.AddMsgListener("MsgRegister", OnMsgRegister);
        NetManager.AddMsgListener("MsgLogin", OnMsgLogin);
        NetManager.AddMsgListener("MsgKick", OnMsgKick);
        NetManager.AddMsgListener("MsgGetText", OnMsgGetText);
        NetManager.AddMsgListener("MsgSaveText", OnMsgSaveText);

        /*
        MsgMove msgMove = new MsgMove();
        msgMove.x = 100;
        msgMove.y = -20;

        byte[] bytes = ProtoBuf.IExtensible.Encode(msgMove);
        string s = System.Text.Encoding.UTF8.GetString(bytes);
        Debug.Log(s);

        //JsonUtility.FromJsonOverwrite(s, msgMove);
        string x = "{\"protoName\":\"MsgMove\",\"x\":100,\"y\":-20,\"z\":0}";
        byte[] bytez = System.Text.Encoding.UTF8.GetBytes(x);
        MsgMove m = (MsgMove)ProtoBuf.IExtensible.Decode("MsgMove", bytez, 0, bytes.Length);
        Debug.Log(m.x + "=" + m.y + "=" + m.z);
        */
        //tank
        //GameObject skinRes = ResManager.LoadPrefab("TankPrefab/tankPrefab");
        //GameObject skin = (GameObject)Instantiate(skinRes);

        //protoBuf
        ReqUserLogin req = new ReqUserLogin();
        req.uname = "hks";
        req.upwd = "1";
        byte[] b = EncodeProto(req);
        Debug.Log(b);
        Debug.Log(System.BitConverter.ToString(b));

        Debug.Log(req.ToString());
        //解码
        ProtoBuf.IExtensible m = DecodeProto("gprotocol.ReqUserLogin", b, 0, b.Length);
        ReqUserLogin r = (ReqUserLogin)m;
        Debug.Log(r.uname + "=" + r.upwd);
        MsgCreateRoom test = new MsgCreateRoom();
        byte[] t1 = ProtoSerializer.Encode(test);
        Debug.Log(System.BitConverter.ToString(t1));
        Debug.Log(EncodeProtoName(req));

        string[] allName = req.ToString().Split('.');
        string protoName = allName[allName.Length - 1];
        Debug.Log(protoName);
    }

    //编码proto
    public static byte[] EncodeProto(ProtoBuf.IExtensible msgBase)
    {
        //将protobuf对象序列化为Byte数组
        using (var memory = new System.IO.MemoryStream())
        {
            ProtoBuf.Serializer.Serialize(memory, msgBase);
            return memory.ToArray();
        }
    }
    public static string EncodeProtoName(ProtoBuf.IExtensible msgBase)
    {
        return msgBase.ToString();
    }

    public static ProtoBuf.IExtensible DecodeProto(string protoName, byte[] bytes, int offset, int count)
    {
        using (var memory = new System.IO.MemoryStream(bytes, offset, count))
        {
            System.Type t = System.Type.GetType(protoName);
            return (ProtoBuf.IExtensible)ProtoBuf.Serializer.NonGeneric.Deserialize(t, memory);
        }
    }
    //降龙寻路算法移动测试
    /*
    private void OnTransform(ProtoBuf.IExtensible msgBase)
    {
        TransformComponent transform = (TransformComponent)msgBase;
        Debug.Log("transform: " + transform.ox + "=" + transform.oy);
    }
    */

    //发送保存协议
    public void OnSaveClick()
    {
        MsgSaveText msg = new MsgSaveText();
        msg.text = textInput.text;
        Debug.Log(textInput.text);
        NetManager.Send(msg);
    }
    private void OnMsgSaveText(ProtoBuf.IExtensible msgBase)
    {
        MsgSaveText msg = (MsgSaveText)msgBase;
        if (msg.result == 1)
        {
            Debug.Log("保存成功！");
        }
        else
        {
            Debug.Log("保存失败！");
        }
    }

    //收到记事本协议
    private void OnMsgGetText(ProtoBuf.IExtensible msgBase)
    {
        MsgGetText msg = (MsgGetText)msgBase;
        textInput.text = msg.text;
        Debug.Log(msg.text + "   f");
        Debug.Log(textInput.text + "  e");
    }

    private void OnMsgKick(ProtoBuf.IExtensible msgBase)
    {
        Debug.Log("被踢下线!");
    }
    //发送注册协议
    public void OnMsgLoginClick()
    {
        MsgLogin msg = new MsgLogin();
        msg.username = usernameInput.text;
        msg.password = passwordInput.text;
        NetManager.Send(msg);
    }
    private void OnMsgLogin(ProtoBuf.IExtensible msgBase)
    {
        MsgLogin msg = (MsgLogin)msgBase;
        if (msg.result == 1)
        {
            Debug.Log("登录成功!");
            //请求记事本文本
            MsgGetText msgGetText = new MsgGetText();
            NetManager.Send(msgGetText);
        }
        else
        {
            Debug.Log("登录失败！");
        }
    }

    //发送注册协议
    public void OnRegisterClick()
    {
        MsgRegister msg = new MsgRegister();
        msg.username = usernameInput.text;
        msg.password = passwordInput.text;
        NetManager.Send(msg);
    }
    private void OnMsgRegister(ProtoBuf.IExtensible msgBase)
    {
        MsgRegister msg = (MsgRegister)msgBase;
        if (msg.result == 1)
        {
            Debug.Log("注册成功！");
        }
        else
        {
            Debug.Log("注册失败！");
        }
    }


    private void OnMsgMove(ProtoBuf.IExtensible msgBase)
    {
        MsgMove msg = (MsgMove)msgBase;
        //消息处理
        Debug.Log(msg.x + "," + msg.y + "," + msg.z);

    }

    private void OnConnectSucc(string err)
    {
        Debug.Log("连接成功！");
        //TODO: 进入游戏
    }

    private void OnConnectFail(string err)
    {
        Debug.Log("连接失败！");
        //TODO: 弹框出现连接失败，请重试！
    }

    private void OnConnectClose(string err)
    {
        Debug.Log("连接关闭！");
        //TODO: 弹出提示框（网络断开）
        //TODO: 弹出按钮（重新连接）
    }

    public void OnConnectClick()
    {
        NetManager.Connect("127.0.0.1", 8888);
        //TODO: 开始转圈圈，提示连接中

    }
    public void OnCloseClick()
    {
        NetManager.Close();

    }

    public void OnMoveClick()
    {
        MsgMove msg = new MsgMove();
        msg.x = 120;
        msg.y = 123;
        msg.z = -6;


        //msg.transform = new TransformComponent();
        msg.ox = 200;
        msg.oy = 300;
        msg.tx = 350;
        msg.ty = 450;
        Debug.Log(msg.ox + msg.oy + msg.tx + msg.ty);

        NetManager.Send(msg);

    }
    // Update is called once per frame
    void Update()
    {
        NetManager.Update();
    }
}

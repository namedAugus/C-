using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System;
using System.Linq;
using UnityEngine;

public enum NetEvent
{
    ConnectSucc = 1,
    ConnectFail = 2,
    Close = 3
}

public static class NetManager
{
    //定义套接字
    static Socket socket;
    //接收缓冲区
    static ByteArray readBuff;
    //写入队列
    static Queue<ByteArray> writeQueue;
    //网络事件委托类型
    public delegate void EventListener(String err);
    //网络事件监听列表
    private static Dictionary<NetEvent, EventListener> eventListeners = new Dictionary<NetEvent, EventListener>();

    //消息列表
    static List<ProtoBuf.IExtensible> msgList = new List<ProtoBuf.IExtensible>();
    //消息列表长度
    static int msgCount = 0;
    //每一次执行Update处理的消息量
    readonly static int MAX_MESSAGE_FIRE = 10;

    //是否启用心跳
    public static bool isUsePing = true;
    //心跳间隔时间
    public static int pingInterval = 30;
    //上一次发送ping的时间
    static float lastPingTime = 0;
    //上一次收到pong的时间
    static float lastPongTime = 0;

    //添加事件监听器
    public static void AddEventListener(NetEvent netEvent, EventListener listener)
    {

        if (eventListeners.ContainsKey(netEvent))
        { //添加事件
            eventListeners[netEvent] += listener;
        }
        else
        { //新增事件
            eventListeners[netEvent] = listener;
        }
    }

    //删除事件监听
    public static void RemoveEventListener(NetEvent netEvent, EventListener listener)
    {
        if (eventListeners.ContainsKey(netEvent))
        {
            eventListeners[netEvent] -= listener;
            //删除
            if (eventListeners[netEvent] == null)
            {
                eventListeners.Remove(netEvent);
            }
        }
    }

    //分发事件
    private static void FireEvent(NetEvent netEvent, string err)
    {
        if (eventListeners.ContainsKey(netEvent))
        {
            eventListeners[netEvent](err);
        }
    }

    //是否正在连接
    static bool isConnecting = false;
    //是否正在关闭
    static bool isClosing = false;
    //连接
    public static void Connect(string ip, int port)
    {
        //状态判断
        if (socket != null && socket.Connected)
        {
            Debug.Log("Connect fail, already connected!");
            return;
        }
        if (isConnecting)
        {
            Debug.Log("State isConnecting");
            return;
        }
        //初始化成员
        InitState();
        //参数设置
        socket.NoDelay = true;
        //Connect
        isConnecting = true;
        socket.BeginConnect(ip, port, ConnectCallBack, socket);
    }

    //初始化状态
    private static void InitState()
    {
        //Socket
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //接收缓冲区
        readBuff = new ByteArray();
        //写入队列
        writeQueue = new Queue<ByteArray>();
        //是否正在连接标记
        isConnecting = false;
        //是否正在关闭标记
        isClosing = false;

        //初始化消息列表
        msgList = new List<ProtoBuf.IExtensible>();
        //初始化消息列表长度
        msgCount = 0;

        //初始化上一次ping的时间
        lastPingTime = Time.time;
        //上一次收到pong的时间
        lastPongTime = Time.time;

        //监听Pong协议
        if (!msgListeners.ContainsKey("MsgPong"))
        {
            AddMsgListener("MsgPong", OnMsgPong);
        }
    }

    //Connect回调
    private static void ConnectCallBack(IAsyncResult ar)
    {
        try
        {
            Socket socket = (Socket)ar.AsyncState;
            socket.EndConnect(ar);
            Debug.Log("Socket Connect Success: " + socket.LocalEndPoint + " to " + socket.RemoteEndPoint);
            FireEvent(NetEvent.ConnectSucc, "");
            isConnecting = false;

            //开始接收数据
            socket.BeginReceive(readBuff.bytes, readBuff.writeIdx, readBuff.remain, 0, ReceiveCallback, socket);
        }
        catch (SocketException e)
        {
            Debug.Log("Socket Connect Fail " + e.ToString());
            FireEvent(NetEvent.ConnectFail, e.ToString());
            isConnecting = false;
        }
    }

    //Receive回调
    public static void ReceiveCallback(IAsyncResult ar)
    {
        try
        {
            Socket socket = (Socket)ar.AsyncState;
            //获取接收数据长度
            int count = socket.EndReceive(ar);
            if (count == 0)
            {
                Close();
                return;
            }
            readBuff.writeIdx += count;
            //处理二进制消息
            OnReceiveData();
            //继续接收数据
            if (readBuff.remain < 8)
            {
                readBuff.MoveBytes();
                readBuff.ReSize(readBuff.length * 2);
            }
            socket.BeginReceive(readBuff.bytes, readBuff.writeIdx, readBuff.remain, 0, ReceiveCallback, socket);
        }
        catch (SocketException e)
        {
            Debug.Log("Socket Receive fail" + e.ToString());
        }
    }

    //数据处理
    public static void OnReceiveData()
    {
        //消息长度
        if (readBuff.length <= 2)
        {
            return;
        }
        //获取消息体长度
        int readIdx = readBuff.readIdx;
        byte[] bytes = readBuff.bytes;
        Int16 bodyLength = (Int16)((bytes[readIdx + 1] << 8) | bytes[readIdx]);
        if (readBuff.length < bodyLength + 2)
        {
            return;
        }
        readBuff.readIdx += 2;


        //解析协议名
        int nameCount = 0;
        string protoName = ProtoSerializer.DecodeName(readBuff.bytes, readBuff.readIdx, out nameCount);
        Debug.Log("接收到协议：" + protoName);
        if (protoName == "")
        {
            Debug.Log("OnReceiveData ProtoSerializer.DecodeName fail");
            return;
        }
        readBuff.readIdx += nameCount;
        //解析协议体
        int bodyCount = bodyLength - nameCount;
        ProtoBuf.IExtensible msgBase = ProtoSerializer.Decode(protoName, readBuff.bytes, readBuff.readIdx, bodyCount);
        readBuff.readIdx += bodyCount;
        readBuff.CheckAndMoveBytes();

        //添加到消息队列
        lock (msgList)
        {
            msgList.Add(msgBase);
        }
        msgCount++;

        //继续读取消息
        if (readBuff.length > 2)
        {
            OnReceiveData();
        }

    }



    //关闭连接
    public static void Close()
    {
        //状态判断
        if (socket == null || !socket.Connected)
        {
            return;
        }
        if (isConnecting)
        {
            return;
        }
        //还有数据在发送
        if (writeQueue.Count > 0)
        {
            isClosing = true;
        }
        else
        {
            //没有数据在发送
            socket.Close();
            FireEvent(NetEvent.Close, "");
        }
    }

    //Send
    public static void Send(ProtoBuf.IExtensible msg)
    {
        //状态判断
        if (socket == null || !socket.Connected)
        {
            return;
        }
        if (isConnecting)
        {
            return;
        }
        if (isClosing)
        {
            return;
        }

        //数据编码
        byte[] nameBytes = ProtoSerializer.EncodeName(msg);
        byte[] bodyBytes = ProtoSerializer.Encode(msg);
        int len = nameBytes.Length + bodyBytes.Length;
        byte[] sendBytes = new byte[2 + len];
        //组装长度
        sendBytes[0] = (byte)(len % 256);
        sendBytes[1] = (byte)(len / 256);
        //组装协议名
        Array.Copy(nameBytes, 0, sendBytes, 2, nameBytes.Length);
        //组装协议体
        Array.Copy(bodyBytes, 0, sendBytes, 2 + nameBytes.Length, bodyBytes.Length);

        //写入队列
        ByteArray ba = new ByteArray(sendBytes);
        int count = 0; //writeQueue的长度
        lock (writeQueue)
        {
            writeQueue.Enqueue(ba);
            count = writeQueue.Count;
        }
        //Send
        if (count == 1)
        {
            socket.BeginSend(sendBytes, 0, sendBytes.Length, 0, SendCallback, socket);
        }
    }

    //Send回调
    public static void SendCallback(IAsyncResult ar)
    {
        //获取state、EndSend的处理
        Socket socket = (Socket)ar.AsyncState;
        //状态判断
        if (socket == null || !socket.Connected)
        {
            return;
        }

        //EndSend
        int count = socket.EndSend(ar);
        //获取写入队列的第一条数据
        ByteArray ba;
        lock (writeQueue)
        {
            ba = writeQueue.First();
        }
        //完整发送
        ba.readIdx += count;
        if (ba.length == 0)
        {
            lock (writeQueue)
            {
                writeQueue.Dequeue();
                ba = writeQueue.First();
            }
        }

        if (ba != null)
        {
            //继续发送
            socket.BeginSend(ba.bytes, ba.readIdx, ba.length, 0, SendCallback, socket);
        }
        else if (isClosing)
        {
            //正在关闭
            socket.Close();
        }
    }

    //处理消息事件模块

    //消息委托类型
    public delegate void MsgListener(ProtoBuf.IExtensible msgBase);
    //消息事件队列
    private static Dictionary<string, MsgListener> msgListeners = new Dictionary<string, MsgListener>();

    //添加消息事件
    public static void AddMsgListener(string msgName, MsgListener listener)
    {
        //添加委托
        if (msgListeners.ContainsKey(msgName))
        {
            msgListeners[msgName] += listener;
        }
        else
        {
            //新增事件
            msgListeners[msgName] = listener;
        }
    }

    //移除消息事件
    public static void RemoveMsgListener(string msgName, MsgListener listener)
    {
        if (msgListeners.ContainsKey(msgName))
        {
            msgListeners[msgName] -= listener;
            if (msgListeners[msgName] == null)
            {
                msgListeners.Remove(msgName);
            }
        }
    }

    //触发消息事件
    public static void FireMsg(string msgName, ProtoBuf.IExtensible msgBase)
    {
        if (msgListeners.ContainsKey(msgName))
        {
            msgListeners[msgName](msgBase);
        }
    }

    //Update
    public static void Update()
    {
        MsgUpdate(); //消息队列循环
        PingUpdate(); //心跳循环
    }
    //更新消息
    public static void MsgUpdate()
    {
        //初步判断，提升性能（延迟一两帧问题不大）
        if (msgCount == 0)
        {
            return;
        }
        //重复处理消息
        for (int i = 0; i < MAX_MESSAGE_FIRE; i++)
        {
            //获取第一条消息
            ProtoBuf.IExtensible msgBase = null;
            lock (msgList)
            {
                if (msgList.Count > 0)
                {
                    msgBase = msgList[0];
                    msgList.RemoveAt(0);
                    msgCount--;
                }
            }
            //分发消息
            if (msgBase != null)
            {
                string[] allName = msgBase.ToString().Split('.');
                string protoName = allName[allName.Length - 1];
                FireMsg(protoName, msgBase);
            }
        }

    }

    //心跳模块
    private static void PingUpdate()
    {
        //是否启用
        if (!isUsePing)
        {
            return;
        }
        //发送ping
        if (Time.time - lastPingTime > pingInterval)
        {
            gprotocol.MsgPing msgPing = new gprotocol.MsgPing();
            Send(msgPing);
            lastPingTime = Time.time; //更新ping时间
        }
        //检测pong
        if (Time.time - lastPongTime > pingInterval * 4)
        {
            Close();
        }
    }

    private static void OnMsgPong(ProtoBuf.IExtensible msgBase)
    {
        lastPongTime = Time.time; //更新pong时间
    }


}

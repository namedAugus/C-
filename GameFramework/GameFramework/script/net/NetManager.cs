using System.Net;
using System.Net.Sockets;
using System.Reflection;

public class NetManager
{
    //监听Socket
    public static Socket listenfd;
    //客户端Socket及状态信息
    public static Dictionary<Socket, ClientState> clients = new Dictionary<Socket, ClientState>();
    
    //Select检查列表
    static List<Socket> checkRead = new List<Socket>();
    //Ping间隔(需与客户端一致)
    public static long pingInterval = 30;

    public static void StarLoop(int listenPort)
    {
        //Socket
        listenfd = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //Bind
        IPAddress ipAdr = IPAddress.Parse("0.0.0.0");
        IPEndPoint ipEp = new IPEndPoint(ipAdr, listenPort);
        listenfd.Bind(ipEp);
        //Listen
        listenfd.Listen(0);
        Console.WriteLine("Server start success! Listening for connection on port " + listenPort);
        //循环
        while (true)
        {
            ResetCheckRead(); //重置checkRead
            Socket.Select(checkRead,null,null,1000); //-1代表一直阻塞，直到有socket可读
            //检查可读对象
            for (int i = checkRead.Count-1; i>=0; i--)
            {
                Socket s = checkRead[i];
                if (s == listenfd)
                {
                    ReadListenfd(s);
                }
                else
                {
                    ReadClientfd(s);
                }
            }
            //Time out
            Timer();
        }
    }
    
    //重置CheckRead
    public static void ResetCheckRead()
    {
        checkRead.Clear();
        checkRead.Add(listenfd);
        foreach (ClientState s in clients.Values)
        {
            checkRead.Add(s.socket);
        }
    }
    
    //读取Listenfd
    public static void ReadListenfd(Socket listenfd)
    {
        try
        {
            Socket clientfd = listenfd.Accept();
            Console.WriteLine("Accpt " + clientfd.RemoteEndPoint.ToString());
            ClientState state = new ClientState();
            state.socket = clientfd;
            clients.Add(clientfd,state);
        }
        catch (SocketException e)
        {
            Console.WriteLine(e.ToString());
        }
    }
    
    //读取clientfd
    public static void ReadClientfd(Socket clientfd)
    {
        if (!clients.ContainsKey(clientfd)) //TODO:这块有时候为空
        {
            Console.WriteLine("Client does not exist! on NetManager.ReadClientfd() line 84");
            return;
        }
        ClientState state = clients[clientfd];
        ByteArray readBuffer = state.readBuffer;
        //接收
        int count = 0;
        //缓冲区不够，清除，若依旧不够，只能返回
        //缓冲区的长度只有1024，单条协议超过缓冲区长度时会发生错误，根据需要调整长度
        if (readBuffer.remain <= 0)
        {
            OnReceiveData(state);
            readBuffer.MoveBytes();
        }

        if (readBuffer.remain <= 0)
        {
            Console.WriteLine("Read fail, maybe msg length > readBuff capacity !");
            Close(state);
            return;
        }

        try
        {
            count = clientfd.Receive(readBuffer.bytes, readBuffer.writeIdx, readBuffer.remain, 0);
        }
        catch (SocketException e)
        {
            Console.WriteLine("Receive SocketException " + e.ToString());
            Close(state);
            return;
        }
        
        //消息处理
        readBuffer.writeIdx += count;
        //处理二进制消息
        OnReceiveData(state);
        //移动缓冲区
        readBuffer.CheckAndMoveBytes();
    }
    
    //关闭连接
    public static void Close(ClientState state)
    {
        //事件分发
        MethodInfo mei = typeof(EventHandler).GetMethod("OnDisconnect");
        object[] ob = { state };
        mei.Invoke(null, ob);
        //关闭
        state.socket.Close();
        clients.Remove(state.socket);
    }
    
    //数据处理
    public static void OnReceiveData(ClientState state)
    {
        ByteArray readBuffer = state.readBuffer;
        byte[] bytes = readBuffer.bytes;
        
        //消息长度
        if (readBuffer.length <= 2)
        {
            return;
        }
        Int16 bodyLength = (Int16)((bytes[readBuffer.readIdx + 1] << 8) | bytes[readBuffer.readIdx]);
        //消息体
        if (readBuffer.length < bodyLength)
        {
            return;
        }

        readBuffer.readIdx += 2;
        //解析协议名
        int nameCount = 0;
        string protoName = ProtoSerializer.DecodeName(readBuffer.bytes, readBuffer.readIdx, out nameCount);

        if (protoName == "")
        {
            Console.WriteLine("OnReceiveData DecodeName failed!");
            Close(state);
        }
        readBuffer.readIdx += nameCount;
        //解析协议体
        int bodyCount = bodyLength - nameCount;
        ProtoBuf.IExtensible msgBase = ProtoSerializer.Decode(protoName, readBuffer.bytes, readBuffer.readIdx, bodyCount);
        readBuffer.readIdx += bodyCount;
        readBuffer.CheckAndMoveBytes();
        //分发消息
        MethodInfo mi = typeof(MsgHandler).GetMethod(protoName);
        object[] o = { state, msgBase };
        Console.WriteLine("Receive " + protoName);
        if (mi != null)
        {
            mi.Invoke(null, o);
        }
        else
        {
            Console.WriteLine("OnReceiveData Invoke fail " + protoName);
        }
        
        //继续读取消息
        if (readBuffer.length < 2)
        {
            OnReceiveData(state);
        }
    }
    
    //定时器（将游戏逻辑与网络模块分开）
    static void Timer()
    {
        //消息分发
        MethodInfo mei = typeof(EventHandler).GetMethod("OnTimer");
        object[] ob = {};
        mei.Invoke(null, ob);
    }
    
    //Send
    public static void Send(ClientState cs, ProtoBuf.IExtensible msg)
    {
        //状态判断
        if (cs == null)
        {
            return;
        }

        if (!cs.socket.Connected)
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
        //为化简代码，不设置回调
        try
        {
            cs.socket.BeginSend(sendBytes, 0, sendBytes.Length, 0, null, null);
        }
        catch (SocketException e)
        {
            Console.WriteLine("Socket Close on BeginSend " + e.ToString());
        }
    }
    
    //获取时间戳
    public static long GetTimeStamp()
    {
        TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
        return Convert.ToInt64(ts.TotalSeconds);
    }
}
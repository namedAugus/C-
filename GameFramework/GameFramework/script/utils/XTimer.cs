using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Diagnostics;
using GameFramework.script.system;

public class XTimer
{
    private Thread syncThread;
    // private TcpClient client;
    // private NetworkStream stream;
    private volatile bool isRunning;
    const int frameRate = 60; //帧率
    public const double interval = 1000.0 / frameRate; // 16.67 milliseconds
    public double deltaTime = 0.0;
    public static XTimer instance = null;
    // public double deltaTime
    // {
    //     get
    //     {
    //         // lock (this)
    //         // {
    //             return deltaTime;
    //         // }
    //     }
    //     private set
    //     {
    //         lock (this)
    //         {
    //             deltaTime = value;
    //         }
    //     }
    // }

    public static XTimer GetInstance()
    {
        if (instance == null)
        {
            instance = new XTimer();
        }

        return instance;
    }
    // public GameServer(TcpClient client)
    // {
    //     this.client = client;
    //     this.stream = client.GetStream();
    // }
    public XTimer()
    {
        // this.client = client;
        // this.stream = client.GetStream();
    }

    public void StartSync()
    {
        isRunning = true;
        syncThread = new Thread(SyncLoop);
        syncThread.Start();

        Console.WriteLine("Starting ticker at "+frameRate+" frames per second...");
    }

    private void SyncLoop()
    {
       
        Stopwatch stopwatch = new Stopwatch();

        while (isRunning)
        {
            stopwatch.Restart();

            SendSyncRequest();

            // Calculate the remaining time in the frame
            double elapsed = stopwatch.Elapsed.TotalMilliseconds;
            deltaTime = interval - elapsed;

            if (deltaTime > 0)
            {
                Thread.Sleep((int)deltaTime);
            }
            // Console.WriteLine(XTimer.instance.deltaTime);
            foreach (Player player in PlayerManager.players.Values)
            {
                // if (player.astarSystem != null)  //TODO:astar XTimer
                // {
                //     // Console.WriteLine("666  "+player.transform.ox);
                //     player.astarSystem.PlayerNavUpdate(player,player.transform);
                // }
            }
        }
    }

    private void SendSyncRequest()
    {
        // 构建同步请求数据
        string syncData = "Sync Request";
        byte[] data = Encoding.UTF8.GetBytes(syncData);
        
        // 发送数据到客户端
        try
        {
            // stream.Write(data, 0, data.Length);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error sending sync request: " + ex.Message);
            StopSync();
        }
    }

    public void StopSync()
    {
        isRunning = false;
        if (syncThread != null && syncThread.IsAlive)
        {
            syncThread.Join(); // Wait for the thread to finish
        }

        // // 关闭网络连接
        // if (stream != null)
        // {
        //     stream.Close();
        // }
        // if (client != null)
        // {
        //     client.Close();
        // }

        Console.WriteLine("ticker stopped.");
    }

    static void Main(string[] args)
    {
        // test
        // 模拟一个客户端连接
        TcpClient client = new TcpClient("127.0.0.1", 12345);
    
        // 创建游戏服务器实例
        // GameServer server = new GameServer(client);
        
        // 启动同步
        // server.StartSync();
    
        // 防止控制台应用程序立即退出
        Console.WriteLine("Press the Enter key to stop the sync and exit the program...");
        Console.ReadLine();
    
        // 停止同步
        // server.StopSync();
    }
}
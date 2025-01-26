using GameFramework.script.component;
using GameFramework.script.system;

public class Player
{
    //id
    public string id = ""; //TODO：目前是和account表的username映射，多player时需要改
    //指向ClientState
    public ClientState state;
    //临时数据，如：坐标
    public float x;
    public float y;
    public float z;
    public float ex;
    public float ey;
    public float ez;
    //在哪个房间
    public int roomId = -1; //-1代表不在房间中
    //阵营
    public int camp = 1;
    //hp
    public float hp = 100.0f;
    //防御值
    public float def = 80.0f;
    //攻击力(应该放在子弹那)
    public float att = 100.0f;
    
    
    //降龙测试数据
    public TransformComponent transform;
    public int animState;
    // public AStarSystem astarSystem;
    
    //数据库数据
    public PlayerData data;
    
    //构造函数
    public Player(ClientState state)
    {
        this.state = state;
        this.transform = new TransformComponent();
        this.animState = (int)AnimState.Idle;
    }
    //发送信息
    public void Send(ProtoBuf.IExtensible msg)
    {
        NetManager.Send(state, msg);
    }
}

/*
//同步坦克信息
public class MsgSyncTank : MsgBase
{
    public MsgSyncTank() { protoName = "MsgSyncTank"; }
    //位置、旋转、炮塔旋转
    public float x = 0f;
    public float y = 0f;
    public float z = 0f;
    public float ex = 0f;
    public float ey = 0f;
    public float ez = 0f;
    public float turretY = 0f;
    //服务端传递的时候填写
    public string id = ""; //坦克id(玩家id)
}

//开火协议
public class MsgFire : MsgBase
{
    public MsgFire() { protoName = "MsgFire"; }
    //炮弹初始位置、旋转
    public float x = 0f;
    public float y = 0f;
    public float z = 0f;
    public float ex = 0f;
    public float ey = 0f;
    public float ez = 0f;
    //服务端传递的时候填写
    public string id = ""; //坦克id(玩家id)
}

//击中协议
public class MsgHit : MsgBase
{
    public MsgHit() { protoName = "MsgHit"; }
    //击中谁
    public string targetId = "";
    //击中点（给服务端判断作弊用，暂留）
    public float x = 0f;
    public float y = 0f;
    public float z = 0f;
    //服务端传递的时候填写
    public string id = ""; //坦克id(玩家id)
    public float hp = 0; //被击中坦克的血量
    public float damage = 0; //收到的伤害
}
*/
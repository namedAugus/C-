using gprotocol;
using UnityEngine;
/// <summary>
/// 坦克同步类，与玩家的CtrlTank对应
/// </summary>
public class SyncTank : BaseTank
{
    //预测算法的预测信息，哪个时间到达哪个位置
    private Vector3 lastPos; //上一次位置
    private Vector3 lastRot; //上一次旋转
    private Vector3 forecastPos; //预测位置
    private Vector3 forecastRot; //预测旋转
    private float forecastTime; //预测时间

    //重写init方法，让同步tank完全有服务器控制
    public override void Init(string skinPath)
    {
        base.Init(skinPath);
        //不受物理运动影响
        rigidBody.constraints = RigidbodyConstraints.FreezeAll;
        rigidBody.useGravity = false;
        //初始化预测信息
        lastPos = transform.position;
        lastPos = transform.eulerAngles;
        forecastPos = transform.position;
        forecastRot = transform.eulerAngles;
        forecastTime = Time.time;
    }

    new void Update()
    {
        base.Update();
        //更新位置
        ForecastUpdate();
    }
    //更新位置
    public void ForecastUpdate()
    {
        //时间
        float t = (Time.time - forecastTime) / CtrlTank.syncInterval;
        t = Mathf.Clamp(t, 0f, 1f);
        //位置
        Vector3 pos = transform.position;
        pos = Vector3.Lerp(pos, forecastPos, t);
        transform.position = pos;

        //旋转
        Quaternion quat = transform.rotation;
        Quaternion forecastQuat = Quaternion.Euler(forecastRot);
        quat = Quaternion.Lerp(quat, forecastQuat, t);
        transform.rotation = quat;
    }

    //移动同步
    public void SyncPos(MsgSyncTank msg)
    {
        //预测位置
        Vector3 pos = new Vector3(msg.x, msg.y, msg.z);
        Vector3 rot = new Vector3(msg.ex, msg.ey, msg.ez);
        forecastPos = pos + 2 * (pos - lastPos); //也可以不预测
        forecastRot = rot + 2 * (rot - lastRot);

        //更新原
        lastPos = pos;
        lastRot = rot;
        forecastTime = Time.time;

        //炮塔角度没有预测（可以自行实现)
        Vector3 le = turret.localEulerAngles;
        le.y = msg.turretY;
        turret.localEulerAngles = le;
    }

    //炮弹同步
    public void SyncFire(MsgFire msg)
    {
        Bullet bullet = Fire();
        //更新坐标
        Vector3 pos = new Vector3(msg.x, msg.y, msg.z);
        Vector3 rot = new Vector3(msg.ex, msg.ey, msg.ez);
        bullet.transform.position = pos;
        bullet.transform.eulerAngles = rot;
    }
}
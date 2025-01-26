﻿using gprotocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CtrlTank : BaseTank
{
    //上一次发送同步信息的时间
    private float lastSendSyncTime;
    //同步帧率
    public static float syncInterval = 0.1f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        base.Update();
        //移动控制
        MoveUpdate();
        //炮塔控制
        TurretUpdate();
        //开炮控制
        FireUpdate();

        //发送同步信息
        SyncUpdate();
    }
    //同步控制
    public void SyncUpdate()
    {
        //时间间隔判断
        if (Time.time - lastSendSyncTime < syncInterval)
        {
            return;
        }
        lastSendSyncTime = Time.time; //刷新发送同步时间
        //发送同步协议
        MsgSyncTank msg = new MsgSyncTank();
        msg.x = transform.position.x;
        msg.y = transform.position.y;
        msg.z = transform.position.z;
        msg.ex = transform.eulerAngles.x;
        msg.ey = transform.eulerAngles.y;
        msg.ez = transform.eulerAngles.z;
        msg.turretY = turret.localEulerAngles.y;
        NetManager.Send(msg);
    }
    //移动控制
    public void MoveUpdate()
    {
        if (IsDie())
        {
            return;
        }
        //旋转
        float x = Input.GetAxis("Horizontal");
        transform.Rotate(0, x * steer * Time.deltaTime, 0);
        //前进后退
        float y = Input.GetAxis("Vertical");
        Vector3 s = y * transform.forward * speed * Time.deltaTime;
        transform.position += s;
    }
    //炮塔控制
    public void TurretUpdate()
    {
        if (IsDie())
        {
            return;
        }
        //获取轴向
        float axis = 0;
        if (Input.GetKey(KeyCode.Q))
        {
            axis = -1;
        }
        else if (Input.GetKey(KeyCode.E))
        {
            axis = 1;
        }
        //旋转角度
        Vector3 le = turret.localEulerAngles;
        // 角度增量 = 方向 * 时间 * 速度
        le.y += axis * Time.deltaTime * turretSpeed;
        turret.localEulerAngles = le; //turret本地欧拉角
    }

    //开炮控制
    public void FireUpdate()
    {
        if (IsDie())
        {
            return;
        }
        //按键判定
        if (!Input.GetKey(KeyCode.Space))
        {
            return;
        }
        //cd判断
        if (Time.time - lastFireTime < fireCd)
        {
            return;
        }
        //发射
        Bullet bullet = Fire(); //父类基本方法
        //发送同步协议
        MsgFire msg = new MsgFire();
        msg.x = bullet.transform.position.x;
        msg.y = bullet.transform.position.y;
        msg.z = bullet.transform.position.z;
        msg.ex = bullet.transform.eulerAngles.x;
        msg.ey = bullet.transform.eulerAngles.y;
        msg.ez = bullet.transform.eulerAngles.z;
        NetManager.Send(msg);
    }
}

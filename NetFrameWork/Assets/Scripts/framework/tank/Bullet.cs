using gprotocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    //移动速度
    public float speed = 100f;
    //发射者
    public BaseTank tank;
    //炮弹模型
    private GameObject skin;
    //刚体
    Rigidbody rigidBody;
    //攻击力
    public float att = 100.0f;

    //初始化
    public void Init()
    {
        //皮肤
        GameObject skinRes = ResManager.LoadPrefab("bulletPrefab");
        skin = (GameObject)Instantiate(skinRes);
        skin.transform.parent = this.transform;
        skin.transform.localPosition = Vector3.zero;
        skin.transform.localEulerAngles = Vector3.zero;

        //物理
        rigidBody = gameObject.AddComponent<Rigidbody>();
        rigidBody.useGravity = false;
    }
    void Start()
    {

    }

    void Update()
    {
        //向前移动
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    //碰撞
    private void OnCollisionEnter(Collision collision)
    {
        //打到坦克
        GameObject collObj = collision.gameObject;
        BaseTank hitTank = collObj.GetComponent<BaseTank>();
        //不能打到自己
        if (hitTank == tank)
        {
            return;
        }
        //打到其他坦克
        if (hitTank != null)
        {
            hitTank.Attacked(att);
            SendMsgHit(tank, hitTank); //同步hit协议发送
        }
        //显示爆炸效果
        GameObject explode = ResManager.LoadPrefab("Particles/fire");
        Instantiate(explode, transform.position, transform.rotation);
        //消毁炮弹
        Destroy(gameObject);
    }

    //发送hit协议 (原，目标）
    void SendMsgHit(BaseTank tank, BaseTank hitTank)
    {
        if (hitTank == null || tank == null)
        {
            return;
        }
        //不是自己发出的炮弹(这个需要辨别，在客户端)
        if (tank.id != GameMain.id)
        {
            return;
        }
        MsgHit msg = new MsgHit();
        msg.targetId = hitTank.id;
        msg.id = tank.id;
        msg.x = transform.position.x;
        msg.y = transform.position.y;
        msg.z = transform.position.z;
        NetManager.Send(msg);
    }
}

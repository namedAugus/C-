using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 坦克基类
/// </summary>
public class BaseTank : MonoBehaviour
{
    //坦克模型
    private GameObject skin;

    //转向速度
    public float steer = 30;
    //移动速度
    public float speed = 3.0f;
    //刚体
    protected Rigidbody rigidBody;

    //炮塔旋转速度
    public float turretSpeed = 30f;
    //炮塔
    public Transform turret;
    //炮管
    public Transform gun;
    //发射点
    public Transform firePoint;
    //炮弹cd
    public float fireCd = 0.5f;
    //上一次发射炮弹时间
    public float lastFireTime = 0;
    //hp
    public float hp = 100.0f;
    //防御值
    public float def = 80.0f;
    //攻击力(应该放在子弹那)
    public float att = 100.0f;

    //网络需求的属性
    //属于哪一位玩家
    public string id = "";
    //阵营
    public int camp = 0;

    private void Start()
    {

    }

    //初始化
    public virtual void Init(string skinPath)
    {
        //皮肤（坦克模型)
        GameObject skinRes = ResManager.LoadPrefab(skinPath);
        skin = (GameObject)Instantiate(skinRes);
        skin.transform.parent = this.transform;
        skin.transform.localPosition = Vector3.zero;
        skin.transform.localEulerAngles = Vector3.zero;
        //物理
        rigidBody = gameObject.AddComponent<Rigidbody>();
        //rigidBody.useGravity = false;
        BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>();
        boxCollider.center = new Vector3(0, 2.5f, 1.47f);
        boxCollider.size = new Vector3(7, 5, 12);
        //炮塔炮管
        turret = skin.transform.Find("Turret");
        gun = turret.transform.Find("Gun");
        firePoint = gun.transform.Find("FirePoint");
    }

    internal void Update()
    {

    }
    public bool IsDie()
    {
        return hp <= 0;
    }
    //发射炮弹
    public Bullet Fire()
    {
        //已经死亡
        if (IsDie())
        {
            return null;
        }
        //产生炮弹
        GameObject bulletObj = new GameObject("bullet");
        Bullet bullet = bulletObj.AddComponent<Bullet>();
        bullet.Init();
        bullet.tank = this;
        //位置
        bullet.transform.position = firePoint.position;
        bullet.transform.rotation = firePoint.rotation;
        //更新时间(至于cd交给ctrltank处理）
        lastFireTime = Time.time;
        return bullet;
    }
    //被攻击
    public void Attacked(float att)
    {
        //已经死亡
        if (IsDie())
        {
            return;
        }
        //扣血
        hp = hp - (att - def);
        //死亡
        if (IsDie())
        {
            //显示焚烧效果
            GameObject obj = ResManager.LoadPrefab("Particles/explosion");
            GameObject explosion = Instantiate(obj, transform.position, transform.rotation);
            explosion.transform.SetParent(transform, false);
            Invoke("OnDestroy", 1.0f);
        }
    }
    private void OnDestroy()
    {
        Destroy(gameObject);
    }
}

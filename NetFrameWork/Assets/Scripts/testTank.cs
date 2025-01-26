using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testTank : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameObject tankObj = new GameObject("myTank");
        //BaseTank baseTank = tankObj.AddComponent<BaseTank>();
        //baseTank.Init("TankPrefab/tankPrefab");

        CtrlTank ctrlTank = tankObj.AddComponent<CtrlTank>();
        ctrlTank.Init("TankPrefab/tankPrefab");
        ctrlTank.transform.localPosition = new Vector3(-70, 0, 30);
        //加相机
        tankObj.AddComponent<CameraFollow>();

        //添加测试受击tank
        GameObject tankObj2 = new GameObject("enemyTank");
        BaseTank baseTank = tankObj2.AddComponent<BaseTank>();
        baseTank.Init("TankPrefab/tankPrefab");
        baseTank.transform.position = new Vector3(-50, 0, 30);

        //测试面板
        //PanelManager.Init();
        //PanelManager.Open<LoginPanel>();
        //PanelManager.Open<TipPanel>("你最帅~");
    }

    // Update is called once per frame
    void Update()
    {

    }
}

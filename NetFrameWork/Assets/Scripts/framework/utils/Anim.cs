using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//强制要求当前代码加入一个Image组件
//如果没有Image组件，那么自动加上，如果有就使用
[RequireComponent(typeof(Image))]
public class Anim : MonoBehaviour
{

    //Image对象
    private Image image;
    //动画帧数组 暴露出去用编辑器拖动每一帧进来
    public Sprite[] sprite;
    //帧动画播放的时间间隔
    public float duration = 0.1f;
    // 是否循环播放
    public bool isLoop = false;
    //是否在加载的时候开始播放
    public bool isPlayOnload = false;
    //已播放的时间
    private float playedTime;
    //是否在播放中
    private bool isPlaying = false;

    // Use this for initialization
    void Start()
    {
        this.image = this.GetComponent<Image>();
        if (this.isPlayOnload)
        {
            if (this.isLoop)
            {
                this.PlayLoop();
            }
            else
            {
                this.PlayOnce();
            }
        }
    }
    void PlayOnce()
    {
        if (this.sprite.Length <= 1)
        {
            return;
        }
        this.playedTime = 0;
        this.isPlaying = true;
        this.isLoop = false;
    }
    void PlayLoop()
    {
        if (this.sprite.Length <= 1)
        {
            return;
        }
        this.playedTime = 0;
        this.isPlaying = true;
        this.isLoop = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (this.isPlaying)
        {
            this.playedTime += Time.deltaTime; //获得过去的总时间
            int index = (int)(this.playedTime / duration); //向下取整，获取当前时间下到了哪一帧
            if (this.isLoop == false)
            { //once
                if (index >= this.sprite.Length)
                { //结束了
                    this.isPlaying = false;
                    this.playedTime = 0;
                }
                else
                {
                    this.image.sprite = this.sprite[index]; //播放
                }
            }
            else
            { //loop
                while (index >= this.sprite.Length)
                {
                    index -= this.sprite.Length; //将下标回退到初始值（一个周期）
                    this.playedTime -= (this.duration * this.sprite.Length); //将时间回退一个周期
                }
                this.image.sprite = this.sprite[index]; //播放
            }
        }
    }
}
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class LeftAnim : MonoBehaviour
{

    //Image对象
    private Image image;
    //纹理对象
    public Texture2D[] texture;
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

    void Init()
    {


        sprite = new Sprite[texture.Length];
        for (int i = 0; i < texture.Length; i++)
        {
            this.sprite[0] = Sprite.Create(texture[i],
               new Rect(0, 0, this.texture[i].width, texture[i].height),
               new Vector2(0.5f, 0.5f),
               100.0f);

        }

    }

    // Use this for initialization
    void Start()
    {
        //this.Init();
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
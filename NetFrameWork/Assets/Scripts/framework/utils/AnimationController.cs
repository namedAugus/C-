/**
 * 动画控制器  用于对散图按顺序播放动画
 * @作者 落日故人 QQ 583051842
 * 
 */
using UnityEngine;
using UnityEngine.UI;

public class AnimationController : MonoBehaviour {

    public Sprite[] images = null;

    public float frameTime = 0.1f;

    /// <summary>
    /// 播放次数
    /// </summary>
    public int playTimes = 0;

    /**
     * 是否倒播
     */
    public bool reverse = false;

    public bool autoPlayOnLoad = true;

    /// <summary>
    /// 播放完自动销毁
    /// </summary>
    public bool autoDestroy = false;

    public int frameNum = 0;

    public int frameIndex = 0;

    public int nextFrameIndex = 0;


    public bool running = true;

    private Image m_render = null;

    private float time = 0;

   

    private int currentTimes = 0;

    private RectTransform uiTransform = null;
   

    public void Awake()
    {
        this.uiTransform = this.GetComponent<RectTransform>();
        this.m_render = this.GetComponent<Image>();
    }

	// Use this for initialization
	public void Start () {
        if (this.images.Length != 0)
        {
            this.frameNum = this.images.Length; 
        }

        //this.time = this.frameTime;
        this.running = this.autoPlayOnLoad;

        if(this.reverse)
        {
            this.frameIndex = this.frameNum - 1;
            this.nextFrameIndex = this.frameNum - 1;
        }

	}
	
	// Update is called once per frame
    public void Update () {
        
        if (!this.running)
            return;

        float dt = Time.deltaTime;
        if (this.images.Length == 0)
            return;

        this.time -= dt;

        if (this.playTimes != 0 && this.currentTimes == this.playTimes)
        {
            this.running = false;
            return;
        }
            

        if (this.time <= 0)
        {
            this.time = this.frameTime;

            if(!this.reverse)
            {
                this.frameIndex = this.nextFrameIndex % this.frameNum;
                this.nextFrameIndex = this.frameIndex + 1;

                this.m_render.sprite = this.images[this.frameIndex];
                
                if(this.m_render.sprite)
                {
                    Rect rect = this.m_render.sprite.rect;
                    this.uiTransform.sizeDelta = new Vector2(rect.width, rect.height); 
                }
                
               
                if (this.frameIndex == this.frameNum - 1)
                {
                    this.currentTimes++;

                   

                    if (this.playTimes != 0 && this.currentTimes == this.playTimes)
                    {
                        if (this.autoDestroy)
                        {
                            GameObject.Destroy(this.gameObject);
                        }
                    }
                }
            }else
            {
                this.frameIndex = (this.nextFrameIndex + this.frameNum) % this.frameNum;
                this.nextFrameIndex = this.frameIndex - 1;

                this.m_render.sprite = this.images[this.frameIndex];
                
                if(this.m_render.sprite)
                {
                    Rect rect = this.m_render.sprite.rect;
                    this.uiTransform.sizeDelta = new Vector2(rect.width, rect.height);
                }
                

                if (this.frameIndex == 0)
                {
                    this.currentTimes++;

                  
                    if (this.playTimes != 0 && this.currentTimes == this.playTimes)
                    {

                       

                        if (this.autoDestroy)
                        {
                            GameObject.Destroy(this.gameObject);
                        }
                    }
                }

            }
        }
	}

    /// <summary>
    /// 播放
    /// </summary>
    public void play()
    {
        this.running = true;
        this.frameIndex = 0;
        this.currentTimes = 0;
        
        this.time = this.frameTime;

        if (this.images.Length != 0)
        {
            this.frameNum = this.images.Length;

            if(this.reverse)
            {
                this.frameIndex = this.frameNum - 1;
                this.nextFrameIndex = this.frameNum - 1;
            }

        }

        if(!this.m_render)
        {
            this.m_render = this.GetComponent<Image>();
        }

        if (this.m_render)
            this.m_render.sprite = this.images[0];

        
        if(this.m_render.sprite)
        {
            Rect rect = this.m_render.sprite.rect;
            this.uiTransform.sizeDelta = new Vector2(rect.width, rect.height);
        }

    }

    public void gotoAndPlay(int frameIndex)
    {
        if(!this.m_render)
        {
            this.m_render = this.GetComponent<Image>();
        }

        this.running = true;
        this.frameIndex = frameIndex;
        this.nextFrameIndex = frameIndex;
        this.currentTimes = 0;
        //this.time = 0;
    }

    /// <summary>
    /// 停止
    /// </summary>
    public void stop()
    {
        this.running = false;
    }

    public void gotoAndStop(int frameIndex)
    {
        this.frameIndex = frameIndex;

        if (this.frameIndex < 0)
            this.frameIndex = 0;

        if (this.frameIndex > this.images.Length - 1)
            this.frameIndex = this.images.Length - 1;

        if(!this.m_render)
        {
            this.m_render = this.GetComponent<Image>();
        }

        this.m_render.sprite = this.images[this.frameIndex];

        if(this.m_render.sprite)
        {
            Rect rect = this.m_render.sprite.rect;
            this.uiTransform.sizeDelta = new Vector2(rect.width, rect.height);
        }

        this.running = false;
    }

    public bool isPlayEnd()
    {
        return this.frameIndex == this.frameNum;
    }

}

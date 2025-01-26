using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class MovieClip : MonoBehaviour
{
    private float timer = 0.01f;
    private Image m_sprite = null;

    public float interval = 0.1f;
    public Texture2D texture = null;
    public float playTimes = 0;
    public int row = 4;
    public int col = 4;
    public int rowIndex = 0;

    public bool isAll = false;
    public bool autoPlayOnLoad = true;
    public bool autoDestroy = false;
    public int begin = 0;
    public int end = 0;

    private int totalFrame = 0;
    private int currentFrame = 0;
    private int currentTimes = 0;
    private bool running = true;
    public int playIndex = 0;
    private int _pieceWidth = 0;
    private int _pieceHeight = 0;
    private Sprite[][] _bitmapArr = null;

    private int clamp(int value, int minLimit, int maxLimit)
    {
        if (value < minLimit) {
            return minLimit;
        }

        if (value > maxLimit) {
            return maxLimit;
        }

        return value;
    }

    public void setAlpha(float value) {
        this.m_sprite.color = new Color(1.0f, 1.0f, 1.0f, value);
    }

    public void Awake() {
        if (this.end == 0) {
            this.end = this.col;
        }

        this.rowIndex = this.clamp(this.rowIndex, 0, this.row - 1);
        this._pieceWidth = this.texture.width / this.col;
        this._pieceHeight = this.texture.height / this.row;

        this.m_sprite = this.GetComponent<Image>();

        if (!this.m_sprite) {
            this.m_sprite = this.gameObject.AddComponent<Image>();
        }

        this._bitmapArr = new Sprite[this.row][];
        for (int i = 0; i < this.row; i++) {
            this._bitmapArr[i] = new Sprite[this.col];

            for (int j = 0; j < this.col; j++) {
                this._bitmapArr[i][j] = Sprite.Create(this.texture, new Rect(j * this._pieceWidth, i * this._pieceHeight, this._pieceWidth, this._pieceHeight), new Vector2(0, 0));
            }
        }

        this.m_sprite.sprite = this._bitmapArr[this.rowIndex][0];
        ((RectTransform)(this.m_sprite.transform)).sizeDelta = new Vector2(this._pieceWidth, this._pieceHeight);
        
        this.timer = 0;
        this.running = this.autoPlayOnLoad;
    }

    public void reset() {
        this.timer = 0;
        this.playIndex = 0;
        this.currentTimes = 0;
        this.currentFrame = 0;

        this.playAction();
    }

    public void playAction()
    {
        this.rowIndex = this.clamp(this.rowIndex, 0, this.row - 1);

        this.playIndex = this.playIndex % (this.end - this.begin) + this.begin;

        this.m_sprite.sprite = this._bitmapArr[this.rowIndex][this.playIndex];
        //this.m_sprite.spriteFrame.setRect(this.rect);

        this.playIndex++;
    }

    public void play()
    {
        this.running = true;
    }

    public void stop()
    {
        this.running = false;
    }

    public void gotoAndPlay(int frame)
    {
        this.running = true;
        this.playIndex = frame;
        this.playIndex = this.clamp(this.playIndex, 0, this.col - 1);
    }

    public void gotoAndStop(int frame)
    {
        this.running = false;

        this.playIndex = frame;
        this.playIndex = this.clamp(this.playIndex, 0, this.col - 1);

        this.m_sprite.sprite = this._bitmapArr[this.rowIndex][this.playIndex];
    }

    public void Update() {
        this.animUpdate(Time.deltaTime);
    }

    public void animUpdate(float dt) {

        if (!this.running)
            return;

        if (this.playTimes != 0 && this.currentTimes == this.playTimes)
        {
            this.running = false;
            return;
        }


        this.timer -= dt;

        if (this.timer <= 0)
        {

            this.timer = this.interval;

            this.currentFrame = this.currentFrame % this.col;

            this.playAction();

            this.currentFrame++;

            if (this.currentFrame == this.col)
            {

                if (this.isAll)
                {
                    this.rowIndex++;

                    if (this.rowIndex == this.row)
                    {
                        this.currentTimes++;

                        // this.node.emit("completeTimes");

                        if (this.playTimes != 0 && this.currentTimes == this.playTimes)
                        {
                            // this.node.emit("complete");

                            if (this.autoDestroy)
                            {
                                // this.node.destroy();
                                GameObject.Destroy(this.gameObject);
                            }
                        }
                    }

                    this.rowIndex %= this.row;
                }
                else
                {
                    this.currentTimes++;

                    // this.node.emit("completeTimes");

                    if (this.playTimes != 0 && this.currentTimes == this.playTimes)
                    {
                        // this.node.emit("complete");

                        if (this.autoDestroy) {
                            // this.node.destroy();
                            GameObject.Destroy(this.gameObject);
                        }
                    }
                }

            }
        }

    }
}

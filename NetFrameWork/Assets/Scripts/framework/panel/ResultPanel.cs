using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultPanel : BasePanel
{
    //胜利显示图片
    private Image winImage;
    //失败显示图片
    private Image lostImage;
    //确定按钮
    private Button okBtn;

    //初始化
    public override void OnInit()
    {
        skinPath = "ResultPanel";
        layer = PanelManager.Layer.Tip;
    }
    //显示
    public override void OnShow(params object[] para)
    {
        //寻找组件
        winImage = skin.transform.Find("WinImage").GetComponent<Image>();
        lostImage = skin.transform.Find("LostImage").GetComponent<Image>();
        okBtn = skin.transform.Find("OkBtn").GetComponent<Button>();
        //监听
        okBtn.onClick.AddListener(OnOkClick);
        //显示哪张图片
        if (para.Length == 1)
        {
            bool isWin = (bool)para[0];
            if (isWin)
            {
                winImage.gameObject.SetActive(true);
                lostImage.gameObject.SetActive(false);
            }
            else
            {
                winImage.gameObject.SetActive(false);
                lostImage.gameObject.SetActive(true);
            }
        }
    }

    //关闭
    public override void OnClose()
    {

    }

    public void OnOkClick()
    {
        PanelManager.Open<RoomPanel>();
        Close();
    }
}

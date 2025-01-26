using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TipPanel : BasePanel
{
    //提示文本
    private Text text;
    //确定按钮
    private Button okBtn;

    //初始化
    public override void OnInit()
    {
        skinPath = "TipPanel";
        layer = PanelManager.Layer.Tip;
    }

    //显示
    public override void OnShow(params object[] para)
    {
        text = skin.transform.Find("Text").GetComponent<Text>();
        okBtn = skin.transform.Find("OKBtn").GetComponent<Button>();
        //监听
        okBtn.onClick.AddListener(this.OnOkClick);
        //提示语
        if (para.Length == 1)
        {
            text.text = (string)para[0];
        }
    }

    private void OnOkClick()
    {
        Close();
    }
    //关闭
    public override void OnClose()
    {

    }
}

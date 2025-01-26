using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResManager : MonoBehaviour
{
    //加载预设
    public static GameObject LoadPrefab(string path)
    {
        return Resources.Load<GameObject>(path); //该api以Resources为根目录加载
    }
    //加载Texture2D
    public static Texture2D LoadTexture2D(string path)
    {
        return Resources.Load<Texture2D>(path); //该api以Resources为根目录加载
    }
}

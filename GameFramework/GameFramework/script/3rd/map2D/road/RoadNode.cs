/**
 * 路点信息
 * @作者 落日故人 QQ 583051842
 */
public class RoadNode {

    private float _px;//像素坐标x轴
    private float _py;//像素坐标y轴

    private int _cx;//世界坐标x轴 真 （存储用）
    private int _cy;//世界坐标y轴 真 （存储用）

    private int _dx;//直角坐标x轴
    private int _dy;//直角坐标y轴
    
    private int _value = 0;//节点的值
    private int _f = 0; //路点的f值
    private int _g = 0; //路点的g值	
    private int _h = 0; //路点的h值
    private RoadNode _parent = null; //路点的父节点
    

    //-------------二堆叉存储结构-----------------
    private RoadNode _treeParent = null; //二堆叉结构的父节点

    private RoadNode _left = null; //二堆叉结构的左子节点

    private RoadNode _right = null; //二堆叉结构的右子节点

    private int _openTag = 0; //是否在开启列表标记

    private int _closeTag = 0; //是否在关闭列表标记

    public RoadNode()
    {
    }
    
    /**
     * 重置二叉堆存储信息
     */
    public void resetTree()
    {
        this._treeParent = null;
        this._left = null;
        this._right = null;
    }

    public string toString()
    {
        return "路点像素坐标：（" + this._px + "," + this._py +"),  " +
            "路点世界坐标：（" + this._cx + "," + this._cy +"),  " + 
            "路点平面直角坐标：（" + this._dx + "," + this._dy +")";
    }

    public float px
    {
        get { return this._px; }
        set { this._px = value; }
    }

    public float py {
        get { return this._py; }
        set { this._py = value; }
    }


    public int cx
    {
        get { return this._cx; }
        set { this._cx = value; }
    }

    public int cy
    {
        get { return this._cy; }
        set { this._cy = value; }
    }

    public int dx
    {
        get { return this._dx; }
        set { this._dx = value; }
    }

    public int dy
    {
        get { return this._dy; }
        set { this._dy = value; }
    }

    public int value
    {
        get { return this._value; }
        set { this._value = value; }
    }

    public int f
    {
        get { return this._f; }
        set { this._f = value; }
    }

    public int g
    {
        get { return this._g; }
        set { this._g = value; }
    }

    public int h
    {
        get { return this._h; }
        set { this._h = value; }
    }

    public RoadNode parent
    {
        get { return this._parent; }
        set { this._parent = value; }
    }

    //-------------二堆叉存储结构-----------------

    /**
     * 二堆叉结构的父节点
     */
    public RoadNode treeParent
    {
        get { return this._treeParent; }
        set { this._treeParent = value; }
    }
    /**
     * 二堆叉结构的左子节点
     */
    public RoadNode left
    {
        get { return this._left; }
        set { this._left = value; }
    }

    /**
     * 二堆叉结构的右子节点
     */
    public RoadNode right
    {
        get { return this._right; }
        set { this._right = value; }
    }

    /**
     * 是否在开启列表标记
     */
    public int openTag
    {
        get { return this._openTag; }
        set { this._openTag = value; }
    }
    /**
     * 是否在关闭列表标记
     */
    public int closeTag
    {
        get { return this._closeTag; }
        set { this._closeTag = value; }
    }

}

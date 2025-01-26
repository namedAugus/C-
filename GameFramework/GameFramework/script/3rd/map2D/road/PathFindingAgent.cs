

using System;
using System.Collections.Generic;
using UnityEngine;
/**
* 寻路代理器
* @作者 落日故人 QQ 583051842
*/
public class PathFindingAgent {

    private static PathFindingAgent _instance;

    public static PathFindingAgent instance
    {
        get {
            if(PathFindingAgent._instance == null) {
                PathFindingAgent._instance = new PathFindingAgent();
            }
            return PathFindingAgent._instance;
        }
       
    }

    private Dictionary<string, RoadNode> _roadDic = new Dictionary<string, RoadNode>();

    private IRoadSeeker _roadSeeker = null;

    private MapData _mapData = null;
    public MapData mapData{
        get {
            return this._mapData;
        }
    }

    private MapType _mapType = MapType.angle45;
    public MapType mapType
    {
        get {
            return this._mapType;
        }
    }

    /**
     * 获得寻路接口
     */
    public IRoadSeeker roadSeeker
    {
        get {
            return this._roadSeeker;
        }
    }

    /**
     *用于检索一个节点周围8个点的向量数组 (四边形格子用)
     */		
    protected int [,]_round = { { 0, -1 }, { 1, -1 }, { 1, 0 }, { 1, 1 }, { 0, 1 }, { -1, 1 }, { -1, 0 }, { -1, -1 } };
    //private _round1:number[][] = [[0,-1],[1,0],[0,1],[-1,0]]; //只要4方向周围的路点，请使用这个

    /**
     *用于检索一个节点周围6个点的向量数组 格子列数为偶数时使用  (六边形格子用)
     */		
    protected int[,] _round1 = { { 0, -1 }, { 1, -1 }, { 1, 0 }, { 0, 1 }, { -1, 0 }, { -1, -1 } };

    /**
     *用于检索一个节点周围6个点的向量数组 格子列数为奇数时使用  (六边形格子用)
     */
    protected int[,] _round2 = { { 0, -1 }, { 1, 0 }, { 1, 1 }, { 0, 1 }, { -1, 1 }, { -1, 0 } };


    /**
     * 初始化寻路数据
     * @param roadDataArr 路电数据
     * @param mapType 地图类型
     */
    public void init(MapData mapData)
    {
        this._mapData = mapData;
        this._mapType = mapData.type;

        MapRoadUtils.instance.updateMapInfo(this._mapData.mapWidth,this._mapData.mapHeight,this._mapData.nodeWidth,this._mapData.nodeHeight,this._mapData.type);
        
        int len = this._mapData.roadDataArr.Length;
        int len2= this._mapData.roadDataArr[0].Length;
        
        int value = 0;
        int dx = 0;
        int dy = 0;
        int cx = 0;
        int cy = 0;

        for(int i = 0 ; i < len ; i++)
        {
            for(int j = 0 ; j < len2 ; j++)
            {
                value = this._mapData.roadDataArr[i][j];
                dx = j;
                dy = i;

                RoadNode node = MapRoadUtils.instance.getNodeByDerect(dx,dy);
                node.value = value; // 可以走的，不可以走的，还是半透走，隐藏走的;

                if(this._mapType == MapType.honeycomb2)
                {
                    cx = node.cx;
                    cy = node.cy;

                    //如果是横式六边形，则需要把路点世界坐标转置，即x,y调换。因为六边形寻路组件AstarHoneycombRoadSeeker是按纵式六边形写的
                    node.cx = cy;
                    node.cy = cx;
                }

                this._roadDic[node.cx + "_" + node.cy] = node;
            }
        }

        if(this._mapType == MapType.honeycomb || this._mapType == MapType.honeycomb2)
        {
            this._roadSeeker = new AstarHoneycombRoadSeeker(this._roadDic);
        }
        else
        {
            this._roadSeeker = new AStarRoadSeeker(this._roadDic);
        }
    }

    /**
     * 寻路方法，如果目标不可到达，不会返回任何路径。  备注：2d寻路，参数的单位是像素，3d寻路单位是米，2d和3d寻路是通用的
     * 
     * 如果有疑问可以联系@作者 QQ 583051842
     * 
     * @param startX 起始像素位置X
     * @param startY 起始像素位置Y
     * @param targetX 目标像素位置X
     * @param targetY 目标像素位置Y
     * @param radius 寻路的半径 （2d是像素，3d是米）。备注：如果寻路的角色想按自身的占地面积进行寻路，可以设置这个值
     * @returns 返回寻路路点路径
     */
    public List<RoadNode> seekPath(float startX,float startY,float targetX,float targetY,int radius= 0)
    {
        RoadNode startNode = this.getRoadNodeByPixel(startX,startY);
        RoadNode targetNode = this.getRoadNodeByPixel(targetX,targetY);
        int seakRadius = this.getSeakRadius(radius);
        
        List<RoadNode> roadNodeArr = this._roadSeeker.seekPath(startNode,targetNode,seakRadius);

        return roadNodeArr;
    }

    /**
     * 寻路方法，如果目标点不可达到，则返回离目标点最近的路径。  备注：2d寻路，参数的单位是像素，3d寻路单位是米，2d和3d寻路是通用的
     * 
     * 如果有疑问可以联系@作者 QQ 583051842
     * 
     * @param startX 起始像素位置X
     * @param startY 起始像素位置Y
     * @param targetX 目标像素位置X
     * @param targetY 目标像素位置Y
     * @param radius 寻路的半径 （2d是像素，3d是米）。备注：如果寻路的角色想按自身的占地面积进行寻路，可以设置这个值
     * @returns 返回寻路路点路径
     */
    public List<RoadNode> seekPath2(float startX, float startY, float targetX, float targetY,int radius = 0)
    {
        RoadNode startNode = this.getRoadNodeByPixel(startX,startY);
        RoadNode targetNode = this.getRoadNodeByPixel(targetX,targetY);
        int seakRadius = this.getSeakRadius(radius);

        List<RoadNode> roadNodeArr = this._roadSeeker.seekPath2(startNode,targetNode,seakRadius);

        return roadNodeArr;
    }

    /**
     * 测试寻路过程，测试用，如果遇到bug，可以通过这个函数监测寻路的每一个步骤的情况。能快速定位问题。
     * @param startX 起始像素位置X
     * @param startY 目标像素位置Y
     * @param targetX 目标像素位置X
     * @param targetY 目标像素位置Y
     * @param radius 寻路的像素位置
     * @param seekRoadCallback 寻路的每个步骤都会执行这个回调
     * @param target 回调函数的目标对象，用于给回调函数指定this是什么
     * @param time 测试寻路时，每个步骤之间的时间间隔，单位是毫秒，如果想慢点查看每个寻路步骤，可以把这个值设置大点
     */	
    public void testSeekRoad(float startX, float startY, float targetX, float targetY,int radius,Action seekRoadCallback,object target,float time)
    {
        RoadNode startNode = this.getRoadNodeByPixel(startX,startY);
        RoadNode targetNode = this.getRoadNodeByPixel(targetX,targetY);
        int seakRadius = this.getSeakRadius(radius);

        this._roadSeeker.testSeekPathStep(startNode,targetNode,seakRadius,seekRoadCallback,target,time);
    }

    /**
     * 把像素半径转换为寻路半径,寻路半径是以路点为单位的半径，不能用像素半径进行寻路，所以要转换
     * @param radius 像素半径
     */
    public int getSeakRadius(int radius = 0)
    {
        float nodeRadius = Mathf.Min(MapRoadUtils.instance.halfNodeWidth,MapRoadUtils.instance.halfNodeHeight); //把路点的宽半径和高半径之间的最小值作为路点的半径

        //seakRadius 是寻路用的半径，单位以路点为单位。和radius以像素为单位有区别，所以要把radius转换成寻路用的半径
        int seakRadius = (int)Mathf.Ceil((radius - nodeRadius) / (2 * nodeRadius));
        if(seakRadius > 0)
        {
            return seakRadius;
        }

        return 0;
    }

    /**
     * 两个路点之间是否可直达（即是否可以两点一线到达）
     * @param startNode 
     * @param targetNode 
     */
    public bool isArriveBetweenTwoNodes(RoadNode startNode, RoadNode targetNode)
    {
        if(startNode == null || targetNode == null)
        {
            //两个点，只要有一个不在地图内，就是不可直达
            return false;
        }

        return this._roadSeeker.isArriveBetweenTwoNodes(startNode,targetNode);
    }

    /**
     * 在地图上，两个位置之间是否可直达。2d是像素位置，3d是米
     * @param x1 位置1 x轴
     * @param y1 位置1 y轴
     * @param x2 位置2 x轴
     * @param y2 位置2 y轴
     * @returns 
     */
    public bool isArriveBetweenTwoPos(float x1, float y1, float x2, float y2)
    {
        RoadNode startNode = this.getRoadNodeByPixel(x1,y1);
        RoadNode targetNode = this.getRoadNodeByPixel(x2,y2);
        return this._roadSeeker.isArriveBetweenTwoNodes(startNode,targetNode);
    }

    /**
     * 根据像素坐标获得路节点
     * @param px 像素坐标x
     * @param py 像素坐标y
     * @returns 
     */
    public RoadNode getRoadNodeByPixel(float px, float py)
    {
        Point point = MapRoadUtils.instance.getWorldPointByPixel(px,py);

        RoadNode node = null;
        if(this._mapType == MapType.honeycomb2) //因为初始化时 横式六边形已经对世界坐标做过转置，所以读取路节点时也要通过转置的方式
        {
            node = this.getRoadNode(point.y,point.x);
        }else
        {
            node = this.getRoadNode(point.x,point.y);
        }
        
        return node;
    }

    /**
     * 根据世界坐标获得路节点
     * @param cx 
     * @param cy 
     */
    public RoadNode getRoadNode(int cx,int cy)
    {
        //return this._roadDic[cx + "_" + cy];
        return this._roadSeeker.getRoadNode(cx,cy);
    }

    /**
     * 获得一个路点周围相邻的所有路点
     * @param roadNode 选定的路点
     * @param isIncludeSelf 是否包含选定的路点
     * @returns 
     */	
    public List<RoadNode> getRoundRoadNodes(RoadNode roadNode, bool isIncludeSelf = false)
    {
        if(roadNode == null)
        {
            return new List<RoadNode>();
        }

        List<RoadNode> nodeArr = new List<RoadNode>();
        int[,] round = null;

        if(isIncludeSelf)
        {
            nodeArr.Add(roadNode);
        }

        if(this.mapType == MapType.honeycomb || this.mapType == MapType.honeycomb2)
        {
            round = (roadNode.cx % 2 == 0) ? this._round1 : this._round2;
        }else
        {
            round = this._round;
        }

        for(int i = 0; i < round.GetLength(0); i++)
        {
            int cx = roadNode.cx + round[i,0];
            var cy = roadNode.cy + round[i,1];

            nodeArr.Add(this.getRoadNode(cx,cy));
        }

        return nodeArr;
    }

    /**
     * 设置最大寻路步骤，超过这个寻路步骤还寻不到终点，就默认无法达到终点。
     * 设置这个限制的目的是，防止地图太大，路点太多，没有限制的话，寻路消耗会很大，甚至会卡死
     * @param maxStep 
     */
    public void setMaxSeekStep(int maxStep)
    {
        this._roadSeeker.setMaxSeekStep(maxStep);
    }

    /**
     * 设置路径优化等级，具体优化内容请查看脚本PathOptimize，里面有对各种优化等级的详细解释
     * @param optimize 
     */
    public void setPathOptimize(PathOptimize optimize)
    {
        this._roadSeeker.setPathOptimize(optimize);
    }

    /**
     * 设置4方向路点地图的寻路类型，只针对45度和90度地图，不包括六边形地图（六边形地图设置这个值无任何反应）
     * @param pathQuadSeek  4方向路点地图的寻路类型
     */
    public void setPathQuadSeek(PathQuadSeek pathQuadSeek)
    {
        this._roadSeeker.setPathQuadSeek(pathQuadSeek);
    }

    /**
     * 自定义路点是否能通过的条件，
     * @param callback 回调方法用于自定义路点是否可通过的条件，如果参数是null，则用默认判断条件，不是null，则取代默认判断条件
     */
    public void setRoadNodePassCondition(IRoadSeeker.PassTest callback)
    {
        
        this._roadSeeker.setRoadNodePassCondition(callback);
    }
}

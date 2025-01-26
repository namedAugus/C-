

using System.Collections.Generic;
/**
* 地图障碍边缘算法工具
* @作者 落日故人 QQ 583051842
*/
public class ObstacleEdgeUtils{

    private static ObstacleEdgeUtils _instance;

    public static ObstacleEdgeUtils instance
    {
        get {
            if (ObstacleEdgeUtils._instance == null)
            {
                ObstacleEdgeUtils._instance = new ObstacleEdgeUtils();
            }
            return ObstacleEdgeUtils._instance;
        }
        
    }

    /**
     * 用绘图工具显示障碍边缘，测试用
     * @param graphics 
     */
    public void showObstacleEdge(/*Graphics graphics*/)
    {
        /*var pointDic:{[key:string]:QuadNode} = ObstacleEdgeUtils.instance.getEdge(); 
        ObstacleEdgeUtils.instance.optimizeEdge(pointDic);
        var obstacleLines:ObstacleLine[] = ObstacleEdgeUtils.instance.getObstacleEdge();

        graphics.clear();
        graphics.lineWidth = 2.4;
        graphics.strokeColor.fromHEX("#ffff00");
        graphics.fillColor.fromHEX("#ff0000");

        var nodeWidth:number = MapRoadUtils.instance.nodeWidth;
        var nodeHeight:number = MapRoadUtils.instance.nodeHeight;
        var halfNodeWidth:number = nodeWidth / 2;
        var halfNodeHeight:number = nodeHeight / 2;

        var mapType:MapType = MapRoadUtils.instance.mapType;

        var pointNum:number = 0;

        for(var key in pointDic)
        {
            if(pointDic[key] == null)
            {
                continue;
            }

            pointNum ++;

            var roadNode:RoadNode = MapRoadUtils.instance.getNodeByWorldPoint(pointDic[key].x + 0.5, pointDic[key].y + 0.5)

            if(mapType == MapType.angle45)
            {
                //菱形
                //console.log("edge",key,roadNode.px - halfNodeWidth, roadNode.py - halfNodeHeight);
                graphics.circle(roadNode.px, roadNode.py - halfNodeHeight, 5);
                graphics.fill();
            }else if(mapType == MapType.angle90)
            {
                //矩形
                //console.log("edge",key,roadNode.px - halfNodeWidth, roadNode.py - halfNodeHeight);
                graphics.circle(roadNode.px - halfNodeWidth, roadNode.py - halfNodeHeight, 5);
                graphics.fill();
            }
        }

        //this.drawEdge(pointDic);

        var len:number = obstacleLines.length;
        for(var i = 0 ; i < len ; i++)
        {
            graphics.moveTo(obstacleLines[i].startX, obstacleLines[i].startY);
            graphics.lineTo(obstacleLines[i].endX, obstacleLines[i].endY);
            graphics.stroke();
        }

        //console.log("点数量",pointNum,"线数量",len);
        */
    }

    public List<ObstacleLine> getObstacleEdge()
    {
        Dictionary<string, QuadNode> pointDic = this.getEdge();
        this.optimizeEdge(pointDic);
        return this.getEdgeLine(pointDic);
    }

    public void optimizeEdge(Dictionary<string, QuadNode>  pointDic)
    {
        List<string> invalidKeys = new List<string>(); //要删除的key列表

        foreach(var key in pointDic.Keys)
        {
            QuadNode quadNode = pointDic[key];

            var isHorizonalLine = (quadNode.left != null && quadNode.right != null);
            var isVerticalLine = (quadNode.up != null  && quadNode.down != null);

            if(isHorizonalLine && !isVerticalLine)
            {
                quadNode.left.right = quadNode.right;
                quadNode.right.left = quadNode.left;
                quadNode.left = null;
                quadNode.right = null;
                invalidKeys.Add(key);
            }

            if(!isHorizonalLine && isVerticalLine)
            {
                quadNode.up.down = quadNode.down;
                quadNode.down.up = quadNode.up;
                quadNode.up = null;
                quadNode.down = null;
                invalidKeys.Add(key);
            }
        }

        int len = invalidKeys.Count;

        for(var i = 0 ; i < len; i++)
        {
            string key = invalidKeys[i];

            // pointDic[key] = null;
            // delete pointDic[key];
            pointDic.Remove(key);
        }
    }

    public List<ObstacleLine> getEdgeLine(Dictionary<string, QuadNode> pointDic)
    {
        float nodeWidth = MapRoadUtils.instance.nodeWidth;
        float nodeHeight = MapRoadUtils.instance.nodeHeight;

        float halfNodeWidth = nodeWidth / 2;
        float halfNodeHeight = nodeHeight / 2;

        MapType mapType = MapRoadUtils.instance.mapType;

        QuadNode quadNode = null;
        RoadNode roadNode1 = null;
        RoadNode roadNode2 = null;

        float startX = 0;
        float startY = 0;

        float endX = 0;
        float endY = 0;

        List<ObstacleLine> obstacleLines = new List<ObstacleLine>();

        foreach(var key in pointDic.Keys)
        {
            quadNode = pointDic[key];

            roadNode1 = MapRoadUtils.instance.getNodeByWorldPoint((int)(quadNode.x + 0.5), (int)(quadNode.y + 0.5));

            if(mapType == MapType.angle45)
            {
                startX = roadNode1.px;
            }else if(mapType == MapType.angle90)
            {
                startX = roadNode1.px - halfNodeWidth;
            }

            startY = roadNode1.py - halfNodeHeight;

            if(quadNode.left != null && !quadNode.connectLeft)
            {
                
                roadNode2 = MapRoadUtils.instance.getNodeByWorldPoint((int)(quadNode.left.x + 0.5), (int)(quadNode.left.y + 0.5));
                
                if(mapType == MapType.angle45)
                {
                    endX = roadNode2.px;
                }else if(mapType == MapType.angle90)
                {
                    endX = roadNode2.px - halfNodeWidth;
                }
                
                endY = roadNode2.py - halfNodeHeight;

                ObstacleLine obstacleLine = new ObstacleLine();
                obstacleLine.moveTo(startX, startY);
                obstacleLine.lineTo(endX, endY);
                obstacleLines.Add(obstacleLine);

                quadNode.connectLeft = true;
                quadNode.left.connectRight = true;
            }

            if(quadNode.up != null && !quadNode.connectUp)
            {
                roadNode2 = MapRoadUtils.instance.getNodeByWorldPoint((int)(quadNode.up.x + 0.5), (int)(quadNode.up.y + 0.5));
                
                if(mapType == MapType.angle45)
                {
                    endX = roadNode2.px;
                }else if(mapType == MapType.angle90)
                {
                    endX = roadNode2.px - halfNodeWidth;
                }

                endY = roadNode2.py - halfNodeHeight;

                ObstacleLine obstacleLine = new ObstacleLine();
                obstacleLine.moveTo(startX, startY);
                obstacleLine.lineTo(endX, endY);
                obstacleLines.Add(obstacleLine);

                quadNode.connectUp = true;
                quadNode.up.connectDown = true;
            }

            if(quadNode.right != null && !quadNode.connectRight)
            {
                roadNode2 = MapRoadUtils.instance.getNodeByWorldPoint((int)(quadNode.right.x + 0.5), (int)(quadNode.right.y + 0.5));
                
                if(mapType == MapType.angle45)
                {
                    endX = roadNode2.px;
                }else if(mapType == MapType.angle90)
                {
                    endX = roadNode2.px - halfNodeWidth;
                }

                endY = roadNode2.py - halfNodeHeight;

                ObstacleLine obstacleLine = new ObstacleLine();
                obstacleLine.moveTo(startX, startY);
                obstacleLine.lineTo(endX, endY);
                obstacleLines.Add(obstacleLine);

                quadNode.connectRight = true;
                quadNode.right.connectLeft = true;
            }

            if(quadNode.down != null && !quadNode.connectDown)
            {
                roadNode2 = MapRoadUtils.instance.getNodeByWorldPoint((int)(quadNode.down.x + 0.5), (int)(quadNode.down.y + 0.5));
                
                if(mapType == MapType.angle45)
                {
                    endX = roadNode2.px;
                }else if(mapType == MapType.angle90)
                {
                    endX = roadNode2.px - halfNodeWidth;
                }

                endY = roadNode2.py - halfNodeHeight;

                ObstacleLine obstacleLine = new ObstacleLine();
                obstacleLine.moveTo(startX, startY);
                obstacleLine.lineTo(endX, endY);
                obstacleLines.Add(obstacleLine);

                quadNode.connectDown = true;
                quadNode.down.connectUp = true;
            }
            
        }

        return obstacleLines;
    }

    public Dictionary<string, QuadNode> getEdge()
    {
        MapType mapType = MapRoadUtils.instance.mapType;

        if(mapType == MapType.angle45)
        {
            return this.getEdge45Angle();
        }else if(mapType == MapType.angle90)
        {
            return this.getEdge90Angle();
        }
        return new Dictionary<string, QuadNode>();
    }

    public Dictionary<string, QuadNode> getEdge45Angle()
    {
        int row = MapRoadUtils.instance.row;
        int col = MapRoadUtils.instance.col;

        int cx = 0;
        int cy = 0;

        Dictionary<string, QuadNode> pointDic = new Dictionary<string, QuadNode>();

        for(var i = 0 ; i <=  row; i ++)
        {
            for(var j = 0 ; j <=  col; j ++)
            {
                RoadNode roadNode = MapRoadUtils.instance.getNodeByDerect(j,i);
                cx = roadNode.cx;
                cy = roadNode.cy;

                if(roadNode.dx == 0 && roadNode.dy % 2 == 0) //特殊处理最左边边缘
                {
                    QuadNode currQuadNode = new QuadNode((cx - 0.5f), (cy + 0.5f));
                    string key = currQuadNode.x + "_" + currQuadNode.y;
                    pointDic[key] = currQuadNode;
                }

                if(roadNode.dx < col && roadNode.dy == row) //特殊处理最上边边缘
                {
                    QuadNode currQuadNode = new QuadNode((cx + 0.5f), (cy - 0.5f));
                    string key = currQuadNode.x + "_" + currQuadNode.y;
                    pointDic[key] = currQuadNode;
                }
                
                if((roadNode.dx == col && roadNode.dy % 2 == 0 ) || roadNode.dy == row)
                {
                    this.saveLeftDownCornerQuadData(cx,cy,pointDic);
                }else
                {
                    RoadNode rightUp = PathFindingAgent.instance.getRoadNode(cx, cy);

                    if(rightUp == null)
                    {
                        continue;
                    }

                    RoadNode leftUp = PathFindingAgent.instance.getRoadNode(cx - 1, cy);
                    RoadNode leftDown = PathFindingAgent.instance.getRoadNode(cx - 1, cy - 1);
                    RoadNode rightDown = PathFindingAgent.instance.getRoadNode(cx, cy - 1);

                    if(this.isEnableValue(rightUp.value))
                    {
                        if(this.isOutEdgeNode(leftUp) || this.isOutEdgeNode(leftDown) || this.isOutEdgeNode(rightDown))
                        {
                            this.saveLeftDownCornerQuadData(cx,cy,pointDic);
                        }
                    }else if(this.isDisableValue(rightUp.value))
                    {
                        if(!this.isOutEdgeNode(leftUp) || !this.isOutEdgeNode(leftDown) || !this.isOutEdgeNode(rightDown))
                        {
                            this.saveLeftDownCornerQuadData(cx,cy,pointDic);
                        }
                    }
                }
            }
        }

        return pointDic;
    }

    public Dictionary<string, QuadNode> getEdge90Angle()
    {
        int row = MapRoadUtils.instance.row;
        int col = MapRoadUtils.instance.col;

        int cx = 0;
        int cy = 0;

        Dictionary<string, QuadNode> pointDic = new Dictionary<string, QuadNode>();

        for(var i = 0 ; i <= row; i ++)
        {
            for(var j = 0 ; j <= col; j ++)
            {
                cx = j;
                cy = i;
                
                if(cx == col || cy == row)
                {
                    this.saveLeftDownCornerQuadData(cx,cy,pointDic);
                }else
                {
                    RoadNode rightUp = PathFindingAgent.instance.getRoadNode(cx, cy);

                    RoadNode leftUp = PathFindingAgent.instance.getRoadNode(cx - 1, cy);
                    RoadNode leftDown = PathFindingAgent.instance.getRoadNode(cx - 1, cy - 1);
                    RoadNode rightDown = PathFindingAgent.instance.getRoadNode(cx, cy - 1);
    
                    if(this.isEnableValue(rightUp.value))
                    {
                        if(this.isOutEdgeNode(leftUp) || this.isOutEdgeNode(leftDown) || this.isOutEdgeNode(rightDown))
                        {
                            this.saveLeftDownCornerQuadData(cx,cy,pointDic);
                        }
                    }else if(this.isDisableValue(rightUp.value))
                    {
                        if(!this.isOutEdgeNode(leftUp) || !this.isOutEdgeNode(leftDown) || !this.isOutEdgeNode(rightDown))
                        {
                            this.saveLeftDownCornerQuadData(cx,cy,pointDic);
                        }
                    }
                }
            }
        }

        return pointDic;
    }

    private void saveLeftDownCornerQuadData(float cx, float cy, Dictionary<string, QuadNode> pointDic)
    {
        QuadNode currQuadNode = new QuadNode(cx - 0.5f, cy - 0.5f);
        string key = currQuadNode.x + "_" + currQuadNode.y;
        pointDic[key] = currQuadNode;

        RoadNode leftUp = PathFindingAgent.instance.getRoadNode((int)(currQuadNode.x - 0.5), (int)(currQuadNode.y + 0.5));
        RoadNode rightUp = PathFindingAgent.instance.getRoadNode((int)(currQuadNode.x + 0.5), (int)(currQuadNode.y + 0.5));
        RoadNode rightDown = PathFindingAgent.instance.getRoadNode((int)(currQuadNode.x + 0.5), (int)(currQuadNode.y - 0.5));
        RoadNode leftDown = PathFindingAgent.instance.getRoadNode((int)(currQuadNode.x - 0.5), (int)(currQuadNode.y - 0.5));

        var leftQuadNode = pointDic[(currQuadNode.x - 1) + "_" + currQuadNode.y];
        if(leftQuadNode != null)
        {
            
            if((!this.isOutEdgeNode(leftUp) &&  this.isOutEdgeNode(leftDown)) || 
                (this.isOutEdgeNode(leftUp) &&  !this.isOutEdgeNode(leftDown))
                ){
                currQuadNode.left = leftQuadNode;
                leftQuadNode.right = currQuadNode;
            }
        }

        var upQuadNode = pointDic[currQuadNode.x + "_" + (currQuadNode.y + 1)];
        if(upQuadNode != null)
        {
            if((!this.isOutEdgeNode(leftUp) &&  this.isOutEdgeNode(rightUp)) || 
                   (this.isOutEdgeNode(leftUp) &&  !this.isOutEdgeNode(rightUp))
                ){
                    currQuadNode.up = upQuadNode;
                    upQuadNode.down = currQuadNode;
                }
        }

        var rightQuadNode = pointDic[(currQuadNode.x + 1) + "_" + currQuadNode.y];
        if(rightQuadNode != null)
        {
            if((!this.isOutEdgeNode(rightUp) &&  this.isOutEdgeNode(rightDown)) || 
                   (this.isOutEdgeNode(rightUp) &&  !this.isOutEdgeNode(rightDown))
                ){
                    currQuadNode.right = rightQuadNode;
                    rightQuadNode.left = currQuadNode;
                }
        }

        var downQuadNode = pointDic[currQuadNode.x + "_" + (currQuadNode.y - 1)];
        if(downQuadNode != null)
        {
            if((!this.isOutEdgeNode(leftDown) &&  this.isOutEdgeNode(rightDown)) || 
                   (this.isOutEdgeNode(leftDown) &&  !this.isOutEdgeNode(rightDown))
                ){
                    currQuadNode.down = downQuadNode;
                    downQuadNode.up = currQuadNode;
                }
        }
    }

    public bool isEnableValue(int value)
    {
        if(value != 1)
        {
            return true;
        }

        return false;
    }

    public bool isDisableValue(int value)
    {
        if(value == 1)
        {
            return true;
        }

        return false;
    }

    public bool isOutEdgeNode(RoadNode node)
    {
        if(node == null || this.isDisableValue(node.value))
        {
            return true;
        }

        return false;
    }
  
}

public class ObstacleLine
{
    public float startX = 0;
    public float startY = 0;

    public float endX = 0;
    public float endY = 0;

    public void moveTo(float x,float y)
    {
        this.startX = x;
        this.startY = y;
    }

    public void lineTo(float x,float y)
    {
        this.endX = x;
        this.endY = y;
    }
}

public class QuadNode
{
    public float x = 0;
    public float y = 0;

    public QuadNode(float x = 0 , float y = 0)
    {
        this.x = x;
        this.y = y;
    }

    public QuadNode left = null;
    public QuadNode up = null;
    public QuadNode right = null;
    public QuadNode down = null;

    public bool connectLeft = false;
    public bool connectUp = false;
    public bool connectRight = false;
    public bool connectDown = false;
}



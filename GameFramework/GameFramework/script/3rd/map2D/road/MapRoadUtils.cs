
using System;
using UnityEngine;
/**
* 地图路点信息算法工具，包括方形，菱形，六边形的路点算法
* @作者 落日故人 QQ 583051842
*/
public class MapRoadUtils {

	private static MapRoadUtils _instance;

	public static MapRoadUtils instance
	{
		get{
			if (MapRoadUtils._instance == null)
			{
				MapRoadUtils._instance = new MapRoadUtils();
			}
			return MapRoadUtils._instance;
		}
    }

    /**
     * 地图宽度
     */		
    private float _mapWidth; 
    
    /**
     *地图高度 
     */		
    private float _mapHeight;
    
    /**
     *地图一共分成几行 
     */		
    private int _row;
    
    /**
     *地图一共分成几列 
     */		
    private int _col;
    
    /**
     *地图路点单元格宽 
     */		
    private float _nodeWidth;
    
    /**
     *地图路点单元格高 
     */		
    private float _nodeHeight;
    
    /**
     *地图路点单元宽的一半 
     */	
    private float _halfNodeWidth;
    
    /**
     *地图路点单元高的一半 
     */	
    private float _halfNodeHeight;
    
	/**
	 * 地图类型
	 */
    private MapType _mapType;
    
    private IMapRoad _mapRoad;

    public void updateMapInfo(int mapWidth, int mapHeight, int nodeWidth, int nodeHeight, MapType mapType)
    {
        this._mapWidth = mapWidth;
        this._mapHeight = mapHeight;
        this._nodeWidth = nodeWidth;
        this._nodeHeight = nodeHeight;
        
        this._halfNodeWidth = (int) (this._nodeWidth / 2);
        this._halfNodeHeight = (int) (this._nodeHeight / 2);
        
        this._mapType = mapType;
        
		
        switch(this._mapType)
        {
            case MapType.angle45:

				this._col = (int)Mathf.Ceil(mapWidth / this._nodeWidth);
				this._row = (int)Mathf.Ceil(mapHeight / this._nodeHeight) * 2;

				this._mapRoad = new MapRoad45Angle(this._row,this._col,this._nodeWidth,this._nodeHeight,this._halfNodeWidth,this._halfNodeHeight);break;
            case MapType.angle90:

				this._col = (int)Mathf.Ceil(mapWidth / this._nodeWidth);
				this._row = (int)Mathf.Ceil(mapHeight / this._nodeHeight);

				this._mapRoad = new MapRoad90Angle(this._row,this._col,this._nodeWidth,this._nodeHeight,this._halfNodeWidth,this._halfNodeHeight);break;
            case MapType.honeycomb:
                
                //this._nodeHeight = (this._nodeWidth / 2) * 1.732;
                
                this._col = (int)Mathf.Ceil((this._mapWidth - this._nodeWidth / 4) / (this._nodeWidth / 4 * 6)) * 2;
                this._row = (int)Mathf.Ceil((this._mapHeight - this._nodeHeight / 2) / this._nodeHeight);
                
                this._mapRoad = new MapRoadHoneycomb(this._row,this._col,this._nodeWidth,this._nodeHeight,this._halfNodeWidth,this._halfNodeHeight);break;
		
			case MapType.honeycomb2:
			
				//this._nodeHeight = (this._nodeWidth / 2) * 1.732;
				
				this._col = (int)Mathf.Ceil((this._mapWidth - this._nodeHeight / 2) / this._nodeHeight);
				this._row = (int)Mathf.Ceil((this._mapHeight - this._nodeWidth / 4) / (this._nodeWidth / 4 * 6)) * 2;
				
				this._mapRoad = new MapRoadHoneycomb2(this._row,this._col,this._nodeWidth,this._nodeHeight,this._halfNodeWidth,this._halfNodeHeight);break;
			
		}
		
    }
    
    /**
     *根据地图平面像素坐标获得路节点  
        * @param x 
        * @param y
        * @return 
        * 
        */		
    public RoadNode getNodeByPixel(float x,float y)
    {
        if(this._mapRoad != null)
        {
            return this._mapRoad.getNodeByPixel(x,y);
        }
        return new RoadNode();
    }
    
    /**
     *根据路点平面坐标点获得路节点  
        * @param px
        * @param py
        * @return 
        * 
        */			
    public RoadNode getNodeByDerect(int dx, int dy)
    {
        if(this._mapRoad != null)
        {
            return this._mapRoad.getNodeByDerect(dx,dy);
        }
        return new RoadNode();
    }
    
    /**
     *根据路点场景世界坐标获得路节点 
        * @param cx
        * @param cy
        * @return 
        * 
        */			
    public RoadNode getNodeByWorldPoint(int cx, int cy)
    {
        if(this._mapRoad != null)
        {
            return this._mapRoad.getNodeByWorldPoint(cx,cy);
        }
        return new RoadNode();
    }
    
    /**
     *根据像素坐标得到场景世界坐标 
        * @param x
        * @param y
        * @return 
        * 
        */		
    public Point getWorldPointByPixel(float x,float y)
    {
        if(this._mapRoad != null)
        {
            return this._mapRoad.getWorldPointByPixel(x,y);
        }
        return new Point();
    }
    
    /**
     *根据世界坐标获得像素坐标 
        * @param cx
        * @param cy
        * @return 
        * 
        */		
    public Point getPixelByWorldPoint(int cx, int cy)
    {
        if(this._mapRoad != null)
        {
            return this._mapRoad.getPixelByWorldPoint(cx,cy);
        }
        return new Point();
    }
    
    /**
     *根据像素坐标获得网格平面坐标
        * @param x
        * @param y
        * @return 
        * 
        */		
    public Point getDerectByPixel(float x,float y)
    {
        if(this._mapRoad != null)
        {
            return this._mapRoad.getDerectByPixel(x,y);
        }
        return new Point();
        
    }
    
    /**
     *根据世界坐标获得网格平面坐标 
        * @param cx
        * @param cy
        * @return 
        * 
        */		
    public Point getDerectByWorldPoint(int cx, int cy)
    {
        if(this._mapRoad != null)
        {
            return this._mapRoad.getDerectByWorldPoint(cx,cy);
        }
        return new Point();
    }
    
    /**
     *根据网格平面坐标获得世界坐标 
        * @param dx
        * @param dy
        * @return 
        * 
        */		
/*	public getWorldPointByDerect(float dx, float dy):Point
    {
        var cx:number = (dy + dx) / 2;
        var cy:number = (dy - dx) / 2 + col - 1;
        return new Point(cx,cy);
    }*/
    
    public Point getPixelByDerect(int dx,int dy)
    {
        if(this._mapRoad != null)
        {
            return this._mapRoad.getPixelByDerect(dx,dy);
        }
        return new Point();
    }
    
	/**
	 * 地图宽
	 */
    public float mapWidth
    {
		get {
			return this._mapWidth;
		}
        
    }
    
	/**
	 * 地图高
	 */
    public float mapHeight
    {
		get {
			return this._mapHeight;
		}
        
    }
    
	/**
	 * 路点宽
	 */
    public float nodeWidth
    {
		get {
			return this._nodeWidth;
		}
        
    }
    
	/**
	 * 路点高
	 */
    public float nodeHeight
    {
		get {
			return this._nodeHeight;
		}
        
    }
    
	/**
	 * 路点总行数
	 */
	public int row
	{
		get { return this._row; }
		set { this._row = value; }
	}
/**
 * 路点总列数
 */
	public int col
	{
		get { return this._col; }
		set { this._col = value; }
	}
/**
 * 路点宽的一半
 */
	public float halfNodeWidth
	{
		get { return this._halfNodeWidth; }
		set { this._halfNodeWidth = value; }
	}
/**
 * 路点高的一半
 */
	public float halfNodeHeight
	{
		get { return this._halfNodeHeight; }
		set { this._halfNodeHeight = value; }
	}
/**
 *地图类型
*/
	public MapType mapType
	{
		get { return this._mapType; }
		set { this._mapType = value; }
	}
}
	
/**
 *地图路点处理接口 
 * @author Administrator
 * 
 */
public interface IMapRoad
{
	/**
	 *根据地图平面像素坐标获得路节点  
	 * @param x 
	 * @param y
	 * @return 
	 * 
	 */
	public RoadNode getNodeByPixel(float x,float y);

	/**
	 *根据路点平面坐标点获得路节点  
	 * @param px
	 * @param py
	 * @return 
	 * 
	 */
	public RoadNode getNodeByDerect(int dx, int dy);

	/**
	 *根据路点场景世界坐标获得路节点 
	 * @param cx
	 * @param cy
	 * @return 
	 * 
	 */
	public RoadNode getNodeByWorldPoint(int cx, int cy);


	/**
	 *根据像素坐标得到场景世界坐标 
	 * @param x
	 * @param y
	 * @return 
	 * 
	 */
	public Point getWorldPointByPixel(float x, float y);


	/**
	 *根据世界坐标获得像素坐标 
	 * @param cx
	 * @param cy
	 * @return 
	 * 
	 */
	public Point getPixelByWorldPoint(int cx, int cy);
	
	
	/**
	 *根据像素坐标获得网格平面坐标
	 * @param x
	 * @param y
	 * @return 
	 * 
	 */		
	public Point getDerectByPixel(float x, float y);
	
	
	/**
	 *根据世界坐标获得网格平面坐标 
	 * @param cx
	 * @param cy
	 * @return 
	 * 
	 */		
	public Point getDerectByWorldPoint(int cx, int cy);
	
	
	/**
	 *根据网格平面坐标获得像素坐标 
	 * @param dx
	 * @param dy
	 * @return 
	 * 
	 */		
	public Point getPixelByDerect(int dx, int dy);
}

/**
 *45度等视角地图路点处理接口实现 
 * @author Administrator
 * 
 */
class MapRoad45Angle : IMapRoad
{
	
	/**
	 *地图一共分成几行 
	 */		
	private int _row;
	
	/**
	 *地图一共分成几列 
	 */		
	private int _col;
	
	/**
	 *地图路点单元格宽 
	 */		
	private float _nodeWidth;
	
	/**
	 *地图路点单元格高 
	 */		
	private float _nodeHeight;
	
	/**
	 *地图路点单元宽的一半 
	 */	
	private float _halfNodeWidth;
	
	/**
	 *地图路点单元高的一半 
	 */	
	private float _halfNodeHeight;
	
	public MapRoad45Angle(int row,int col,float nodeWidth,float nodeHeight, float halfNodeWidth,float halfNodeHeight)
	{
		this._row = row;
		this._col = col;
		this._nodeWidth = nodeWidth;
		this._nodeHeight = nodeHeight;
		this._halfNodeWidth = halfNodeWidth;
		this._halfNodeHeight = halfNodeHeight;
	}
	
	/**
	 *根据地图平面像素坐标获得路节点  
	 * @param x 
	 * @param y
	 * @return 
	 * 
	 */		
	public RoadNode getNodeByPixel(float x, float y)
	{
		Point wPoint = this.getWorldPointByPixel(x,y);
		Point fPoint = this.getPixelByWorldPoint(wPoint.x,wPoint.y);
		Point dPoint = this.getDerectByPixel(x,y);

		RoadNode node = new RoadNode();
		
		node.cx = (int)wPoint.x;
		node.cy = wPoint.y;
		
		node.px = fPoint.x;
		node.py = fPoint.y;
		
		node.dx = dPoint.x;
		node.dy = dPoint.y;
		
		return node;
	}
	
	/**
	 *根据路点平面坐标点获得路节点  
	 * @param px
	 * @param py
	 * @return 
	 * 
	 */			
	public RoadNode getNodeByDerect(int dx,int dy)
	{

		Point fPoint = this.getPixelByDerect(dx,dy);
		Point wPoint = this.getWorldPointByPixel(fPoint.x,fPoint.y);

		RoadNode node = new RoadNode();
		
		node.cx = wPoint.x;
		node.cy = wPoint.y;

		node.px = fPoint.x; // 图像坐标
		node.py = fPoint.y; // 推向
		
		node.dx = dx;
		node.dy = dy;
		
		return node;
	}
	
	/**
	 *根据路点场景世界坐标获得路节点 
	 * @param cx
	 * @param cy
	 * @return 
	 * 
	 */			
	public RoadNode getNodeByWorldPoint(int cx, int cy)
	{
		Point point = this.getPixelByWorldPoint(cx, cy);
		return this.getNodeByPixel(point.x,point.y);
	}
	
	/**
	 *根据像素坐标得到场景世界坐标 
	 * @param x
	 * @param y
	 * @return 
	 * 
	 */		
	public Point getWorldPointByPixel(float x,float y)
	{
		int cx = (int) Mathf.Ceil(x/this._nodeWidth - 0.5f + y/this._nodeHeight) - 1;
		int cy = (this._col - 1) - (int)Mathf.Ceil(x/this._nodeWidth - 0.5f - y/this._nodeHeight);
		
		return new Point(cx,cy);
	}
	
	/**
	 *根据世界坐标获得像素坐标 
	 * @param cx
	 * @param cy
	 * @return 
	 * 
	 */		
	public Point getPixelByWorldPoint(int cx, int cy)
	{
		int x = (int) Mathf.Floor((cx + 1 - (cy  - (this._col - 1))) * this._halfNodeWidth);
		int y = (int) Mathf.Floor((cx + 1 + (cy  - (this._col - 1))) * this._halfNodeHeight);
		return new Point(x,y);
	}
	
	/**
	 *根据像素坐标获得网格平面坐标
	 * @param x
	 * @param y
	 * @return 
	 * 
	 */		
	public Point getDerectByPixel(float x,float y)
	{
		Point worldPoint = this.getWorldPointByPixel(x,y);
		Point pixelPoint = this.getPixelByWorldPoint(worldPoint.x,worldPoint.y);
		int dx = (int) Mathf.Floor( pixelPoint.x / this._nodeWidth ) - ( pixelPoint.x  % this._nodeWidth == 0 ? 1 : 0 );
		int dy =  (int) Mathf.Floor( pixelPoint.y / this._halfNodeHeight ) - 1;
		return new Point(dx,dy);
	}
	
	/**
	 *根据世界坐标获得网格平面坐标 
	 * @param cx
	 * @param cy
	 * @return 
	 * 
	 */		
	public Point getDerectByWorldPoint(int cx, int cy)
	{
		int dx = (int) ((cx - (cy - (this._col -1)))/2);
		int dy = cx  + (cy  - (this._col - 1));
		return new Point(dx,dy);
	}
	
	/**
	 *根据网格平面坐标获得像素坐标 
	 * @param dx
	 * @param dy
	 * @return 
	 * 
	 */		
	public Point getPixelByDerect(int dx, int dy )
	{
		int x = (int) Mathf.Floor((dx + dy % 2) * this._nodeWidth + (1 - dy % 2) * this._halfNodeWidth);
		int y = (int) Mathf.Floor((dy + 1) * this._halfNodeHeight);
		return new Point(x,y);
	}
}

/**
 *90度平面地图路点处理接口实现 
 * @author Administrator
 * 
 */
class MapRoad90Angle : IMapRoad
{
	/**
	 *地图一共分成几行 
	 */		
	private int _row;
	
	/**
	 *地图一共分成几列 
	 */		
	private int _col;
	
	/**
	 *地图路点单元格宽 
	 */		
	private float _nodeWidth;
	
	/**
	 *地图路点单元格高 
	 */		
	private  float _nodeHeight;
	
	/**
	 *地图路点单元宽的一半 
	 */	
	private float _halfNodeWidth;
	
	/**
	 *地图路点单元高的一半 
	 */	
	private float _halfNodeHeight;
	
	public MapRoad90Angle(int row,int col,float nodeWidth,float nodeHeight,float halfNodeWidth,float halfNodeHeight)
	{
		this._row = row;
		this._col = col;
		this._nodeWidth = nodeWidth;
		this._nodeHeight = nodeHeight;
		this._halfNodeWidth = halfNodeWidth;
		this._halfNodeHeight = halfNodeHeight;
	}
	
	/**
	 *根据地图平面像素坐标获得路节点  
	 * @param x 
	 * @param y
	 * @return 
	 * 
	 */		
	public RoadNode getNodeByPixel(float x, float y)
	{
		Point wPoint = this.getWorldPointByPixel(x,y);
		Point fPoint = this.getPixelByWorldPoint(wPoint.x,wPoint.y);
		Point dPoint = this.getDerectByPixel(x,y);

		RoadNode node = new RoadNode();
		
		node.cx = wPoint.x;
		node.cy = wPoint.y;
		
		node.px = fPoint.x;
		node.py = fPoint.y;
		
		node.dx = dPoint.x;
		node.dy = dPoint.y;
		
		return node;
	}
	
	/**
	 *根据路点平面坐标点获得路节点  
	 * @param px
	 * @param py
	 * @return 
	 * 
	 */			
	public RoadNode getNodeByDerect(int dx, int dy)
	{

		Point fPoint = this.getPixelByDerect(dx,dy);
		Point wPoint = this.getWorldPointByPixel(fPoint.x,fPoint.y);

		RoadNode node = new RoadNode();
		
		node.cx = wPoint.x;
		node.cy = wPoint.y;
		
		node.px = fPoint.x;
		node.py = fPoint.y;
		
		node.dx = dx;
		node.dy = dy;
		
		return node;
	}
	
	/**
	 *根据路点场景世界坐标获得路节点 
	 * @param cx
	 * @param cy
	 * @return 
	 * 
	 */			
	public RoadNode getNodeByWorldPoint(int cx,int cy)
	{
		Point point = this.getPixelByWorldPoint(cx, cy);
		return this.getNodeByPixel(point.x,point.y);
	}
	
	/**
	 *根据像素坐标得到场景世界坐标 
	 * @param x
	 * @param y
	 * @return 
	 * 
	 */		
	public Point getWorldPointByPixel(float x, float y)
	{
		int cx = (int)(x / this._nodeWidth);
		int cy = (int)(y / this._nodeHeight);
		
		return new Point(cx,cy);
	}
	
	/**
	 *根据世界坐标获得像素坐标 
	 * @param cx
	 * @param cy
	 * @return 
	 * 
	 */		
	public Point getPixelByWorldPoint(int cx, int cy)
	{
		int x = (int) Mathf.Floor((cx + 1) * this._nodeWidth - this._halfNodeWidth);
		int y = (int) Mathf.Floor((cy + 1) * this._nodeHeight - this._halfNodeHeight);
		return new Point(x,y);
	}
	
	/**
	 *根据像素坐标获得网格平面坐标
	 * @param x
	 * @param y
	 * @return 
	 * 
	 */		
	public Point getDerectByPixel(float x, float y)
	{
		int dx = (int) Mathf.Floor(x / this._nodeWidth);
		int dy = (int) Mathf.Floor(y / this._nodeHeight);
		return new Point(dx,dy);
	}
	
	/**
	 *根据世界坐标获得网格平面坐标 90度地图的世界坐标和网格坐标相同
	 * @param cx
	 * @param cy
	 * @return 
	 * 
	 */		
	public Point getDerectByWorldPoint(int cx, int cy)
	{
		return new Point(cx,cy);
	}
	
	/**
	 *根据网格平面坐标获得像素坐标 
	 * @param dx
	 * @param dy
	 * @return 
	 * 
	 */	
	public Point getPixelByDerect(int dx, int dy)
	{
		int x = (int) Mathf.Floor((dx + 1) * this._nodeWidth - this._halfNodeWidth);
		int y = (int) Mathf.Floor((dy + 1) * this._nodeHeight - this._halfNodeHeight);
		return new Point(x,y);
	}
}

/**
 *蜂巢式（即正六边形）地图路点处理接口实现 
 * @author Administrator
 * 
 */
class MapRoadHoneycomb : IMapRoad
{
	/**
	 *地图一共分成几行 
	 */		
	private int _row;
	
	/**
	 *地图一共分成几列 
	 */		
	private int _col;
	
	/**
	 *地图路点单元格宽 
	 */		
	private float _nodeWidth;
	
	/**
	 *地图路点单元格高 
	 */		
	private float _nodeHeight;
	
	/**
	 *地图路点单元宽的一半 
	 */	
	private float _halfNodeWidth;
	
	/**
	 *地图路点单元高的一半 
	 */	
	private float _halfNodeHeight;
	
	/**
	 * 六边形直径的4分之一
	 */	
	private float _nwDiv4;
	
	/**
	 * 六边形直径的半径
	 */	
	private float _radius;

	/**
	 * 六边形宽高的tan值，正六边形为1.732
	 */
	private  float _proportion = 1.732f;
	
	/**
	 *蜂巢式（即正六边形）地图路点处理 
	 * @param row
	 * @param col
	 * @param nodeWidth
	 * @param nodeHeight
	 * @param halfNodeWidth
	 * @param halfNodeHeight
	 * 
	 */	
	public MapRoadHoneycomb(int row,int col, float nodeWidth,float nodeHeight,float halfNodeWidth,float halfNodeHeight)
	{
		this._row = row;
		this._col = col;
		this._nodeWidth = nodeWidth;
		//this._nodeHeight = (this._nodeWidth / 2) * 1.732;
		this._nodeHeight = nodeHeight;
		this._halfNodeWidth = halfNodeWidth;
		this._halfNodeHeight = halfNodeHeight;
		
		this._nwDiv4 = this._nodeWidth / 4;
		this._radius = this._nwDiv4 * 4;

		this._proportion = this._nodeHeight * 2 / this._nodeWidth;

	}
	
	/**
	 *根据地图平面像素坐标获得路节点  
	 * @param x 
	 * @param y
	 * @return 
	 * 
	 */		
	public RoadNode getNodeByPixel(float x, float y)
	{
		Point wPoint = this.getWorldPointByPixel(x,y);
		Point fPoint = this.getPixelByWorldPoint(wPoint.x,wPoint.y);
		//var dPoint:Point = getDerectByPixel(x,y);

		RoadNode node = new RoadNode();
		
		node.cx = wPoint.x;
		node.cy = wPoint.y;
		
		node.px = fPoint.x;
		node.py = fPoint.y;
		
		node.dx = wPoint.x;
		node.dy = wPoint.y;
		
		return node;
	}
	
	/**
	 *根据路点平面坐标点获得路节点  
	 * @param px
	 * @param py
	 * @return 
	 * 
	 */			
	public RoadNode getNodeByDerect(int dx, int dy)
	{
		
		Point fPoint = this.getPixelByDerect(dx,dy);
		//Point wPoint = getWorldPointByPixel(fPoint.x,fPoint.y);

		RoadNode node = new RoadNode();
		
		node.cx = (int)dx;
		node.cy = (int)dy;

		node.px = fPoint.x;
		node.py = fPoint.y;
		
		node.dx = (int)dx;
		node.dy = (int)dy;
		
		return node;
	}
	
	/**
	 *根据路点场景世界坐标获得路节点 
	 * @param cx
	 * @param cy
	 * @return 
	 * 
	 */			
	public RoadNode getNodeByWorldPoint(int cx, int cy)
	{
		Point point = this.getPixelByWorldPoint(cx, cy);
		return this.getNodeByPixel(point.x,point.y);
	}
	
	/**
	 *根据像素坐标得到场景世界坐标 
	 * @param x
	 * @param y
	 * @return 
	 * 
	 */		
	public Point getWorldPointByPixel(float x, float y)
	{
		float nwDiv4Index = (float) Mathf.Floor(x / this._nwDiv4);    //六边形的外切矩形竖方向均等分成4分，所有的六边形外切矩形连在一起形成一个个4分之一矩形宽的区域，nwDiv4Index就是该区域的索引
		
		int col = (int) Mathf.Floor(nwDiv4Index / 3);    //取得临时六边形横轴的索引,根据不同的情况可能会变
		
		int row;              //六边形纵轴的索引
		
		int cx;

		int cy;
		
		if((nwDiv4Index - 1) % 6 == 0 || (nwDiv4Index - 2) % 6 == 0)
		{
			row = (int) Mathf.Floor(y / this._nodeHeight);
			cx = col;
			cy = row;
		}else if((nwDiv4Index - 4) % 6 == 0 || (nwDiv4Index - 5) % 6 == 0)
		{
			if(y < this._nodeHeight / 2)
			{
				row = -1;
			}else
			{
				row = (int) Mathf.Floor((y - this._nodeHeight / 2)  / this._nodeHeight);
			}
			cx = col;
			cy = row;
		}else{
			
			
			
			if(col % 2 == 0)
			{
				//(x - 1,y - 1)  (x - 1,y)
				row = (int) Mathf.Floor(y / this._nodeHeight);
				
				if(this.testPointInHoneycomb(col,row,x,y))
				{
					cx = col;
					cy = row;
				}else if(this.testPointInHoneycomb(col - 1,row - 1,x,y))
				{
					cx = col - 1;
					cy = row - 1;
				}else
				{
					cx = col - 1;
					cy = row;
				}
				
			}else
			{
				//(x - 1,y)  (x - 1,y + 1)
				
				if(y < this._nodeHeight / 2)
				{
					row = -1;
				}else
				{
					row = (int) Mathf.Floor((y - this._nodeHeight / 2)  / this._nodeHeight);
				}
				
				if(this.testPointInHoneycomb(col,row,x,y))
				{
					cx = col;
					cy = row;
				}else if(this.testPointInHoneycomb(col - 1,row,x,y))
				{
					cx = col - 1;
					cy = row;
				}else
				{
					cx = col - 1;
					cy = row + 1;
				}
				
			}
			
			
		}
		
		return new Point(cx,cy);
	}
	
	private bool testPointInHoneycomb(int col,int row,float px,float py)
	{
		
		float a = this._nwDiv4 * 2;
		
		Point point = this.getPixelByWorldPoint(col,row);
		
		float absX = Mathf.Abs(px - point.x);
		float absY = Mathf.Abs(py - point.y);
		
		//return a-absX >= absY/(1.732);

		return a - absX >= absY / this._proportion;
	}
	
	/**
	 *根据世界坐标获得像素坐标 
	 * @param cx
	 * @param cy
	 * @return 
	 * 
	 */		
	public Point getPixelByWorldPoint(int cx, int cy)
	{
		int x = (int) Mathf.Floor((2 + 3 * cx)/4 * this._nodeWidth);
		int y = (int) Mathf.Floor((cy + 1/2 *( 1 + (cx % 2))) * this._nodeHeight);

		return new Point(x,y);
	}
	
	/**
	 *根据像素坐标获得网格平面坐标
	 * @param x
	 * @param y
	 * @return 
	 * 
	 */		
	public Point getDerectByPixel(float x, float y)
	{
		
		return this.getWorldPointByPixel(x,y);
	}
	
	/**
	 *根据世界坐标获得网格平面坐标 90度地图的世界坐标和网格坐标相同
	 * @param cx
	 * @param cy
	 * @return 
	 * 
	 */		
	public Point getDerectByWorldPoint(int cx, int cy)
	{
		return new Point(cx,cy);
	}
	
	/**
	 *根据网格平面坐标获得像素坐标 
	 * @param dx
	 * @param dy
	 * @return 
	 * 
	 */	
	public Point getPixelByDerect(int dx, int dy)
	{
		int x = (int)((2 + 3 * dx)/4 * this._nodeWidth);
		int y = (int)((dy + 1/2 *( 1 + (dx % 2))) * this._nodeHeight);
		return new Point(x,y);
	}
}


/**
 *蜂巢式（即正六边形）地图路点处理接口实现 横
 * @author Administrator
 * 
 */
 class MapRoadHoneycomb2 : IMapRoad
 {
	 
	private MapRoadHoneycomb mapRoadHoneycomb;
	 
	 /**
	  *蜂巢式（即正六边形）地图路点处理 
	  * @param row
	  * @param col
	  * @param nodeWidth
	  * @param nodeHeight
	  * @param halfNodeWidth
	  * @param halfNodeHeight
	  * 
	  */	
	 public MapRoadHoneycomb2(int row,int col, float nodeWidth,float nodeHeight, float halfNodeWidth,float halfNodeHeight)
	 {
		 this.mapRoadHoneycomb = new MapRoadHoneycomb(row, col,nodeWidth, nodeHeight, halfNodeWidth, halfNodeHeight);
	 }

	 /**
	  * 转置路节点，即把x,y轴调换过来
	  * @param node 
	  */
	 private RoadNode transposedNode(RoadNode node)
	 {
		var temp_cx = node.cx;
		var temp_dx = node.dx;
		var temp_px = node.px;

		node.cx = node.cy;
		node.cy = temp_cx;

		node.dx = node.dy;
		node.dy = temp_dx;

		node.px = node.py;
		node.py = temp_px;

		return node;
	 }

	 /**
	  * 转置坐标点，即把x,y轴调换过来
	  * @param point 
	  */
	 private Point transposedPoint(Point point)
	 {
		 var temp_x = point.x;
		 point.x = point.y;
		 point.y = temp_x;

		 return point;
	 }
	 
	 /**
	  *根据地图平面像素坐标获得路节点  
	  * @param x 
	  * @param y
	  * @return 
	  * 
	  */		
	 public RoadNode getNodeByPixel(float x, float y)
	 {
		 return this.transposedNode(this.mapRoadHoneycomb.getNodeByPixel(y,x));
	 }
	 
	 /**
	  *根据路点平面坐标点获得路节点  
	  * @param px
	  * @param py
	  * @return 
	  * 
	  */			
	 public RoadNode getNodeByDerect(int dx, int dy)
	 {
		 return this.transposedNode(this.mapRoadHoneycomb.getNodeByDerect(dy,dx));
	 }
	 
	 /**
	  *根据路点场景世界坐标获得路节点 
	  * @param cx
	  * @param cy
	  * @return 
	  * 
	  */			
	 public RoadNode getNodeByWorldPoint(int cx, int cy)
	 {
		 return this.transposedNode(this.mapRoadHoneycomb.getNodeByWorldPoint(cy,cx));
	 }
	 
	 /**
	  *根据像素坐标得到场景世界坐标 
	  * @param x
	  * @param y
	  * @return 
	  * 
	  */		
	 public Point getWorldPointByPixel(float x, float y)
	 {
		 return this.transposedPoint(this.mapRoadHoneycomb.getWorldPointByPixel(y,x));
	 }
	 
	 /**
	  *根据世界坐标获得像素坐标 
	  * @param cx
	  * @param cy
	  * @return 
	  * 
	  */		
	 public Point getPixelByWorldPoint(int cx, int cy)
	 {
		 return this.transposedPoint(this.mapRoadHoneycomb.getPixelByWorldPoint(cy,cx));
	 }
	 
	 /**
	  *根据像素坐标获得网格平面坐标
	  * @param x
	  * @param y
	  * @return 
	  * 
	  */		
	 public Point getDerectByPixel(float x, float y)
	 {
		 
		 return this.transposedPoint(this.mapRoadHoneycomb.getDerectByPixel(y,x));
	 }
	 
	 /**
	  *根据世界坐标获得网格平面坐标 90度地图的世界坐标和网格坐标相同
	  * @param cx
	  * @param cy
	  * @return 
	  * 
	  */		
	 public Point getDerectByWorldPoint(int cx, int cy)
	 {
		 return this.transposedPoint(this.mapRoadHoneycomb.getDerectByWorldPoint(cy,cx));
	 }
	 
	 /**
	  *根据网格平面坐标获得像素坐标 
	  * @param dx
	  * @param dy
	  * @return 
	  * 
	  */	
	 public Point getPixelByDerect(int dx, int dy)
	 {
		 return this.transposedPoint(this.mapRoadHoneycomb.getPixelByDerect(dy,dx));
	 }
 }

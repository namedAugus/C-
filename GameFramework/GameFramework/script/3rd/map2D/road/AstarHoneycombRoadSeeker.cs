
using System;
using System.Collections.Generic;
using UnityEngine;
/**
* 六边形 A*寻路算法 
* @作者 落日故人 QQ 583051842
* 
*/
public class AstarHoneycombRoadSeeker : IRoadSeeker {

	/**
	 * 横向移动一个格子的代价
	 */
	private const int COST_STRAIGHT = 10;

	/**
	 * 斜向移动一个格子的代价
	 */
	private const int COST_DIAGONAL = 10;

	/**
	 *最大搜寻步骤数，超过这个值时表示找不到目标 
	 */
	private int maxStep = 1000;

	/**
	 * 开启列表
	 */
	private List<RoadNode> _openlist = new List<RoadNode>();

	/**
	 *关闭列表 
		*/
	private List<RoadNode> _closelist = new List<RoadNode>();

	/**
	 * 二叉堆存储结构
	 */
	private BinaryTreeNode _binaryTreeNode = new BinaryTreeNode();
	
	/**
	 *开始节点 
		*/		
	private RoadNode _startNode;
	
	/**
	 *当前检索节点 
		*/		
	private RoadNode _currentNode;
	
	/**
	 *目标节点 
	 */		
	private RoadNode _targetNode;
	
	/**
	*地图路点数据 
	*/		
	private Dictionary<string, RoadNode> _roadNodes;
	
	/**
	 *用于检索一个节点周围6个点的向量数组 格子列数为偶数时使用
	 */		
	private int[,]_round1 = { { 0, -1 }, { 1, -1 }, { 1, 0 }, { 0, 1 }, { -1, 0 }, { -1, -1 } };
	/**
	 *用于检索一个节点周围6个点的向量数组 格子列数为奇数时使用
	 */
	private int[,] _round2 = { { 0,-1 }, { 1,0 }, { 1,1 }, { 0,1 }, { -1,1 }, { -1, 0 } };

	/**
     * 要检索的周围邻居格子方向向量，当要按角色占据的路点面积进行寻路时使用 (备注，是用六边形格子坐标hx,hy检测邻居路点方向向量，不用寻路的cx,cy坐标)
     */
	private List<List<int>> _neighbours = null;

	/**
	 * 存放检索周围邻居的方向向量的字典
	 */
	private Dictionary<int, List<List<int>>> _neighboursDic = new Dictionary<int, List<List<int>>>();

	private int handle = -1;

	/**
     * 优化类型，默认使用最短路径的优化
     */
	private PathOptimize _pathOptimize = PathOptimize.best;

	/**
     * 定义一个路点是否能通过，如果是null，则用默认判断条件
     */
	private IRoadSeeker.PassTest _isPassCallBack = null;

	public AstarHoneycombRoadSeeker(Dictionary<string, RoadNode> roadNodes)
	{
		this._roadNodes = roadNodes;
	}

	/**
     * 设置最大寻路步骤
     * @param maxStep 
     */
	public void setMaxSeekStep(int maxStep)
	{
		this.maxStep = maxStep;
	}

	/**
	 * 设置路径优化等级
	 * @param optimize 
	 */
	public void setPathOptimize(PathOptimize optimize)
	{
		this._pathOptimize = optimize;
	}

	/**
     * 设置4方向路点的寻路类型 （这个函数不会用于六边形寻路，只是用来兼容寻路接口，防止报异常）
     * @param pathQuadSeek 
     */
	public void setPathQuadSeek(PathQuadSeek pathQuadSeek)
	{
		
	}

	/**
     * 定义一个路点是否能通过，如果参数是null，则用默认判断条件
     * @param callback 
     */
	public void setRoadNodePassCondition(IRoadSeeker.PassTest callback) 
	{
		this._isPassCallBack = callback;
	}

	/**
	 *寻路入口方法 
	 * @param startNode
	 * @param targetNode
	 * @param radius
	 * @return 
	 */
	public List<RoadNode> seekPath(RoadNode startNode, RoadNode targetNode, int radius)
	{
		this._startNode = startNode;
		this._currentNode = startNode;
		this._targetNode = targetNode;

		this._neighbours = this.getNeighbours(radius);
		
		if(this._startNode == null || this._targetNode == null)
			return new List<RoadNode>();

		if (this._startNode == this._targetNode)
		{
			List<RoadNode> list = new List<RoadNode>();
			list.Add(this._targetNode);
			return list;
		}

		if(!this.isCanPass(this._targetNode))
		{
			PathLog.log("目标不可达到：");
			return new List<RoadNode>();
		}
			
		this._startNode.g = 0; //重置起始节点的g值
		this._startNode.resetTree(); //清除起始节点原有的二叉堆关联关系

		this._binaryTreeNode.refleshTag(); //刷新二叉堆tag，用于后面判断是不是属于当前次的寻路
										   //this._binaryTreeNode.addTreeNode(this._startNode); //把起始节点设置为二叉堆结构的根节点

		int step = 0;

		while (true)
		{
			if(step > this.maxStep)
			{
				PathLog.log("没找到目标计算步骤为：",step);
				return new List<RoadNode>();
			}
			
			step++;
			
			this.searchRoundNodes(this._currentNode);
			
			if(this._binaryTreeNode.isTreeNull()) //二叉堆树里已经没有任何可搜寻的点了，则寻路结束，每找到目标
			{
				PathLog.log("没找到目标计算步骤为：",step);
				return new List<RoadNode>();
			}

			this._currentNode = this._binaryTreeNode.getMin_F_Node();
			
			if(this._currentNode == this._targetNode)
			{
				PathLog.log("找到目标计算步骤为：",step);
				return this.getPath();
			}else
			{
				this._binaryTreeNode.setRoadNodeInCloseList(this._currentNode);//打入关闭列表标记
			}
			
		}

		return new List<RoadNode>();
	}


	/**
	 *寻路入口方法 如果没有寻到目标，则返回离目标最近的路径
	 * @param startNode
	 * @param targetNode
	 * @param radius
	 * @return 
	 */
	public List<RoadNode> seekPath2(RoadNode startNode, RoadNode targetNode, int radius = 0)
	{
		this._startNode = startNode;
		this._currentNode = startNode;
		this._targetNode = targetNode;
		
		this._neighbours = this.getNeighbours(radius);

		if(this._startNode == null || this._targetNode == null)
			return new List<RoadNode>();
		
		if(this._startNode == this._targetNode)
		{
			List<RoadNode> list = new List<RoadNode>();
			list.Add(this._targetNode);
			return list;
		}

		int newMaxStep = this.maxStep;

		if(!this.isCanPass(this._targetNode))
		{
			//如果不能直达目标，最大寻路步骤 = 为两点间的预估距离的3倍
			newMaxStep = (Mathf.Abs(this._targetNode.cx - this._startNode.cx) + Mathf.Abs(this._targetNode.cy - this._startNode.cy)) * 3;
			if(newMaxStep > this.maxStep)
			{
				newMaxStep = this.maxStep;
			}
		}
			
		this._startNode.g = 0; //重置起始节点的g值
		this._startNode.resetTree(); //清除起始节点原有的二叉堆关联关系

		this._binaryTreeNode.refleshTag(); //刷新二叉堆tag，用于后面判断是不是属于当前次的寻路
		//this._binaryTreeNode.addTreeNode(this._startNode); //把起始节点设置为二叉堆结构的根节点
		
		int step = 0;

		RoadNode closestNode = null; //距离目标最近的路点

		while(true)
		{
			if(step > newMaxStep)
			{
				PathLog.log("没找到目标计算步骤为：",step);
				return this.seekPath(startNode,closestNode,radius);
			}
			
			step++;
			
			this.searchRoundNodes(this._currentNode);
			
			if(this._binaryTreeNode.isTreeNull()) //二叉堆树里已经没有任何可搜寻的点了，则寻路结束，没找到目标
			{
				PathLog.log("没找到目标计算步骤为：",step);
				return this.seekPath(startNode,closestNode,radius);
			}
			
			this._currentNode = this._binaryTreeNode.getMin_F_Node();

			if(closestNode == null)
			{
				closestNode = this._currentNode;
			}else
			{
				if(this._currentNode.h < closestNode.h)
				{
					closestNode = this._currentNode;
				}
			}
			
			if(this._currentNode == this._targetNode)
			{
				PathLog.log("找到目标计算步骤为：",step);
				return this.getPath();
			}else
			{
				this._binaryTreeNode.setRoadNodeInCloseList(this._currentNode);//打入关闭列表标记
			}
			
		}
		
		return this.seekPath(startNode,closestNode,radius);
	}

	/**
	 *获得最终寻路到的所有路点 
		* @return 
		* 
		*/
	private List<RoadNode> getPath()
	{
		List<RoadNode> nodeArr = new List<RoadNode>();

		RoadNode node = this._targetNode;
		
		while(node != this._startNode)
		{
			nodeArr.Insert(0, node);
			node = node.parent;
		}

		nodeArr.Insert(0, _startNode);

		//如果不优化，则直接返回完整寻路路径
		if (this._pathOptimize == PathOptimize.none)
        {
            return nodeArr;
        }

		//把多个节点连在一起的，横向或者斜向的一连串点，除两边的点保留

		RoadNode preNode;
		RoadNode midNode;
		RoadNode nextNode;

		HoneyPoint preHpos;
		HoneyPoint midHpos;
		HoneyPoint nextHpos;

		//var hround:number[][] = [[-1,-1],[-1,0],[0,1],[1,1],[1,0],[0,-1]];
		//var hround2:number[][] = [[-2,-1],[-1,1],[1,2],[2,1],[1,-1],[-1,-2]];
		int i = 0;
		//第一阶段优化： 对横，竖，正斜进行优化
		for (i = 1 ; i < nodeArr.Count - 1 ; i++)
		{
			preNode = nodeArr[i - 1] as RoadNode;
			midNode = nodeArr[i] as RoadNode;
			nextNode = nodeArr[i + 1] as RoadNode;

			preHpos = this.getHoneyPoint(preNode);
			midHpos = this.getHoneyPoint(midNode);
			nextHpos = this.getHoneyPoint(nextNode);
			
			bool bool1 = midNode.cx == preNode.cx && midNode.cx == nextNode.cx;
			
			bool bool2 = (midNode.cy == preNode.cy && midNode.cy == nextNode.cy) && ((preNode.cx % 2 == midNode.cx % 2 && midNode.cx % 2 == nextNode.cx % 2) );
			
			bool bool3 = preHpos.hx == midHpos.hx  && midHpos.hx == nextHpos.hx;
			
			bool bool4 = preHpos.hy == midHpos.hy  && midHpos.hy == nextHpos.hy;
			
			if(bool1 || bool2 || bool3 || bool4)
			{
				// nodeArr.splice(i,1)
				nodeArr.RemoveAt(i);
				i--;
			}
		}

		//如果只需要优化到第一阶段，则直接返回第一阶段的优化结果
		if(this._pathOptimize == PathOptimize.better)
        {
            return nodeArr;
        }

		//第二阶段优化：对不在横，竖，正斜的格子进行优化
		for(i = 0 ; i < nodeArr.Count - 2 ; i++)
		{
			RoadNode startNode = nodeArr[i];
			RoadNode optimizeNode = null;

			//优先从尾部对比，如果能直达就把中间多余的路点删掉
			int j = 0;
			for (j = nodeArr.Count - 1 ; j > i + 1 ; j--)
			{
				RoadNode targetNode = nodeArr[j] as RoadNode;

				if(this.isArriveBetweenTwoNodes(startNode,targetNode))
				{
					optimizeNode = targetNode;
					break;
				}

			}

			if(optimizeNode != null)
			{
				int optimizeLen = j - i - 1;
				// nodeArr.splice(i + 1,optimizeLen);
				nodeArr.RemoveRange(i + 1, optimizeLen);
			}
		
		}

		return nodeArr;
	}

	/**
	 * 两点之间是否可到达
	 */
	public bool isArriveBetweenTwoNodes(RoadNode startNode, RoadNode targetNode)
	{
		HoneyPoint startPoint = this.getHoneyPoint(startNode);
		HoneyPoint targetPoint = this.getHoneyPoint(targetNode);

		if(startPoint.hx == targetPoint.hx && startPoint.hy == targetPoint.hy)
		{
			return false;
		}

		int disX = Mathf.Abs(targetPoint.hx - startPoint.hx);
		int disY = Mathf.Abs(targetPoint.hy - startPoint.hy);

		var dirX = 0;

		if(targetPoint.hx > startPoint.hx)
		{
			dirX = 1;
		}else if(targetPoint.hx < startPoint.hx)
		{
			dirX = -1;
		}

		var dirY = 0;

		if(targetPoint.hy > startPoint.hy)
		{
			dirY = 1;
		}else if(targetPoint.hy < startPoint.hy)
		{
			dirY = -1;
		}

		float rx = 0;
		float ry = 0;
		int intNum = 0;
		float decimalNum = 0;

		if(disX > disY)
		{
			float rate = (float)disY / (float)disX;

			for(var i = 0 ; i < disX ; i += 2)
			{
				ry = i * dirY * rate;
				intNum = (int)(dirY > 0 ? Mathf.Floor(startPoint.hy + ry) : Mathf.Ceil(startPoint.hy + ry));
				decimalNum = Mathf.Abs(ry % 1);

				HoneyPoint hpoint1 = new HoneyPoint();
				hpoint1.hx = startPoint.hx + i * dirX;
				hpoint1.hy = decimalNum <= 0.5 ? intNum : intNum + 1 * dirY;

				//cc.log(i + "  :: " ,hpoint1.hx, hpoint1.hy," yy ",startPoint.hy + i * dirY * rate,ry % 1,rate,intNum,decimalNum,dirY,ry);

				ry = (i + 1) * dirY * rate;
				intNum = (int)(dirY > 0 ? Mathf.Floor(startPoint.hy + ry) : Mathf.Ceil(startPoint.hy + ry));
				decimalNum = Mathf.Abs(ry % 1);

				HoneyPoint hpoint2 = new HoneyPoint();
				hpoint2.hx = startPoint.hx + (i + 1) * dirX;
				hpoint2.hy = decimalNum <= 0.5 ? intNum : intNum + 1 * dirY;
				
				ry = (i + 2) * dirY * rate;
				intNum = (int)(dirY > 0 ? Mathf.Floor(startPoint.hy + ry) : Mathf.Ceil(startPoint.hy + ry));
				decimalNum = Mathf.Abs(ry % 1);

				HoneyPoint hpoint3 = new HoneyPoint();
				hpoint3.hx = (int)(startPoint.hx + (i + 2) * dirX);
				hpoint3.hy = decimalNum <= 0.5 ? intNum : intNum + 1 * dirY;

				if(!this.isCrossAtAdjacentNodes(startPoint,targetPoint,hpoint1,hpoint2,hpoint3))
				{
					return false;
				}

			}

		}else
		{
			float rate = disX / disY;

			for(var i = 0 ; i < disY ; i += 2)
			{
				rx = i * dirX * rate;
				intNum = (int)(dirX > 0 ? Mathf.Floor(startPoint.hx + rx) : Mathf.Ceil(startPoint.hx + rx));
				decimalNum = Mathf.Abs(rx % 1);

				HoneyPoint hpoint1 = new HoneyPoint();
				hpoint1.hx = decimalNum <= 0.5 ? intNum : intNum + 1 * dirX;
				hpoint1.hy = startPoint.hy + i * dirY;

				rx = (i + 1) * dirX * rate;
				intNum = (int)(dirX > 0 ? Mathf.Floor(startPoint.hx + rx) : Mathf.Ceil(startPoint.hx + rx));
				decimalNum = Mathf.Abs(rx % 1);

				HoneyPoint hpoint2 = new HoneyPoint();
				hpoint2.hx = decimalNum <= 0.5 ? intNum : intNum + 1 * dirX;
				hpoint2.hy = startPoint.hy + (i + 1) * dirY;
				
				rx = (i + 2) * dirX * rate;
				intNum = (int)(dirX > 0 ? Mathf.Floor(startPoint.hx + rx) : Mathf.Ceil(startPoint.hx + rx));
				decimalNum = Mathf.Abs(rx % 1);

				HoneyPoint hpoint3 = new HoneyPoint();
				hpoint3.hx = decimalNum <= 0.5 ? intNum : intNum + 1 * dirX;
				hpoint3.hy = startPoint.hy + (i + 2) * dirY;

				if(!this.isCrossAtAdjacentNodes(startPoint,targetPoint,hpoint1,hpoint2,hpoint3))
				{
					return false;
				}
			}
		}

		return true;
	}

	/**
     * 判断三个相邻的点是否可通过
     * @param node1 
     * @param node2 
     */
    private bool isCrossAtAdjacentNodes(HoneyPoint startPoint, HoneyPoint targetPoint, HoneyPoint hpoint1, HoneyPoint hpoint2, HoneyPoint hpoint3)
    {
		RoadNode node1 = this.getNodeByHoneyPoint(hpoint1.hx,hpoint1.hy);
		RoadNode node2 = this.getNodeByHoneyPoint(hpoint2.hx,hpoint2.hy);
		RoadNode node3 = this.getNodeByHoneyPoint(hpoint3.hx,hpoint3.hy); //节点3主要用做路径方向的判断

        if(node1 == node2)
        {
            return false;
        }

        //前两个点只要有一个点不能通过就不能通过，节点3只做方向向导，不用考虑是否可通过和是否存在
        if(!this.isPassNode(node1) || !this.isPassNode(node2))
        {
            return false;
        }

		//按寻路面积检测两个点只要有一个点不能通过就不能通过
        if(!this.isCanPass(node1) || !this.isCanPass(node2))
        {
            return false;
        }

        int dirX1 = hpoint1.hx - hpoint2.hx;
        int dirY1 = hpoint1.hy - hpoint2.hy;

        int dirX2 = hpoint3.hx - hpoint2.hx;
		int dirY2 = hpoint3.hy - hpoint2.hy;
		
		//hround:number[][] = [[-1,-1],[-1,0],[0,1],[1,1],[1,0],[0,-1]]; //相邻点向量 
		//[-1,1] [1,-1] //特殊相邻点向量

        //如果不是相邻的两个点 则不能通过
        if((Mathf.Abs(dirX1) > 1 || Mathf.Abs(dirY1) > 1) || (Mathf.Abs(dirX2) > 1 || Mathf.Abs(dirY2) > 1))
        {
            return false;
        }

        //特殊相邻点 特殊对待
        if(dirX1 == -dirY1) //如果第一个点和第二个点是特殊相邻点
        {
            if(dirX1 == -1)
            {
                if(!this.isPassNode(this.getNodeByHoneyPoint(hpoint2.hx -1,hpoint2.hy)) || !this.isPassNode(this.getNodeByHoneyPoint(hpoint2.hx,hpoint2.hy + 1)))
                {
                    return false;
                }
            }else
            {
                if(!this.isPassNode(this.getNodeByHoneyPoint(hpoint2.hx + 1,hpoint2.hy)) || !this.isPassNode(this.getNodeByHoneyPoint(hpoint2.hx,hpoint2.hy - 1)))
                {
                    return false;
                }
            }
		}
		
		//第一个点和第二个点已经可通过，如果第二个点是终点，那么可直达
		if(hpoint2.hx == targetPoint.hx && hpoint2.hy == targetPoint.hy) 
        {
            return true;
        }

		//特殊相邻点 特殊对待
        if(dirX2 == -dirY2) //如果第二个点和第三个点是特殊相邻点 
        {
            if(dirX2 == -1)
            {
                if(!this.isPassNode(this.getNodeByHoneyPoint(hpoint2.hx -1,hpoint2.hy)) || !this.isPassNode(this.getNodeByHoneyPoint(hpoint2.hx,hpoint2.hy + 1)))
                {
                    return false;
                }
            }else
            {
                if(!this.isPassNode(this.getNodeByHoneyPoint(hpoint2.hx + 1,hpoint2.hy)) || !this.isPassNode(this.getNodeByHoneyPoint(hpoint2.hx,hpoint2.hy - 1)))
                {
                    return false;
                }
            }
        }

        //如果相邻的点和目标点在同一直线
        if(hpoint1.hx == hpoint2.hx  && hpoint2.hx == hpoint3.hx)
        {
            return true;
        }
        
        //var hround2:number[][] = [[-2,-1],[-1,1],[1,2],[2,1],[1,-1],[-1,-2]];

        if(this.isPassNode(this.getNodeByHoneyPoint(hpoint2.hx + (dirX1 + dirX2),hpoint2.hy + (dirY1 + dirY2))))
        {
            return true;
        }

        return false;

    }

	/**
	 * 获得六边形格子坐标（以正斜角和反斜角为标准的坐标）
	 * @param node 
	 */
	public HoneyPoint getHoneyPoint(RoadNode node)
	{
		int hx = (int)(node.cy + Mathf.Ceil(node.cx / 2)); //设置反斜角为x坐标
		int hy = (int)(node.cy - Mathf.Floor(node.cx / 2)); //设置正斜角为y坐标
		
		return new HoneyPoint(hx,hy);
	}

	/**
	 * 根据六边形格子坐标获得路节点
	 * @param hx 
	 * @param hy 
	 * @returns 
	 */
	public RoadNode getNodeByHoneyPoint(int hx,int hy)
	{
		int cx = hx - hy; //研究出来的
		int cy = (int)(Mathf.Floor((hx - hy) / 2) + hy); //研究出来的

		return this.getRoadNode(cx,cy);
	}

	/**
	 * 获得一个点周围指定方向相邻的一个点
	 * @param node 制定的点
	 * @param roundIndex 0是下，然后顺时针，5右下
	 */
	public RoadNode getRoundNodeByIndex(RoadNode node, int roundIndex)
	{
		if(node == null)
		{
			return null;
		}

		roundIndex = roundIndex % 6;

		int[,] round = null;

		round = (node.cx % 2 == 0) ? this._round1 : this._round2;

		int cx = node.cx + round[roundIndex,0];
		int cy = node.cy + round[roundIndex,1];

		return this.getRoadNode(cx,cy);
		
	}
	
	/**
	 * 获得一个点周围所有的相邻点
	 * @param node 
	 */
	public List<RoadNode>  getRoundNodes(RoadNode node)
	{
		int[,] round = null;

		round = (node.cx % 2 == 0) ? this._round1 : this._round2;

		List<RoadNode> nodeArr = new List<RoadNode>();

		for(int i = 0 ; i < round.GetLength(0); i++)
		{
			int cx = node.cx + round[i,0];
			int cy = node.cy + round[i,1];

			RoadNode node2 = this.getRoadNode(cx,cy);

			nodeArr.Add(node2);
		}

		return nodeArr;
	}

	/**
	 * 是否是可通过的点 
	 * @param node 
	 */
	public bool isPassNode(RoadNode node)
	{
		if(this._isPassCallBack != null)
		{
			return this._isPassCallBack(node);
		}

		if(node == null || node.value == 1)
		{
			return false;
		}

		return true;
	}

	/**
     * 是否能通过这个节点
     * @param node 
     * @returns 
     */
	public bool isCanPass(RoadNode node)
	{
		if(!this.isPassNode(node)) //如果当前路点不能通过就不是可通过的点
		{
			return false;
		}

		//------------------------以下逻辑是检测这个点周围的邻居路点是否可通过----------------------------------

		if(this._neighbours == null || this._neighbours.Count == 0) //证明寻路的角色没占据其周围的路点，只有自身脚下的单个路点。直接可过
		{
			return true;
		}

		HoneyPoint honeyPoint = this.getHoneyPoint(node); //_neighbours是用六边形格子坐标hx,hy，不是用寻路的cx，cy坐标，所以要转换

		for(int i = 0 ; i < this._neighbours.Count ; i++)
		{
			int hx = honeyPoint.hx + this._neighbours[i][0];
			int hy = honeyPoint.hy + this._neighbours[i][1];
			RoadNode node2 = this.getNodeByHoneyPoint(hx,hy);

			if(!this.isPassNode(node2))
			{
				return false;
			}
		}

		return true;
	}

	/**
	 * 根据世界坐标获得路节点
	 * @param cx 
	 * @param cy 
	 * @returns 
	 */
	public RoadNode getRoadNode(int cx, int cy)
    {
		string key = cx + "_" + cy;
		if (this._roadNodes.ContainsKey(key))
		{
			return this._roadNodes[cx + "_" + cy];
		}
		return null;
    }

	/**
     * 根据半径获当前位置周围的所有邻居方向向量
     * @param radius 
     * @returns 
     */
	public List<List<int>>  getNeighbours(int radius)
	{
		if(radius == 0)
		{
			return null;
		}

		List<List<int>> neighbours = null;

		if(this._neighboursDic[radius] != null)
		{
			neighbours = this._neighboursDic[radius];

		}else
		{
			neighbours = new List<List<int>>();

			Dictionary<string, bool> hasCheckDic = new Dictionary<string, bool>();
			Dictionary<string, bool> hasSaveDic = new Dictionary<string, bool>();
			int[,] hround = { { -1, -1 }, { -1, 0 }, { 0, 1 }, { 1, 1 }, { 1, 0 }, { 0, -1 } }; //相邻点向量 

			//----------------------------------------------------------------------------------//
			//hround:number[][] = [[-1,-1],[-1,0],[0,1],[1,1],[1,0],[0,-1]]; //相邻点向量 
			//[-1,1] [1,-1] //特殊相邻点向量
			//----------------------------------------------------------------------------------//

			this.getNeighboursRecursive(0,0,radius,1,neighbours,hasCheckDic,hasSaveDic,hround);

			this._neighboursDic[radius] = neighbours;
		}

		return neighbours;
	}

	/**
	 * 以递归的方式获得周围邻居方向向量
	 * @param hx 检测坐标x
	 * @param hy 检测坐标y
	 * @param radius 检测的半径
	 * @param checkRadius 当前正在检测的半径
	 * @param outNeighbours 返回的所有的邻居方向向量
	 * @param hasCheckDic 用来保存是否已经检测过的坐标标记
	 * @param hasSaveDic 用来保存是否已经检保存过的坐标标记
	 * @param hround 检测周围用的方向向量
	 * @returns 
	 */
	private void getNeighboursRecursive(int hx,int hy,int radius,int checkRadius, List<List<int>> outNeighbours,
		Dictionary<string, bool> hasCheckDic, Dictionary<string, bool>  hasSaveDic, int[,] hround)
	{
		if(checkRadius > radius)
		{
			return;
		}

		string key = hx + "_" + hy;
		hasCheckDic[key] = true;

		int len = hround.Length;

		for(var i = 0; i < len ; i++)
		{
			int newHx = hx + hround[i,0];
			int newHy = hy + hround[i,1];

			if(newHx == 0 && newHy == 0)
			{
				continue;
			}

			key = newHx + "_" + newHy;
			if(hasCheckDic[key])
			{
				continue;
			}

			if(!hasSaveDic[key])
			{
				List<int> v = new List<int>();
				v.Add(newHx);
				v.Add(newHy);
				outNeighbours.Add(v);
				hasSaveDic[key] = true;
			}

			this.getNeighboursRecursive(newHx,newHy,radius,checkRadius + 1,outNeighbours,hasCheckDic,hasSaveDic,hround);
		}
	}

	/**
	 *测试寻路步骤 
	 * @param startNode
	 * @param targetNode
	 * @param radius
	 * @return 
	 */		
	public void testSeekPathStep(RoadNode startNode, RoadNode targetNode, int radius,Action callback,object target,float time = 100)
	{
		/*this._startNode = startNode;
		this._currentNode = startNode;
		this._targetNode = targetNode;

		this._neighbours = this.getNeighbours(radius);
		
		if(!this.isCanPass(this._targetNode))
			return;
		
		this._startNode.g = 0; //重置起始节点的g值
		this._startNode.resetTree(); //清除起始节点原有的二叉堆关联关系

		this._binaryTreeNode.refleshTag(); //刷新二叉堆tag，用于后面判断是不是属于当前次的寻路
		//this._binaryTreeNode.addTreeNode(this._startNode); //把起始节点设置为二叉堆结构的根节点
		
		this._closelist = [];

		var step:number = 0;
		
		clearInterval(this.handle);
		this.handle = setInterval(()=>{

			if(step > this.maxStep)
			{
				PathLog.log("没找到目标计算步骤为：",step);
				clearInterval(this.handle);
				return;
			}
			
			step++;
			
			this.searchRoundNodes(this._currentNode);
			
			if(this._binaryTreeNode.isTreeNull()) //二叉堆树里已经没有任何可搜寻的点了，则寻路结束，每找到目标
			{
				PathLog.log("没找到目标计算步骤为：",step);
				clearInterval(this.handle);
				return;
			}

			this._currentNode = this._binaryTreeNode.getMin_F_Node();
			
			if(this._currentNode == this._targetNode)
			{
				PathLog.log("找到目标计算步骤为：",step);
				clearInterval(this.handle);

				this._openlist = this._binaryTreeNode.getOpenList();
				callback.apply(target,[this._startNode,this._targetNode,this._currentNode,this._openlist,this._closelist,this.getPath()]);
			}else
			{
				this._binaryTreeNode.setRoadNodeInCloseList(this._currentNode);//打入关闭列表标记
				this._openlist = this._binaryTreeNode.getOpenList();
				this._closelist.push(this._currentNode);
				callback.apply(target,[this._startNode,this._targetNode,this._currentNode,this._openlist,this._closelist,null]);
			}

		},time);
		*/
	}
	
	/**
	 *查找一个节点周围可通过的点 
	 * @param node
	 * @return 
	 */		
	private void searchRoundNodes(RoadNode node)
	{
		
		int[,] round = null;

		round = (node.cx % 2 == 0) ? this._round1 : this._round2;

		
		for(int i = 0 ; i < round.GetLength(0); i++)
		{
			int cx = node.cx + round[i,0];
			int cy = node.cy + round[i,1];
			RoadNode node2 = this.getRoadNode(cx,cy);
			
			if(this.isPassNode(node2) && this.isCanPass(node2) && node2 != this._startNode  && !this.isInCloseList(node2))
			{
				this.setNodeF(node2);
			}
		}
	}
	
	/**
	 *设置节点的F值 
	 * @param node
	 */		
	public void setNodeF(RoadNode node)
	{	
		int g;
		
		if(node.cx == this._currentNode.cx || node.cy == this._currentNode.cy)
		{
			g = this._currentNode.g + COST_STRAIGHT;
		}else
		{
			g = this._currentNode.g + COST_DIAGONAL;
		}
		
		if(this.isInOpenList(node))
		{
			if(g < node.g)
			{
				node.g = g;

				node.parent = this._currentNode;
				node.h = (Mathf.Abs(this._targetNode.cx - node.cx) + Mathf.Abs(this._targetNode.cy - node.cy)) * COST_STRAIGHT;
				node.f = node.g + node.h;

				//节点的g值已经改变，把节点先从二堆叉树结构中删除，再重新添加进二堆叉树
				this._binaryTreeNode.removeTreeNode(node); 
				this._binaryTreeNode.addTreeNode(node);

			}
		}else
		{
			node.g = g;
			
			this._binaryTreeNode.setRoadNodeInOpenList(node);//给节点打入开放列表的标志
			node.resetTree();

			node.parent = this._currentNode;
			node.h = (Mathf.Abs(this._targetNode.cx - node.cx) + Mathf.Abs(this._targetNode.cy - node.cy)) * COST_STRAIGHT;
			node.f = node.g + node.h;

			this._binaryTreeNode.addTreeNode(node);
		}
		
		
	}
	
	/**
	 *节点是否在开启列表 
	 * @param node
	 * @return 
	 */		
	private bool isInOpenList(RoadNode node)
	{
		return this._binaryTreeNode.isInOpenList(node);
	}
	
	/**
	 * 节点是否在关闭列表
	 * @param node 
	 * @returns 
	 */	
	private bool isInCloseList(RoadNode node)
	{
		return this._binaryTreeNode.isInCloseList(node);
	}
	
	/**
	 * 释放资源
	 */
	public void dispose()
	{
		this._roadNodes = null;
		this._round1 = null;
		this._round2 = null;
	}
}

/**
 * 六边形格子坐标（以正斜角和反斜角为标准的坐标）
 */
public class HoneyPoint
{
	public int hx = 0;
	public int hy = 0;

	public HoneyPoint(int x = 0, int y = 0)
	{
		this.hx = x;
		this.hy = y;
	}
}
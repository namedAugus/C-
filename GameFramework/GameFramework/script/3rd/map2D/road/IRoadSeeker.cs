

using System;
using System.Collections.Generic;
/**
* 寻路接口
* @作者 落日故人 QQ 583051842
* 
*/
public interface IRoadSeeker {
    public delegate bool PassTest(RoadNode node);

    /**
     *寻路入口方法 
     * @param startNode 寻路起始路点
     * @param targetNode 寻路目标路点
     * @param radius 寻路半径，以路点为单位，0代表忽略半径，n代表以寻露点为中心，向周围扩展n个路点单位用于检测是否可以寻路通过
     * @returns 
     */
    List<RoadNode> seekPath(RoadNode startNode, RoadNode targetNode, int radius);

    /**
     *寻路入口方法 如果没有寻到目标，则返回离目标最近的路径
     * @param startNode 寻路起始路点
     * @param targetNode 寻路目标路点
     * @param radius 寻路半径，以路点为单位，0代表忽略半径，n代表以寻露点为中心，向周围扩展n个路点单位用于检测是否可以寻路通过
     * @returns 
     */
    List<RoadNode>  seekPath2(RoadNode startNode, RoadNode targetNode, int radius);

    /**
     *测试寻路步骤 
     * @param startNode 寻路起始路点
     * @param targetNode 寻路目标路点
     * @param radius 寻路半径，以路点为单位，0代表忽略半径，n代表以寻露点为中心，向周围扩展n个路点单位用于检测是否可以寻路通过
     * @returns
     */
    void testSeekPathStep(RoadNode startNode, RoadNode targetNode,int radius, Action callback,object target,float time);


    /**
     * 两个路点之间是否可直达（即是否可以两点一线到达）
     * @param startNode 
     * @param targetNode 
     */
    bool isArriveBetweenTwoNodes(RoadNode startNode, RoadNode targetNode);

    /**
	 * 是否是可通过的点 
	 * @param node 
	 */
    bool isPassNode(RoadNode node);

    /**
	 * 根据世界坐标获得路节点，（世界坐标即为寻路用的路点坐标）
	 * @param cx 世界坐标X
	 * @param cy 世界坐标Y
	 * @returns 
	 */
    RoadNode getRoadNode(int cx, int cy);

    /**
     * 设置最大寻路步骤，寻路时超过这个寻路步骤还没寻到终点，则视为无法达到目标，寻路结束。
     * 
     * 默认值为1000，值越大，后面寻路的运算消耗越大，所以设置这个值要量力而行。
     * @param maxStep 最大寻路步骤
     */
    void setMaxSeekStep(int maxStep);

    /**
     * 寻路的路径的优化等级。
     * 
     * 有none(不优化),better（较好的优化）,best（最好的优化），每种优化都有特定方面的用途。默认值为best
     * 
     * 每种优化的原理和用途可查看枚举PathOptimize的脚本，里面有详细解释。
     * @param optimize 路径优化等级
     */
    void setPathOptimize(PathOptimize optimize);

    /**
     * 设置4方向路点地图的寻路类型，只针对45度和90度地图，不包括六边形地图（六边形地图设置这个值无任何反应）
     * @param pathQuadSeek  4方向路点地图的寻路类型
     */
    void setPathQuadSeek(PathQuadSeek pathQuadSeek);

    /**
     * 自定义路点是否能通过的条件，
     * @param callback 回调方法用于自定义路点是否可通过的条件，如果参数是null，则用默认判断条件，不是null，则取代默认判断条件
     */
    void setRoadNodePassCondition(PassTest callback);
    
    
}


using System;

[Serializable]
public class MapData {

    /**
     * 地图名称
     */
    public string name = "";

    /**
     * 地图背景资源名
     */
    public string bgName = "";

    /**
     * 地图类型
     */
    public MapType type = MapType.angle45;

    /**
     * 地图宽
     */
    public int mapWidth = 0;

    /**
     * 地图高
     */
    public int mapHeight = 0;

    /**
     * 地图路点宽
     */
    public int nodeWidth = 0;
    
    /**
     * 地图路点高
     */
    public int nodeHeight = 0;

    /**
     * 地图路点数据
     */
    public int[][] roadDataArr = null;

    /**
     * 地图上编辑的所有元素（npc，怪，传送门等）
     */
    public object[] mapItems = null;

    /**
     * 对齐方式：0，左下角; 1,左上角
     */
    public int alignment = 0; 

    /**
     * 偏移量X
     */
    public int offsetX = 0;
    
    /**
     * 偏移量Y
     */
    public int offsetY = 0;    


}



// using UnityEngine;
/**
* 地图参数
*/
class MapParams
{
    /**
     * 地图名称
     */
    public string name = "";

    /**
     * 底图资源名称
     */
    public string bgName = "";

    /**
     * 地图类型
     */
    public MapType mapType = MapType.angle45;

    /**
     * 地图宽
     */
    public int mapWidth = 750;

    /**
     * 地图高
     */
    public int mapHeight = 1600;

    /**
     * 地图单元格宽
     */
    public int ceilWidth = 75;

    /**
     * 地图单元格高
     */
    public int ceilHeight = 75;

    /**
     * 地图视野宽
     */
    public int viewWidth = 750;

    /**
     * 地图视野高
     */
    public int viewHeight = 1334;

    /**
     * 地图切片宽
     */
    public int sliceWidth = 256;

    /**
     * 地图切片高
     */
    public int sliceHeight = 256;

    /**
     * 底图加载模式，是单张还是切片加载
     */
    public MapLoadModel mapLoadModel = MapLoadModel.single;

    /**
     * 地图底图
     */
    // public Texture2D bgTex = null;


}

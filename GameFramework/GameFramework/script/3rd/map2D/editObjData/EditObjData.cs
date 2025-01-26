/**
 * 地图编辑物体对象的数据结构 （基类）
 */
class EditObjData  {

    /**
     * 物体Id
     */
    public string objId = "";

    /**
     * 物体名称
     */
    public string objName = "";

    /**
     * 物体类型
     */
    public string objType = "";

    /**
     * 物体皮肤
     */
    public string skin = "";

    /**
     * x坐标
     */
    public int x = 0;

    /**
     * y坐标
     */
    public int y = 0;

    /**
     * 世界坐标x轴
     */
    public int cx = 0;

    /**
     * 世界坐标y轴
     */
    public int cy = 0;

    /**
     * 自定义参数
     */
    public string userParams = "";
    
}

/**
 * 编辑npc的数据
 */
class EditNpcData : EditObjData
{
    /**
     * 角色方向,值为 0-7
     */
    public int direction = 0;

    /**
     * 是否巡逻
     */
    public bool isPatrol = false;

    /**
     * 对话id
     */
    public int dialogueId = 0;

    /**
     * 任务id
     */
    public int taskId = 0;

    /**
     * 功能id
     */
    public int funcId = 0;

    /**
     * npc类型
     */
    public int npcType = 0;
}

/**
 * 编辑怪物的数据
 */
class EditMonsterData : EditObjData
{
    /**
     * 角色方向,值为 0-7
     */
    public int direction = 0;

    /**
     * 是否巡逻
     */
    public bool isPatrol = false;

    /**
     * 对话id
     */
    public int dialogueId = 0;

    /**
     * 战斗id
     */
    public int fightId = 0;

    /**
     * 怪物类型
     */
    public int monsterType = 0;
}

/**
 * 编辑传送门的数据
 */
class EditTransferData : EditObjData
{
    /**
     * 传送到目标地图Id
     */
    public string targetMapId = "";

    /**
     * 目标地图的出生点Id
     */
    public int targetMapSpawnId = 0;

    /**
     * 传送门类型
     */
    public int transferType = 0;
}

/**
 * 编辑出生点的位置
 */
class EditSpawnPointData : EditObjData
{
    /**
     * 出生点Id
     */
    public int spawnId = 0;

    /**
     * 是否是默认出生点
     */
    public bool defaultSpawn = false;
}

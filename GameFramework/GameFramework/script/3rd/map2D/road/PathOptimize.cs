/**
 * 路径优化类型
 * @作者 落日故人 QQ 583051842
 */
 public enum PathOptimize 
 {
    /**
     * 不优化
     * 
     * 
     * 路径不一定是最短路径
     * 
     * 路径只会横行和纵向相连，另外还有45度斜向相连（两个路点间宽和高比例相等的情况）
     * 
     * 使用情况：一般对需要严格检测走每一个路点信息的情况使用，比如SLG游戏，经营类游戏
     */
    none = 0,

    /**
     * 更好的优化
     * 
     * 
     * 路径不一定是最短路径
     * 
     * 路径只会横行和纵向相连，另外还有45度斜向相连（两个路点间宽和高比例相等的情况）。
     * 
     * 优化原理：多个路排成一条线，把多余的路点优化掉，只留首尾两端的两个路点。比如100个路点水平排成一条线，经过优化后就剩“第1个路点”和“第100个路点”，中间的98个路点被优化掉
     * 
     * 优化的目的: 后端向前端发送寻路消息时，可以减少发很多路径信息
     * 
     * 使用情况：一般用于SLG游戏。
     */
    better = 1,

    /**
     * 最好的优化
     * 
     * 
     * 会得寻路中最短的路径
     * 
     * 和better优化对比，best的路径优化不限于横行，纵向，斜45度这3种情况。而是无论任何角度，只要首尾相连的两个点能直接通过，那么中间多余的n个点都会被优化掉，已达到最短路径的优化
     * 
     * 例如：寻路时，开始点和目标点中间有曲曲折折很多个路点，但是经过优化检测，发现开始点和目标点是可以两点一线直达的，所以中间的这一系列点都会被优化掉。最终只剩开始点和目标点，玩家也会两点一线走完这路径，不会绕弯。
     * 
     * 使用情况：一般用于RPG，RTS游戏。
     */
    best = 2,
 }
 
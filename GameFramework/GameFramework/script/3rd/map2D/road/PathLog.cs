
using System;
/**
* 寻路模块专用的日志打印接口
*/
public class PathLog {


    public static void log(object message, Object context) { 
    }

    public static void log(object message)
    {
    }
    // public static log:Function = console.log; //默认打印日志
    //public static log:Function = function(...args){}; //默认不打印日志

    /**
     * 设置是否可打印日志
     * @param enable 
     */
    public static void setLogEnable(bool enable)
    {
        if(enable)
        {
            // this.log = console.log;
        }else
        {
            // this.log = function(...args){};
        }
    }

}

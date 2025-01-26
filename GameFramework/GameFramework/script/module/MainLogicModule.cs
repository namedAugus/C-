namespace GameFramework.script.module;

public class MainLogicModule
{
    
    public static void StartLoop()
    {
        //多线程ticker
        XTimer.GetInstance().StartSync(); 
        
    }
}
// See https://aka.ms/new-console-template for more information
using GameFramework.script.module;
Console.WriteLine("Welcome! XServer Start...");
if (!DBManager.Connect("game", "127.0.0.1", 3307, "root", "root"))
{
    return;
}
// //测试
// if (DBManager.RegisterUser("hks", "1"))
// {
//     Console.WriteLine("User 'hks' successfully registered!");
// }
MainLogicModule.StartLoop();
// 获取当前工作目录
// string currentDirectory = Directory.GetCurrentDirectory();
// Console.WriteLine("Current Directory: " + currentDirectory);
//
// // 获取应用程序基目录
// string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
// Console.WriteLine("Base Directory: " + baseDirectory);
NetManager.StarLoop(8888);



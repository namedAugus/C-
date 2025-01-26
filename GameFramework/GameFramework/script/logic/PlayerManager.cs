using System.Collections.Generic;

public class PlayerManager
{
    //玩家列表
    public static Dictionary<string, Player> players = new Dictionary<string, Player>();
    //玩家是否在线
    public static bool IsOnline(string username)
    {
        return players.ContainsKey(username);
    }
    //获取玩家
    public static Player GetPlayer(string username)
    {
        if (players.ContainsKey(username))
        {
            return players[username];
        }

        return null;
    }
    //添加玩家
    public static void AddPlayer(string username, Player player)
    {
        players.Add(username, player);
    }
    //删除玩家
    public static void RemovePlayer(string username)
    {
        players.Remove(username);
    }
    
}

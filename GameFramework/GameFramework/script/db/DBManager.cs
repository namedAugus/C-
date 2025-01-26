
using System;
using System.Text.RegularExpressions;
using LitJson;
using MySql.Data.MySqlClient;

public class DBManager
{
    public static MySqlConnection mysql;
    
    //连接mysql数据库
    public static bool Connect(string db, string ip, int port, string user, string password)
    {
        //创建MysqlConnection对象
        mysql = new MySqlConnection();
        //连接参数
        string s = string.Format("Database={0}; Data Source={1}; port={2}; User Id={3}; Password={4}", db,ip,port,user,password);
        mysql.ConnectionString = s;
        //连接
        try
        {
            mysql.Open();
            Console.WriteLine("[数据库] connect success!");
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine("[数据库] connect fail, " + e.Message);
            return false;
        }
    }
    
    //判定安全字符串
    private static bool IsSafeString(string str)
    {
        return !Regex.IsMatch(str, @"[-|;|,|\/|\(|\)|\[|\]|\}|\{|%|@|\*|!|\']");
    }
    
    //判定用户是否存在
    public static bool IsAccountExist(string username)
    {
        //防止sql注入
        if (!IsSafeString(username))
        {
            return false;
        }
        //sql语句
        string s = string.Format("select * from account where username = '{0}'", username);
        //查询
        try
        {
            MySqlCommand cmd = new MySqlCommand(s, mysql);
            MySqlDataReader dataReader = cmd.ExecuteReader();
            bool hasRows = dataReader.HasRows;
            dataReader.Close();
            return !hasRows;
        }
        catch (Exception e)
        {
            Console.WriteLine("[数据库] IsSafeString err, "+e.Message);
            return false;
        }
    }
    
    //注册
    public static bool RegisterUser(string username, string password)
    {
        //防止sql注入
        if (!IsSafeString(username))
        {
            Console.WriteLine("[数据库] RegisterUser fail, username not safe");
            return false;
        }
        if (!IsSafeString(username))
        {
            Console.WriteLine("[数据库] RegisterUser fail, password not safe");
            return false;
        }
        //能否注册
        if (!DBManager.IsAccountExist(username))
        {
            Console.WriteLine("[数据库] RegisterUser fail, username exist"); 
            return false;
        }
        //TODO:password加密
        
        //写入数据库User表
        string sql = string.Format("insert into account(username,password) values('{0}','{1}')", username, password);
        try
        {
            MySqlCommand cmd = new MySqlCommand(sql, mysql);
            cmd.ExecuteNonQuery();
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine("[数据库] RegisterUser fail, " + e.Message); 
            return false;
        }
    }
    
    //创建角色
    public static bool CreatePlayer(string username)
    {
        //防止sql注入
        if (!DBManager.IsSafeString(username))
        {
            Console.WriteLine("[数据库] CreatePlayer fail, id not safe");
            return false;
        }
        //序列化
        PlayerData playerData = new PlayerData();
        string data = JsonMapper.ToJson(playerData);
        //写入数据库
        string sql = string.Format("insert into player(username,data) values('{0}','{1}')", username, data);
        try
        {
            MySqlCommand cmd = new MySqlCommand(sql, mysql);
            cmd.ExecuteNonQuery();
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine("[数据库] CreatePlayer err, " + e.Message); 
            return false;
        }
    }
    
    //检测用户名和密码
    public static bool CheckPassword(string username, string password)
    {
        //防止sql注入
        if (!DBManager.IsSafeString(username))
        {
            Console.WriteLine("[数据库] CheckPassword fail, username not safe");
            return false;
        }
        if (!DBManager.IsSafeString(password))
        {
            Console.WriteLine("[数据库] CheckPassword fail, password not safe");
            return false;
        }
        //查询
        string sql = string.Format("select * from account where username = '{0}' and password ='{1}';", username, password);
        try
        {
            MySqlCommand cmd = new MySqlCommand(sql, mysql);
            MySqlDataReader dataReader = cmd.ExecuteReader();
            bool hasRows = dataReader.HasRows;
            dataReader.Close();
            return hasRows;
        }
        catch (Exception e)
        {
            Console.WriteLine("[数据库] CheckPassword err, " + e.Message); 
            return false;
        }
    }
    
    //获取玩家数据
    public static PlayerData GetPlayerData(string username)
    {
        //防止sql注入
        if (!DBManager.IsSafeString(username))
        {
            Console.WriteLine("[数据库] GetPlayerData fail, id not safe");
            return null;
        }
        //SQL
        string sql = string.Format("select * from player where username = '{0}';", username);
        try
        {
            MySqlDataReader dataReader = new MySqlCommand(sql, mysql).ExecuteReader();
            if (!dataReader.HasRows)
            {
                dataReader.Close();
                return null;
            }
            //读取
            dataReader.Read();
            string data = dataReader.GetString("data");
            //反序列化
            PlayerData playerData = JsonMapper.ToObject<PlayerData>(data);
            dataReader.Close();
            return playerData;
        }
        catch (Exception e)
        {
            Console.WriteLine("[数据库] GetPlayerData err, " + e.Message); 
            return null;
        }
    }
    
    //更新或保存玩家数据
    public static bool UpdatePlayerData(string username, PlayerData playerData)
    {
        //序列化
        string data = JsonMapper.ToJson(playerData);
        //sql
        string sql = string.Format("update player set username = '{0}', data = '{1}'", username, data);
        //更新
        try
        {
            int flag = new MySqlCommand(sql, mysql).ExecuteNonQuery();
            if (flag > 0)
            {
                return true;
            }
            return false;
        }
        catch (Exception e)
        {
            Console.WriteLine("[数据库] UpdatePlayerData err, " + e.Message); 
            return false;
        }
    }
}

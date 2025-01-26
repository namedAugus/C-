
using gprotocol;
using LitJson;
using MySql.Data.MySqlClient;

public partial class MsgHandler
{
    //获取记事本内容
    public static void MsgGetText(ClientState c, ProtoBuf.IExtensible msgBase)
    {
        MsgGetText msg = (MsgGetText)msgBase;
        Player player = c.player;
        if(player == null) return;
        //获取text
        msg.text = player.data.text;
        Console.WriteLine(msg.text);
        player.Send(msg);
    }
    
    //保存记事本内容
    public static void MsgSaveText(ClientState c, ProtoBuf.IExtensible msgBase)
    {
        MsgSaveText msg = (MsgSaveText)msgBase;
        Player player = c.player;
        if(player == null) return;
        //获取text
        player.data.text = msg.text;
        // player.Send(msg);
        //序列化
        string data = JsonMapper.ToJson(player.data);
        //sql
        string sql = string.Format("update player set username = '{0}', data = '{1}'", player.id, data);
        //更新
        try
        {
            int flag = new MySqlCommand(sql, DBManager.mysql).ExecuteNonQuery();
            if (flag > 0)
            {
                // return true;
            }
            // return false;
        }
        catch (Exception e)
        {
            Console.WriteLine("[数据库] MsgSaveText err, " + e.Message); 
            // return false;
        }
    }
}
using gprotocol;
public partial class MsgHandler
{
   //注册协议处理
   public static void MsgRegister(ClientState c, ProtoBuf.IExtensible msgBase)
   {
      MsgRegister msg = (MsgRegister)msgBase;
      //注册
      if (DBManager.RegisterUser(msg.username, msg.password))
      {
         bool flag = DBManager.CreatePlayer(msg.username); //TODO：这块可以优化，username不要直接给player表的id
         if (flag == false)
         {
            msg.result = 0;
         }
         else
         {
            msg.result = 1;
         }
      }
      else
      {
         msg.result = 0;
      }

      NetManager.Send(c, msg);
   }
   //登录协议处理
   public static void MsgLogin(ClientState c, ProtoBuf.IExtensible msgBase)
   {
      MsgLogin msg = (MsgLogin)msgBase;
      //密码校验
      if (!DBManager.CheckPassword(msg.username, msg.password))
      {
         msg.result = 0;
         NetManager.Send(c, msg);
         return;
      }
      //已登录
      if (c.player != null)
      {
         msg.result = 0;
         NetManager.Send(c, msg);
         return;
      }
      //抢占登录，后者踢前者下线
      if (PlayerManager.IsOnline(msg.username)) //username or id ?
      {
         //发送踢下线协议
         Player other = PlayerManager.GetPlayer(msg.username);
         MsgKick msgKick = new MsgKick();
         msgKick.reason = 1;
         other.Send(msgKick);
         //断开当前用户连接
         NetManager.Close(other.state);
      }
      
      //登录成功，获取玩家数据
      PlayerData playerData = DBManager.GetPlayerData(msg.username);
      if (playerData == null)
      {
         // msg.result = 1;
         // NetManager.Send(c, msg);
         // return;
         playerData = new PlayerData();
         playerData.text = ""; //TODO:这块处理data为空的情况

      }
      //构建player
      Player player = new Player(c);
      player.id = msg.username; //username or id?
      player.data = playerData;
      PlayerManager.AddPlayer(msg.username, player);
      c.player = player;
      //返回登录协议
      msg.result = 1;
      player.Send(msg);

   }
   
   //
}
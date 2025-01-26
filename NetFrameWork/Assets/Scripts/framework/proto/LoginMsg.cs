/*
//注册协议
public class MsgRegister : MsgBase
{
    public MsgRegister() { protoName = "MsgRegister"; }
    //客户端发
    public string username = "";
    public string password = "";
    //服务端返回（1-成功，0-失败）
    public int result = 0;
}

//登录协议
public class MsgLogin : MsgBase
{
    public MsgLogin() { protoName = "MsgLogin"; }
    //客户端发
    public string username = "";
    public string password = "";
    //服务端返回（1-成功，0-失败）
    public int result = 0;
}

//踢下线协议
public class MsgKick : MsgBase
{
    public MsgKick() { protoName = "MsgKick"; }
    //原因（1 - 账户多人登录）
    public int reason = 0;
}
*/
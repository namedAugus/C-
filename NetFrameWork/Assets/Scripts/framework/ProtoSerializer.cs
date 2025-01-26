using System;
using UnityEngine;

public class ProtoSerializer
{
    static string prefix = "gprotocol.";
    //编码proto
    public static byte[] Encode(ProtoBuf.IExtensible msgBase)
    {
        //将protobuf对象序列化为Byte数组
        using (var memory = new System.IO.MemoryStream())
        {
            ProtoBuf.Serializer.Serialize(memory, msgBase);
            return memory.ToArray();
        }
    }
    //解码proto
    public static ProtoBuf.IExtensible Decode(string protoName, byte[] bytes, int offset, int count)
    {
        using (var memory = new System.IO.MemoryStream(bytes, offset, count))
        {

            System.Type t = System.Type.GetType(prefix + protoName);
            //System.Type t = System.Type.GetType(protoName);
            return (ProtoBuf.IExtensible)ProtoBuf.Serializer.NonGeneric.Deserialize(t, memory);
        }
    }

    //编码ptoto协议名(2字节长度+字符串）
    public static byte[] EncodeName(ProtoBuf.IExtensible msgBase)
    {

        string[] allName = msgBase.ToString().Split('.');
        string protoName = allName[allName.Length - 1];
        Debug.Log("proto协议：" + protoName);
        //名字bytes和长度
        byte[] nameBytes = System.Text.Encoding.UTF8.GetBytes(protoName);
        Int16 len = (Int16)nameBytes.Length;
        //申请bytes数值
        byte[] bytes = new byte[2 + len];
        //组装2字节长度信息
        bytes[0] = (byte)(len % 256);
        bytes[1] = (byte)(len / 256);
        //组装名字bytes
        Array.Copy(nameBytes, 0, bytes, 2, len);
        return bytes;
    }

    //解码proto协议名(2字节长度+字符串）
    public static string DecodeName(byte[] bytes, int offset, out int count)
    {


        count = 0;
        //必须大于2字节
        if (offset + 2 > bytes.Length)
        {
            return "";
        }

        //读取长度
        Int16 len = (Int16)((bytes[offset + 1] << 8) | bytes[offset]);
        if (len <= 0)
        {
            return "";
        }

        //长度必须足够
        if (offset + 2 + len > bytes.Length)
        {
            return "";
        }

        //解析
        count = 2 + len;
        string name = System.Text.Encoding.UTF8.GetString(bytes, offset + 2, len);
        return name;

    }



}

using System;
namespace UnityEngine
{
    /// <summary>
    /// 服务器拓展UnityEngine数学库（保持服务器与客户端一致）
    /// </summary>
    public static class Mathf
    {
        public const float PI = 3.14159274F;
        // 绝对值
        public static float Abs(float value)
        {
            return Math.Abs(value);
        }
        public static int Abs(int value)
        {
            return Math.Abs(value);
        }

        // 最大值
        public static float Max(float a, float b)
        {
            return Math.Max(a, b);
        }
        public static int Max(int a, int b)
        {
            return Math.Max(a, b);
        }

        // 最小值
        public static float Min(float a, float b)
        {
            return Math.Min(a, b);
        }
        public static int Min(int a, int b)
        {
            return Math.Min(a, b);
        }

        // 向上取整
        public static float Ceil(float value)
        {
            return (float)Math.Ceiling(value);
        }
        // public static int Ceil(int value)
        // {
        //     return (int)Math.Ceiling(value);
        // }
        
        // 向下取整
        public static float Floor(float value)
        {
            return (float)Math.Floor(value);
        }
        
        // 实现 Mathf.Round
        public static float Round(float f)
        {
            return (float)Math.Round(f);
        }

        // 实现 Mathf.Atan2
        public static float Atan2(float y, float x)
        {
            return (float)Math.Atan2(y, x);
        }

        // 实现 Mathf.Cos
        public static float Cos(float angle)
        {
            return (float)Math.Cos(angle);
        }

        // 实现 Mathf.Sin
        public static float Sin(float angle)
        {
            return (float)Math.Sin(angle);
        }
        // 其他Mathf函数可以在这里继续实现
    }
}
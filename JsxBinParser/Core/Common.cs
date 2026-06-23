using System;

namespace Parser
{
    /// <summary>
    /// 对应 C++ common.h 中的宏和辅助工具
    /// </summary>
    public static class Common
    {
        public static T Max<T>(T x, T y) where T : IComparable<T>
            => x.CompareTo(y) > 0 ? x : y;

        public static T Min<T>(T x, T y) where T : IComparable<T>
            => x.CompareTo(y) < 0 ? x : y;

        /// <summary>
        /// 对应宏 _in_range : 严格开区间 (min, max)
        /// 返回 (x - min) * (max - x) > 0
        /// </summary>
        public static bool InRangeStrict<T>(T min, T max, T x) where T : IComparable<T>
        {
            // 使用乘法判断，但需转换为 double 或 decimal 避免溢出，实际常用逻辑是直接比较
            // 原宏用乘法，但等效于 (x > min && x < max)
            return x.CompareTo(min) > 0 && x.CompareTo(max) < 0;
        }

        /// <summary>
        /// 对应宏 _in_range_i : 闭区间 [min, max]
        /// 返回 (x - min) * (max - x) >= 0
        /// </summary>
        public static bool InRangeStrictI<T>(T min, T max, T x) where T : IComparable<T>
        {
            return x.CompareTo(min) >= 0 && x.CompareTo(max) <= 0;
        }

        /// <summary>
        /// 对应宏 in_range : 开区间 (min, max)
        /// 直接比较
        /// </summary>
        public static bool InRange<T>(T min, T max, T x) where T : IComparable<T>
        {
            return x.CompareTo(min) > 0 && x.CompareTo(max) < 0;
        }

        /// <summary>
        /// 对应宏 in_range_i : 闭区间 [min, max]
        /// 直接比较
        /// </summary>
        public static bool InRangeI<T>(T min, T max, T x) where T : IComparable<T>
        {
            return x.CompareTo(min) >= 0 && x.CompareTo(max) <= 0;
        }

        public static bool InRange(int min, int max, int x) => x > min && x < max;
        public static bool InRangeI(int min, int max, int x) => x >= min && x <= max;
        public static bool InRange(double min, double max, double x) => x > min && x < max;
        public static bool InRangeI(double min, double max, double x) => x >= min && x <= max;
    }

    // ========== 类型别名说明（仅供理解，实际代码中直接使用以下 .NET 类型） ==========
    // Token  -> byte
    // Byte   -> byte
    // Number -> double
    // String -> string
    // Bytes  -> List<byte>
    // ByteString -> List<ushort>   （对应 std::vector<uint16_t>）
}
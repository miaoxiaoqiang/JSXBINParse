using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Parser
{
    public static class Utils
    {
        public static bool StringEqual(string str1, string str2)
        {
            int minLen = Math.Min(str1.Length, str2.Length);
            // 比较前 minLen 个字符
            return string.Compare(str1, 0, str2, 0, minLen) == 0;
        }

        public static void StringReplaceChar(ref string str, char search, char replace)
        {
            str = str.Replace(search, replace);
        }

        public static void StringStripChar(ref string str, char search)
        {
            str = str.Replace(search.ToString(), "");
        }

        public static void ReplaceStrInplace(ref string subject, string search, string replace)
        {
            subject = subject.Replace(search, replace);
        }

        private const string HexCharsetCapital = "0123456789ABCDEF";
        private const string HexCharsetSmall = "0123456789abcdef";

        public static string UnicodeEscape(ushort value, bool capital = false)
        {
            string cs = capital ? HexCharsetCapital : HexCharsetSmall;
            char[] result = new char[6] { '\\', 'u', '0', '0', '0', '0' };
            for (int i = 0; i < 4; i++)
            {
                int nibble = (value >> (4 * i)) & 0xF;
                result[5 - i] = cs[nibble];
            }

            //return new string(result); //输出转义序列
            return UnescapeUnicode(new string(result)); //输出对应字符
        }

        public static string HexEscape(byte value, bool capital = false)
        {
            string cs = capital ? HexCharsetCapital : HexCharsetSmall;
            char[] result = new char[4] { '\\', 'x', '0', '0' };
            for (int i = 0; i < 2; i++)
            {
                int nibble = (value >> (4 * i)) & 0xF;
                result[3 - i] = cs[nibble];
            }

            //return new string(result); //输出转义序列
            return UnescapeUnicode(new string(result)); //输出对应字符
        }

        public static bool IsNonPrintableAscii(uint value)
        {
            return Common.InRangeI<uint>(0, 7, value) || Common.InRangeI<uint>(0x0E, 0x1F, value) || value == 0x7F;
            //return (value >= 0 && value <= 7) ||
            //       (value >= 0x0E && value <= 0x1F) ||
            //       value == 0x7F;
        }

        public static bool IsNonPrintableUtf8(uint value)
        {
            return IsNonPrintableAscii(value) || Common.InRangeI<uint>(0x80, 0xFF, value);// (value >= 0x80 && value <= 0xFF);
        }

        public static bool IsNonPrintableUtf16(uint value)
        {
            return IsNonPrintableAscii(value) || Common.InRangeI<uint>(0x80, 0xFF, value) || value > 0xFF;
        }

        public static string EscapeHexOrUnicode(ushort value, bool capital = false)
        {
            if (Common.InRangeI<uint>(0x00, 0xFF, value))
            {
                return HexEscape((byte)value, capital);
            }
            else
            {
                return UnicodeEscape(value, capital);
            }
        }

        public static string StringLiteralEscape(ushort value, bool capital = false)
        {
            switch (value)
            {
                case '\b': return "\\b";
                case '\f': return "\\f";
                case '\n': return "\\n";
                case '\r': return "\\r";
                case '\v': return "\\v";
                case '\t': return "\\t";
                case '\"': return "\\\"";
                case '\'': return "\\\'";
                case '\\': return "\\\\";
                default:
                    return IsNonPrintableUtf16(value)
                        ? EscapeHexOrUnicode(value, capital)
                        : new string((char)value, 1);
            }
        }

        public static string StringLiteralEscape(IEnumerable<ushort> value, bool capital = false)
        {
            var sb = new StringBuilder();
            foreach (var c in value)
            {
                sb.Append(StringLiteralEscape(c, capital));
            }

            return sb.ToString();
        }

        public static string StringLiteralUnescape(string value)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < value.Length; i++)
            {
                if (value[i] != '\\' || i + 1 == value.Length)
                {
                    sb.Append(value[i]);
                    continue;
                }

                i++;
                switch (value[i])
                {
                    case 'b': sb.Append('\b'); break;
                    case 'f': sb.Append('\f'); break;
                    case 'n': sb.Append('\n'); break;
                    case 'r': sb.Append('\r'); break;
                    case 'v': sb.Append('\v'); break;
                    case 't': sb.Append('\t'); break;
                    case '"': sb.Append('"'); break;
                    case '\'': sb.Append('\''); break;
                    default:
                        sb.Append('\\');
                        sb.Append(value[i]);
                        break;
                }
            }
            return sb.ToString();
        }

        public static string FromStringLiteral(string value)
        {
            if (value.Length >= 2 && value[0] == '"' && value[value.Length - 1] == '"')
            {
                value = value.Substring(1, value.Length - 2);
            }

            return StringLiteralUnescape(value);
        }

        public static string ToStringLiteral(IEnumerable<ushort> value, bool capital = false)
        {
            return "\"" + StringLiteralEscape(value, capital) + "\"";
        }

        public static string ToStringLiteral(string value, bool capital = false)
        {
            return "\"" + StringLiteralEscape(value.Select(c => (ushort)c), capital) + "\"";
        }

        public static string ToString(IEnumerable<ushort> value)
        {
            if (value == null)
            {
                return string.Empty;
            }

            return new string(value.Select(v => (char)v).ToArray());
        }

        public static List<ushort> ToByteString(string value)
        {
            return value.Select(c => (ushort)c).ToList();
        }

        public static int ByteLength(ulong value)
        {
            int len = 0;
            while (value > 0)
            {
                len++;
                value >>= 8;
            }
            return len;
        }

        public static bool IsNumberNegative(double value)
        {
            ulong bits = BitConverter.ToUInt64(BitConverter.GetBytes(value), 0);
            return (bits & (1UL << 63)) != 0;
        }

        public static ulong NumberRawCastToUInt64(double value)
        {
            return BitConverter.ToUInt64(BitConverter.GetBytes(value), 0);
        }

        public static double NumberRawCastToDouble(ulong value)
        {
            return BitConverter.ToDouble(BitConverter.GetBytes(value), 0);
        }

        public static bool IsNumberInteger(double value)
        {
            ulong positiveBits = NumberToInteger(value);
            return ByteLength(positiveBits) < 8;
        }

        public static bool IsNumberDouble(double value) => !IsNumberInteger(value);

        public static ulong NumberToInteger(double value)
        {
            ulong bits = NumberRawCastToUInt64(value);
            return bits & ~(1UL << 63);
        }

        public static double NumberToDouble(double value)
        {
            ulong integerBits = NumberToInteger(value);
            return NumberRawCastToDouble(integerBits);
        }

        public static ulong NumberAsUInt64(double value)
        {
            if (IsNumberDouble(value))
            {
                return (ulong)value;
            }
            else
            {
                return NumberRawCastToUInt64(value);
            }
        }

        public static long NumberAsInt64(double value)
        {
            if (IsNumberDouble(value))
            {
                return (long)value;
            }
            else
            {
                return (long)NumberRawCastToUInt64(value);
            }
        }

        public static string NumberToString(double value)
        {
            string result = "";

            if (IsNumberNegative(value))
            {
                result += "-";
            }

            if (IsNumberInteger(value))
            {
                ulong i = NumberToInteger(value);
                result += i.ToString();
            }
            else
            {
                double d = NumberRawCastToDouble(NumberToInteger(value));
                result += d.ToString(CultureInfo.InvariantCulture);
            }

            return result;
        }

        public static bool BytesEq(byte[] a, byte[] b)
        {
            if (a.Length != b.Length)
            {
                return false;
            }

            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] != b[i])
                {
                    return false;
                }
            }

            return true;
        }

        public static bool BytesEq(byte[] a, byte[] b, int length)
        {
            if (a.Length < length || b.Length < length)
            {
                return false;
            }

            for (int i = 0; i < length; i++)
            {
                if (a[i] != b[i])
                {
                    return false;
                }
            }

            return true;
        }

        public static void ZeroMem(byte[] buffer)
        {
            Array.Clear(buffer, 0, buffer.Length);
        }

        private static string UnescapeUnicode(string input)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] == '\\' && i + 1 < input.Length)
                {
                    char next = input[i + 1];
                    if (next == 'u' && i + 5 < input.Length)
                    {
                        string hex = input.Substring(i + 2, 4);
                        int code = Convert.ToInt32(hex, 16);
                        sb.Append((char)code);
                        i += 5;
                        continue;
                    }
                    else if (next == 'x' && i + 3 < input.Length)
                    {
                        string hex = input.Substring(i + 2, 2);
                        int code = Convert.ToInt32(hex, 16);
                        sb.Append((char)code);
                        i += 3;
                        continue;
                    }
                }
                sb.Append(input[i]);
            }
            return sb.ToString();
        }

        public static int CountLeadingSpaces(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return 0;
            }

            int count = 0;
            while (count < input.Length && input[count] == ' ')
            {
                count++;
            }

            return count;
        }

        //public static string LTrim(string s, char target = ' ')
        //{
        //    int start = 0;
        //    while (start < s.Length && s[start] == target)
        //    {
        //        start++;
        //    }

        //    return start < s.Length ? s.Substring(start) : "";
        //}

        //public static string RTrim(string s, char target = ' ')
        //{
        //    int end = s.Length - 1;
        //    while (end >= 0 && s[end] == target)
        //    {
        //        end--;
        //    }

        //    return end >= 0 ? s.Substring(0, end + 1) : "";
        //}
    }
}
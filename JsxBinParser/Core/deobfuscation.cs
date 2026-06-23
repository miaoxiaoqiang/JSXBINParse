using System;
using System.Collections.Generic;
using System.Linq;

namespace Parser
{
    public sealed class DeobfuscationContext
    {
        public bool EmptyIdReserved { get; set; } = false;
    }

    public static class Deobfuscator
    {
        private static readonly List<string> Operators = new List<string>
        {
            "=", "==", "!=", "!==", "===", "<=", ">=", ">", "<",
            "|=", "||=", "&&=", "&=", "^=", "??=",
            "|", "||", "&", "&&", "^", "??", "!", "?", ":",
            "instanceof", "typeof", "delete",
            "+", "+=",
            "-", "-=",
            "*", "*=",
            "%", "%=",
            "/", "/=",
            "**", "**=",
            "<<", "<<=",
            ">>", ">>=",
            ">>>", ">>>=",
            "~"
        };

        private static readonly List<char> ProhibitedChars = new List<char>
        {
            '=', '+', '<', '>', '-', '.', '*', '/', '|', '&', '?', '!', ':', '@', '~', '%', '^'
        };

        private static readonly List<ushort> PunctConnectors = new List<ushort>
        {
            0xFE33, 0xFE34, 0xFE4D, 0xFE4E, 0xFE4F, 0xFF3F, 0x203F, 0x2040, 0x2054
        };

        /// <summary>
        /// 检查符号名是否为 ECMAScript 3 运算符
        /// </summary>
        private static bool IsECMA3Operator(List<ushort> symbol)
        {
            string symstr = Utils.ToString(symbol);
            return Operators.Any(op => symstr == op);
        }

        /// <summary>
        /// 检查符号名是否符合 ECMAScript 3 标识符命名规范
        /// </summary>
        private static bool IsECMA3CompliantName(List<ushort> symbol)
        {
            if (symbol == null || symbol.Count == 0)
            {
                return false;
            }

            ushort first = symbol[0];

            // 第一个字符不能是数字（0x29-0x40 包含数字和一些符号）
            if (Common.InRangeI<ushort>(0x29, 0x40, first))// first >= 0x29 && first <= 0x40)
            {
                return false;
            }

            // 不能是 Unicode 组合变音符号
            if (Common.InRangeI<ushort>(0x0300, 0x036F, first))// first >= 0x0300 && first <= 0x036F)
            {
                return false;
            }

            // 不能是 Unicode 连接标点
            if (PunctConnectors.Any(p => p == first))
            {
                return false;
            }

            // 不能包含禁止字符
            bool hasProhibited = symbol.Any(c => ProhibitedChars.Contains((char)c));
            return !hasProhibited;
        }

        /// <summary>
        /// 判断是否应该替换混淆后的符号名（对应 Jsxblind 反混淆）
        /// </summary>
        public static bool JsxblindShouldSubstitute(ref DeobfuscationContext context, List<ushort> symbol, bool operatorCtx)
        {
            // 如果符号名为空
            if (symbol == null || symbol.Count == 0)
            {
                if (context.EmptyIdReserved)
                {
                    return true;
                }

                context.EmptyIdReserved = true;
                return false;
            }

            bool ecmaOperator = IsECMA3Operator(symbol);

            // 如果符号是运算符，且处于运算符上下文，则保留；否则替换
            if (ecmaOperator)
            {
                // 在运算符上下文中，保留原样（不替换）
                if (operatorCtx)
                {
                    return false;
                }
                // 非运算符上下文（如用作变量名），则替换
                return true;
            }

            // 如果不符合 ECMA3 标识符规范，则替换
            if (!IsECMA3CompliantName(symbol))
            {
                return true;
            }

            // 如果符号中包含任何非 ASCII 字符（>126），替换
            if (symbol.Any(c => c > 126))
            {
                return true;
            }

            // 默认不替换
            return false;
        }
    }
}
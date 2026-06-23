using Parser.Nodes;
using System;
using System.Collections.Generic;

namespace Parser
{
    public struct Reference
    {
        public List<ushort> Id;
        public bool Flag;
    }

    public struct LineInfo
    {
        public long LineNumber;
        public AstNode Child;
        public List<string> Labels;

        public readonly string LblStatement()
        {
            string result = "";
            foreach (var label in Labels)
            {
                result += label + ": \n";
            }

            return result;
        }

        public readonly string CreateBody()
        {
            return Child == null ? "" : Child.GetString();
        }
    }

    public enum FunctionType
    {
        Normal = 0,
        ScriptClosure = 1
    }

    public struct FunctionSignature
    {
        public string Name;
        public long NumArgs;
        public long NumVars;
        public long NumConsts;
        public Dictionary<long, string> Variables;
        public int Flags;
    }

    public static class Decoders
    {
        private enum LiteralType { Number }

        private enum NumberType
        {
            kDouble = 8,
            kInteger = 4,
            kShort = 2
        }

        private static string DNumberPrimitive(Reader reader, int length, bool negative)
        {
            byte[] buffer = new byte[length];
            for (int i = 0; i < length; i++)
            {
                buffer[i] = DByte(reader);
            }

            short sign = (short)(negative ? -1 : 1);

            switch (length)
            {
                case (int)NumberType.kDouble:
                    double d = BitConverter.ToDouble(buffer, 0);
                    return (d * sign).ToString(System.Globalization.CultureInfo.InvariantCulture);
                case (int)NumberType.kInteger:
                    uint ui = BitConverter.ToUInt32(buffer, 0);
                    return ((long)ui * sign).ToString();
                case (int)NumberType.kShort:
                    ushort us = BitConverter.ToUInt16(buffer, 0);
                    return ((int)us * sign).ToString();
                default:
                    return "";
            }
        }

        private static string DLiteralPrimitive(Reader reader, LiteralType literalType)
        {
            if (reader.DecrementNodeDepth())
            {
                return "";
            }

            bool negative = false;
            if (reader.Peek() == 'y')
            {
                negative = true;
                reader.Step();
            }

            byte marker = reader.Peek();

            if (marker == '4')
            {
                reader.Step();
                return DNumberPrimitive(reader, 4, negative);
            }
            else if (marker == '2')
            {
                reader.Step();
                return DNumberPrimitive(reader, 2, negative);
            }
            else
            {
                byte num = DByte(reader);
                if (negative)
                {
                    return (-1 * (int)num).ToString();
                }
                else
                {
                    if (literalType == LiteralType.Number)
                    {
                        return ((int)num).ToString();
                    }
                    else
                    {
                        return Utils.StringLiteralEscape(num);
                    }
                }
            }
        }

        public static int DLiteralNum(Reader reader)
        {
            string value = DLiteralPrimitive(reader, LiteralType.Number);
            return string.IsNullOrEmpty(value) ? 0 : int.Parse(value);
        }

        //public static AstNode DNode(Reader reader)
        //{
        //    byte marker = reader.Get();
        //    AstNode node = NodeFactory.Get((NodeType)marker, reader);

        //    if (node != null)
        //    {
        //        node.Parse();
        //        return node;
        //    }

        //    return null;
        //}

        public static string DNumber(Reader reader)
        {
            if (reader.Get() == '8')
            {
                return DNumberPrimitive(reader, 8, false);
            }
            else
            {
                reader.Step(-1);
                string num = DLiteralPrimitive(reader, LiteralType.Number);
                return string.IsNullOrEmpty(num) ? "0" : num;
            }
        }

        public static byte DByte(Reader reader) => reader.GetByte();

        public static string DVariant(Reader reader)
        {
            var var = reader.GetVariant();
            return var != null ? var.ToString() : "";
        }

        public static Reference DIdRef(Reader reader)
        {
            var id = reader.ReadSID();
            bool flag = false;
            if (reader.Version >= JsxbinVersion.v20)
            {
                flag = reader.GetBoolean();
            }

            return new Reference { Id = id, Flag = flag };
        }

        public static Reference DLiteralRef(Reader reader)
        {
            var id = reader.ReadLiteral();
            bool flag = false;
            if (reader.Version >= JsxbinVersion.v20)
            {
                flag = reader.GetBoolean();
            }

            return new Reference { Id = id, Flag = flag };
        }

        public static long DLength(Reader reader)
        {
            string value = DLiteralPrimitive(reader, LiteralType.Number);
            if (!string.IsNullOrEmpty(value))
            {
                if (value[0] == '-')
                {
                    value = value.Substring(1);
                }

                if (long.TryParse(value, out long len))
                {
                    return len;
                }
            }

            return 0;
        }

        public static string DSid(Reader reader)
        {
            return Utils.ToString(reader.ReadSID());
        }

        public static string DOperator(Reader reader)
        {
            return Utils.ToString(reader.ReadSID(true));
        }

        //public static List<AstNode> DChildren(Reader reader)
        //{
        //    long length = DLength(reader);
        //    List<AstNode> result = new List<AstNode>();
        //    for (int i = 0; i < length; i++) //&& reader.Error == ParseError.None
        //    {
        //        var child = DNode(reader);
        //        if (child != null)
        //        {
        //            result.Add(child);
        //        }
        //    }

        //    return result;
        //}

        //public static LineInfo DLineInfo(Reader reader)
        //{
        //    LineInfo result = new LineInfo
        //    {
        //        LineNumber = DLength(reader),
        //        Child = DNode(reader)
        //    };
        //    long labelCount = DLength(reader);
        //    result.Labels = new List<string>();
        //    for (int i = 0; i < labelCount; i++)
        //    {
        //        string label = DSid(reader);
        //        result.Labels.Add(label ?? "");
        //    }
        //    return result;
        //}

        public static FunctionSignature DFnSig(Reader reader)
        {
            FunctionSignature result = new FunctionSignature
            {
                Variables = new Dictionary<long, string>()
            };
            //if (reader.Error != ParseError.None)
            //{
            //    return result;
            //}

            long nVars = DLength(reader);
            for (int i = 0; i < nVars; i++)//&& reader.Error == ParseError.None
            {
                string sid = DSid(reader);
                long idSeq = DLength(reader);
                result.Variables[idSeq] = sid;
            }

            result.NumArgs = DLength(reader);
            result.NumVars = DLength(reader);
            result.NumConsts = DLength(reader);
            result.Name = DSid(reader);
            int sf = DLiteralNum(reader);
            result.Flags = (sf | 4) << 16;
            return result;
        }

        private static bool IsCapitalAlpha(uint value) => 'A' <= value && value <= 'Z';

        private static bool IsSmallAlpha(uint value) => 'a' <= value && value <= 'z';

        private static bool IsNumericalDigit(uint value) => '0' <= value && value <= '9';

        private static bool ValidId0(uint value) => IsSmallAlpha(value) || IsCapitalAlpha(value) || value == '_' || value == '$';

        private static bool ValidIdX(uint value) => ValidId0(value) || IsNumericalDigit(value);

        public static bool ValidId(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return false;
            }

            if (!ValidId0(value[0]))
            {
                return false;
            }

            for (int i = 1; i < value.Length; i++)
            {
                if (!ValidIdX(value[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public static bool ValidId(List<ushort> value)
        {
            if (value == null || value.Count == 0)
            {
                return false;
            }

            if (!ValidId0(value[0]))
            {
                return false;
            }

            for (int i = 1; i < value.Count; i++)
            {
                if (!ValidIdX(value[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public static bool ValidXmlAttribute(List<ushort> value)
        {
            if (value.Count <= 1 || value[0] != '@')
            {
                return false;
            }

            var id = value.GetRange(1, value.Count - 1);
            return ValidId(id);
        }

        public static bool IsInteger(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return false;
            }

            foreach (char c in value)
            {
                if (!IsNumericalDigit(c))
                {
                    return false;
                }
            }

            return true;
        }

        public static bool IsInteger(List<ushort> value)
        {
            if (value == null || value.Count == 0)
            {
                return false;
            }

            foreach (ushort c in value)
            {
                if (!IsNumericalDigit(c))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
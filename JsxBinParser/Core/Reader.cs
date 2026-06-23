using System;
using System.Collections.Generic;
using System.Text;

namespace Parser
{
    public static class Constants
    {
        public static readonly byte[] JsxbinSignatureV10 = Encoding.ASCII.GetBytes("@JSXBIN@ES@1.0@");
        public static readonly byte[] JsxbinSignatureV20 = Encoding.ASCII.GetBytes("@JSXBIN@ES@2.0@");
        public static readonly byte[] JsxbinSignatureV21 = Encoding.ASCII.GetBytes("@JSXBIN@ES@2.1@");
        public const int JsxbinSignatureLen = 15;
    }

    public enum ParseError
    {
        None = 0,
        InvalidVersion,
        ReachedEnd,
        DecodeError,
        NoData,
    }

    public enum VariantType
    {
        None = -1,
        Undefined = 0,
        Null = 1,
        Boolean = 2,
        Number = 3,
        String = 4
    }

    public sealed class Variant
    {
        private VariantType _type;
        private bool _boolValue;
        private double _doubleValue;
        private List<ushort> _stringValue;

        public Variant()
        {
            _type = VariantType.None;
            DoErase();
        }

        public void DoErase()
        {
            _boolValue = false;
            _doubleValue = 0.0;
            _stringValue = null;
        }

        public void SetNull()
        {
            _type = VariantType.Null;
            DoErase();
        }

        public void SetBool(bool value)
        {
            _type = VariantType.Boolean;
            DoErase();
            _boolValue = value;
        }

        public void SetDouble(double value)
        {
            _type = VariantType.Number;
            DoErase();
            _doubleValue = value;
        }

        public void SetString(List<ushort> value)
        {
            _type = VariantType.String;
            DoErase();
            _stringValue = value;
        }

        public new string ToString()
        {
            switch (_type)
            {
                case VariantType.Undefined: return "undefined";
                case VariantType.Null: return "null";
                case VariantType.Boolean: return _boolValue ? "true" : "false";
                case VariantType.Number: return Utils.NumberToString(_doubleValue);
                case VariantType.String: return Utils.ToStringLiteral(_stringValue);
                default: return "";
            }
        }
    }

    public sealed class Reader
    {
        private readonly List<byte> _data;
        private int _start;
        private readonly int _end;
        public int _cursor;
        private int _depth;
        private ParseError _error;
        private JsxbinVersion _version;
        private readonly byte[] _numberBuffer = new byte[8];
        private DeobfuscationContext _deobfuscationContext;

        private readonly bool _unblind;
        private readonly Dictionary<double, List<ushort>> _symbols;

        public Reader(string jsxbin, bool unblind)
        {
            //Utils.StringStripChar(ref _input, ' ');
            //Utils.StringStripChar(ref _input, '\t');
            //Utils.StringStripChar(ref _input, '\r');
            //Utils.StringStripChar(ref _input, '\n');
            //Utils.StringStripChar(ref _input, '\\');

            var sb = new StringBuilder(jsxbin.Length);
            foreach (char c in jsxbin)
            {
                if (c != ' ' && c != '\t' && c != '\r' && c != '\n' && c != '\\')
                {
                    sb.Append(c);
                }
            }
            string _input = sb.ToString();

            byte[] bytes = Encoding.ASCII.GetBytes(_input);
            _data = new List<byte>(bytes);
            _start = 0;
            _cursor = 0;
            _end = _data.Count > 0 ? _data.Count - 1 : -1;
            _depth = 0;
            _error = ParseError.None;
            _version = JsxbinVersion.Invalid;
            _unblind = unblind;
            _deobfuscationContext = new DeobfuscationContext();
            _symbols = new Dictionary<double, List<ushort>>();
        }

        public JsxbinVersion Version => _version;

        public ParseError Error => _error;

        public bool ShouldUnblind => _unblind;

        public bool VerifySignature()
        {
            if (_data == null || _data.Count == 0)
            {
                _error = ParseError.NoData;
                return false;
            }

            if (_data.Count < Constants.JsxbinSignatureLen)
            {
                _error = ParseError.InvalidVersion;
                return false;
            }

            byte[] sig = _data.GetRange(0, Constants.JsxbinSignatureLen).ToArray();
            if (Utils.BytesEq(sig, Constants.JsxbinSignatureV10))
            {
                _version = JsxbinVersion.v10;
            }
            else if (Utils.BytesEq(sig, Constants.JsxbinSignatureV20))
            {
                _version = JsxbinVersion.v20;
            }
            else if (Utils.BytesEq(sig, Constants.JsxbinSignatureV21))
            {
                _version = JsxbinVersion.v21;
            }
            else
            {
                _error = ParseError.InvalidVersion;
                return false;
            }

            _start = _cursor += Constants.JsxbinSignatureLen;

            return true;
        }

        public byte Get()
        {
            byte token = Next();
            while (Ignorable(token))
            {
                token = Next();
            }

            return token;
        }

        public byte Peek(int offset = 0)
        {
            int idx = _cursor + offset;
            if (idx < 0 || idx >= _data.Count)
            {
                return 0;
            }

            return _data[idx];
        }

        public void Step(int offset = 1)
        {
            _cursor += offset;
        }

        public byte GetByte()
        {
            if (_depth > 0)
            {
                --_depth;
                return 0;
            }

            byte m = Get();

            if (m == '0')
            {
                byte n = Get();
                if (n > 0x5A)
                {
                    goto error8;
                }

                _depth = n - 0x40;
                return 0;
            }
            else if (m > 0x5A)
            {
                if (m > 0x6E)
                {
                    goto error8;
                }

                byte z = Get();
                byte l;
                byte r = (byte)(32 * (m + 1));
                if (z > 0x5A)
                {
                    if (z > 0x66)
                    {
                        goto error8;
                    }

                    l = (byte)(z - 0x47);
                }
                else
                {
                    l = (byte)(z - 0x41);
                }
                return (byte)(l | r);
            }

            return (byte)(m - 0x41);

        error8:
            _error = ParseError.DecodeError;
            return 0;
        }

        private double GetNumber()
        {
            if (_depth > 0)
            {
                --_depth;
                return 0.0;
            }

            byte t = Get();
            double sign;
            if (t != 'y')
            {
                sign = 1.0;
            }
            else
            {
                t = Get();
                sign = -1.0;
            }

            double res;
            switch (t)
            {
                case (byte)'2':
                case (byte)'4':
                case (byte)'8':
                    int count = t - 48;
                    if (count < 8)
                    {
                        Array.Clear(_numberBuffer, count, 8 - count);
                    }

                    for (int i = 0; i < count; i++)
                    {
                        _numberBuffer[i] = GetByte();
                    }
                    res = BitConverter.ToDouble(_numberBuffer, 0);
                    break;
                default:
                    Step(-1);
                    res = GetByte();
                    break;
            }

            return sign * res;
        }

        public List<ushort> GetString()
        {
            var result = new List<ushort>();
            ulong length = Utils.NumberAsUInt64(GetNumber());

            for (ulong i = 0; i < length; i++)
            {
                ushort u16 = (ushort)Utils.NumberAsUInt64(GetNumber());
                result.Add(u16);
            }
            return result;
        }

        public bool GetBoolean()
        {
            byte t = Get();
            if (t == 't')
            {
                return true;
            }

            if (t == 'f')
            {
                return false;
            }

            _error = ParseError.DecodeError;
            return false;
        }

        public List<ushort> ReadSID(bool operatorContext = false)
        {
            List<ushort> symbol;
            double id;

            if (Get() == 'z')
            {
                symbol = GetString();
                id = GetNumber();
                int idInt = (int)Utils.NumberAsUInt64(id);

                if (_unblind && Deobfuscator.JsxblindShouldSubstitute(ref _deobfuscationContext, symbol, operatorContext))
                {
                    string deobfuscated = "symbol_" + idInt.ToString();
                    symbol = Utils.ToByteString(deobfuscated);
                }

                AddSymbol(id, symbol);
            }
            else
            {
                Step(-1);
                id = GetNumber();
                symbol = GetSymbol(id);
            }

            return symbol;
        }

        public List<ushort> ReadLiteral()
        {
            List<ushort> symbol;
            double id;

            if (Get() == 'z')
            {
                symbol = GetString();
                id = GetNumber();
                AddSymbol(id, symbol);
            }
            else
            {
                Step(-1);
                id = GetNumber();
                symbol = GetSymbol(id);
                symbol ??= new List<ushort>();
            }

            return symbol;
        }

        public Variant GetVariant()
        {
            if (Get() == 'n')
                return null;
            else
                Step(-1);

            byte type = (byte)(Get() - 'a');
            var result = new Variant();
            switch (type)
            {
                case 0: // 'a'
                    result.DoErase();
                    result.SetNull();
                    break;
                case 1: // 'b'
                    result.SetNull();
                    break;
                case 2: // 'c'
                    result.SetBool(GetBoolean());
                    break;
                case 3: // 'd'
                    result.SetDouble(GetNumber());
                    break;
                case 4: // 'e'
                    result.SetString(GetString());
                    break;
                default:
                    _error = ParseError.DecodeError;
                    break;
            }
            return result;
        }

        private void AddSymbol(double id, List<ushort> symbol)
        {
            _symbols[id] = symbol;
        }

        private List<ushort> GetSymbol(double id)
        {
            return _symbols.TryGetValue(id, out var sym) ? sym : null;
        }

        private int GetNodeDepth()
        {
            if (_depth == 0)
            {
                UpdateNodeDepth();
            }

            return _depth;
        }

        public bool DecrementNodeDepth()
        {
            if (GetNodeDepth() == 0)
            {
                return false;
            }

            _depth--;
            return true;
        }

        private void UpdateNodeDepth()
        {
            _depth = ParseNodeDepth();
        }

        private int ParseNodeDepth()
        {
            byte current = Peek();
            if (current == 'A')
            {
                Step();
                return 1;
            }
            else if (current == '0')
            {
                Step();
                int levels = Get() - 0x3f;
                if (levels > 0x1b)
                {
                    return levels + ParseNodeDepth();
                }

                return levels;
            }
            return 0;
        }

        private byte Next()
        {
            if (_cursor < _end)
            {
                return _data[_cursor++];
            }

            _error = ParseError.ReachedEnd;
            return _end >= 0 ? _data[_end] : (byte)0;
        }

        private static bool Ignorable(byte value)
        {
            char c = (char)value;
            return c == ' ' || c == '\t' || c == '\r' || c == '\n';
        }
    }
}
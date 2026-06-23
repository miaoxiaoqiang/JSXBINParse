using System;
using System.Collections.Generic;

namespace Parser.Nodes
{
    internal enum VariableTypeRange
    {
        kArguments = 0x20000000,
        kVars = 0x40000000,
        kConsts = 0x60000000,
    }

    public sealed class FunctionDeclaration : AstNode
    {
        private LineInfo _bodyInfo;
        private FunctionSignature _signature;
        private int _flags = 0;

        public FunctionDeclaration(Reader reader) : base(reader) { }

        public override NodeType Type => NodeType.FunctionDeclaration;

        public override void Parse()
        {
            _bodyInfo = DecodeLineInfo(Reader);
            _signature = Decoders.DFnSig(Reader);
            _flags = Decoders.DLiteralNum(Reader);
        }

        public override string GetString()
        {
            string body = _bodyInfo.CreateBody();
            var args = new List<string>();

            for (int i = 0; i < _signature.NumArgs; i++)
            {
                uint key = (uint)VariableTypeRange.kArguments + (uint)i;
                if (_signature.Variables.TryGetValue(key, out string varName))
                {
                    args.Add(varName);
                }
            }

            // 脚本闭包处理
            if ((_signature.Flags & 0x10000) != 0)
            {
                if (!string.IsNullOrEmpty(_signature.Name))
                {
                    string quote = Decoders.ValidId(_signature.Name) ? "" : "\"";
                    body = "#script " + quote + _signature.Name + quote + "\n" + body;
                }
                return body;
            }

            string argsString = string.Join(", ", args);
            return "function " + _signature.Name + "(" + argsString + ") {\n" + body + "\n}";
        }
    }
}
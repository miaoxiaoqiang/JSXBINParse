using System.Collections.Generic;

namespace Parser.Nodes
{
    public sealed class TryStatement : AstNode
    {
        private struct TryCatchLayer
        {
            public string Arg;
            public AstNode ExceptionFilter;
            public AstNode CatchBlock;
        }

        private LineInfo _tryBlock;
        private AstNode _finallyBlock;
        private List<TryCatchLayer> _layers = new List<TryCatchLayer>();
        private long _length;

        public TryStatement(Reader reader) : base(reader) { }

        public override NodeType Type => NodeType.TryStatement;

        public override void Parse()
        {
            _tryBlock = DecodeLineInfo(Reader);
            _length = Decoders.DLength(Reader);
            _finallyBlock = DecodeNode(Reader);

            for (int i = 0; i < _length; i++)
            {
                _layers.Add(new TryCatchLayer
                {
                    Arg = Decoders.DSid(Reader),
                    ExceptionFilter = DecodeNode(Reader),
                    CatchBlock = DecodeNode(Reader)
                });
            }
        }

        public override string GetString()
        {
            string result = _tryBlock.LblStatement() + "try {\n";
            result += _tryBlock.CreateBody() + "\n";

            foreach (var layer in _layers)
            {
                result += "} catch (" + layer.Arg;
                if (layer.ExceptionFilter != null)
                {
                    result += " if " + layer.ExceptionFilter.GetString();
                }

                result += ") {" + (layer.CatchBlock == null ? "" : layer.CatchBlock.GetString()) + "\n";
            }

            if (_finallyBlock != null)
            {
                result += "} finally {\n";
                result += _finallyBlock.GetString();
                result += "\n";
            }
            result += "}";
            return result;
        }
    }
}
using System.Collections.Generic;

namespace Parser.Nodes
{
    public sealed class StatementList : AstNode
    {
        private long _length;
        private LineInfo _body;
        private readonly List<AstNode> _statements = new List<AstNode>();

        public StatementList(Reader reader) : base(reader) { }

        public override NodeType Type => NodeType.StatementList;

        public override void Parse()
        {
            _body = DecodeLineInfo(Reader);

            _length = Decoders.DLength(Reader);
            for (int i = 0; i < _length; i++)
            {
                _statements.Add(DecodeNode(Reader));
            }

            var children = DecodeChildren(Reader);
            _statements.AddRange(children);
        }

        public override string GetString()
        {
            string result = "";

            for (int i = 0; i < _statements.Count; i++)
            {
                string expression = _statements[i].GetString();
                result += expression;
                if (i + 1 < _statements.Count)
                {
                    result += "\n";
                }
            }

            return _body.LblStatement() + result;
        }
    }
}
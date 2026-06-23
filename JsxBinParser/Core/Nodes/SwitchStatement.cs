using System.Collections.Generic;

namespace Parser.Nodes
{
    public sealed class SwitchStatement : AstNode
    {
        private LineInfo _lineInfo;
        private AstNode _switchValue;
        private List<AstNode> _cases = new List<AstNode>();
        private List<AstNode> _implementations = new List<AstNode>();

        public SwitchStatement(Reader reader) : base(reader) { }

        public override NodeType Type => NodeType.SwitchStatement;

        public override void Parse()
        {
            _lineInfo = DecodeLineInfo(Reader);
            _switchValue = DecodeNode(Reader);

            long lenCases = Decoders.DLength(Reader);
            for (int i = 0; i < lenCases; i++)
            {
                var node = DecodeNode(Reader);
                if (node != null)
                {
                    _cases.Add(node);
                }
            }

            long lenImplementations = Decoders.DLength(Reader);
            for (int i = 0; i < lenImplementations; i++)
            {
                var node = DecodeNode(Reader);
                if (node != null)
                {
                    _implementations.Add(node);
                }
            }
        }

        public override string GetString()
        {
            string result = "switch (" + _switchValue.GetString() + ") { \n";

            for (int i = 0; i < _cases.Count; i++)
            {
                var caseArgs = (_cases[i] as ListExpression)?.Arguments ?? new List<AstNode>();
                if (caseArgs.Count > 0)
                {
                    foreach (var arg in caseArgs)
                    {
                        result += "case " + arg.GetString() + ":\n";
                    }
                }
                else
                {
                    result += "default:\n";
                }

                if (i < _implementations.Count)
                {
                    result += _implementations[i].GetString() + "\n";
                }
            }
            result += "}";
            return result;
        }
    }
}
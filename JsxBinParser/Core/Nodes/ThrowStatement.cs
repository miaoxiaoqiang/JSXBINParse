namespace Parser.Nodes
{
    public sealed class ThrowStatement : AstNode
    {
        private LineInfo _lineInfo;
        private AstNode _expression;

        public ThrowStatement(Reader reader) : base(reader) { }

        public override NodeType Type => NodeType.ThrowStatement;

        public override void Parse()
        {
            _lineInfo = DecodeLineInfo(Reader);
            _expression = DecodeNode(Reader);
        }

        public override string GetString()
        {
            return "throw " + _expression.GetString() + ";";
        }
    }
}
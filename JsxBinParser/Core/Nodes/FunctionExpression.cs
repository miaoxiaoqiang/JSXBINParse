namespace Parser.Nodes
{
    public sealed class FunctionExpression : AstNode
    {
        private LineInfo _lineInfo;
        private AstNode _expression;

        public FunctionExpression(Reader reader) : base(reader) { }

        public override NodeType Type => NodeType.FunctionExpression;

        public override void Parse()
        {
            _lineInfo = DecodeLineInfo(Reader);
            _expression = DecodeNode(Reader);
        }

        public override string GetString()
        {
            return _expression.GetString();
        }
    }
}
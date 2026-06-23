namespace Parser.Nodes
{
    public sealed class ExpressionStatement : AstNode
    {
        private LineInfo _lineInfo;
        private AstNode _expression;

        public ExpressionStatement(Reader reader) : base(reader) { }

        public override NodeType Type => NodeType.ExpressionStatement;

        public override void Parse()
        {
            _lineInfo = DecodeLineInfo(Reader);
            _expression = DecodeNode(Reader);
        }

        public override string GetString()
        {
            return _lineInfo.LblStatement() + _expression.GetString() + ';';
        }
    }
}
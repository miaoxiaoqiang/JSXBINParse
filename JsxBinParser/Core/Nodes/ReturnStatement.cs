namespace Parser.Nodes
{
    public sealed class ReturnStatement : AstNode
    {
        private LineInfo _lineInfo;
        private AstNode _expression;

        public ReturnStatement(Reader reader) : base(reader) { }

        public override NodeType Type => NodeType.ReturnStatement;

        public override void Parse()
        {
            _lineInfo = DecodeLineInfo(Reader);
            _expression = DecodeNode(Reader);
        }

        public override string GetString()
        {
            string result = _expression == null ? "" : " " + _expression.GetString();
            return _lineInfo.LblStatement() + "return" + result + ";";
        }
    }
}
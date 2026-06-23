namespace Parser.Nodes
{
    public sealed class WhileStatement : AstNode
    {
        private LineInfo _bodyInfo;
        private AstNode _condition;

        public WhileStatement(Reader reader) : base(reader) { }

        public override NodeType Type => NodeType.WhileStatement;

        public override void Parse()
        {
            _bodyInfo = DecodeLineInfo(Reader);
            _condition = DecodeNode(Reader);
        }

        public override string GetString()
        {
            string result = _bodyInfo.LblStatement() + "while (" +
                            (_condition == null ? "true" : _condition.GetString()) + ") {\n";
            result += _bodyInfo.CreateBody() + "\n}";
            return result;
        }
    }
}
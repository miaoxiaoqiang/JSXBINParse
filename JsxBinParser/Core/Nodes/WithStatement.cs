namespace Parser.Nodes
{
    public sealed class WithStatement : AstNode
    {
        private LineInfo _bodyInfo;
        private AstNode _object;

        public WithStatement(Reader reader) : base(reader) { }

        public override NodeType Type => NodeType.WithStatement;

        public override void Parse()
        {
            _bodyInfo = DecodeLineInfo(Reader);
            _object = DecodeNode(Reader);
        }

        public override string GetString()
        {
            return _bodyInfo.LblStatement() + "with (" + _object.GetString() + ") {\n" + _bodyInfo.CreateBody() + "\n}";
        }
    }
}
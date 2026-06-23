namespace Parser.Nodes
{
    public sealed class DebuggerStatement : AstNode
    {
        private LineInfo _lineInfo;

        public DebuggerStatement(Reader reader) : base(reader) { }

        public override NodeType Type => NodeType.DebuggerStatement;

        public override void Parse()
        {
            _lineInfo = DecodeLineInfo(Reader);
        }

        public override string GetString()
        {
            return _lineInfo.LblStatement() + "debugger";
        }
    }
}
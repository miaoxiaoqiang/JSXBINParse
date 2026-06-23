namespace Parser.Nodes
{
    public sealed class ForStatement : AstNode
    {
        private LineInfo _bodyInfo;
        private AstNode _initial;
        private AstNode _test;
        private AstNode _update;

        public ForStatement(Reader reader) : base(reader) { }

        public override NodeType Type => NodeType.ForStatement;

        public override void Parse()
        {
            _bodyInfo = DecodeLineInfo(Reader);
            _initial = DecodeNode(Reader);

            // 如果 initial 是 ListExpression，设置 for_loop 标志
            if (_initial is ListExpression listExpr)
            {
                listExpr.SetForLoop(true);
            }

            _test = DecodeNode(Reader);
            _update = DecodeNode(Reader);
        }

        public override string GetString()
        {
            string result = _bodyInfo.LblStatement();
            result += "for (" + (_initial == null ? "" : _initial.GetString());
            result += "; " + (_test == null ? "" : _test.GetString());
            result += "; " + (_update == null ? "" : _update.GetString());
            result += ") { \n" + _bodyInfo.CreateBody() + '}';
            return result;
        }
    }
}
namespace Parser.Nodes
{
    public sealed class IfStatement : AstNode
    {
        private LineInfo _bodyInfo;
        private AstNode _test;
        private AstNode _otherwise;

        public IfStatement(Reader reader) : base(reader) { }

        public override NodeType Type => NodeType.IfStatement;

        public override void Parse()
        {
            _bodyInfo = DecodeLineInfo(Reader);
            _test = DecodeNode(Reader);
            _otherwise = DecodeNode(Reader);
        }

        public override string GetString()
        {
            string result = _bodyInfo.LblStatement() + "if (" + _test.GetString() + ") { \n"
                            + _bodyInfo.CreateBody() + "\n}";

            if (_otherwise == null)
            {
                return result;
            }

            var current = _otherwise;

            // 处理 else-if 链
            while (current.Type == NodeType.IfStatement && (current as IfStatement)?._otherwise != null)
            {
                var elif = current as IfStatement;
                result += "\n" + elif._bodyInfo.LblStatement() + "else if (" + elif._test.GetString() + ") {\n"
                          + elif._bodyInfo.CreateBody() + "\n}";
                current = elif._otherwise;
            }

            // 最后的 else
            result += "\nelse {\n" + current.GetString() + "\n}";

            return result;
        }
    }
}
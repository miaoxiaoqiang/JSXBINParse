namespace Parser.Nodes
{
    public sealed class DoWhileStatement : AstNode
    {
        private LineInfo _body;
        private AstNode _condition;

        public DoWhileStatement(Reader reader) : base(reader) { }

        public override NodeType Type => NodeType.DoWhileStatement;

        public override void Parse()
        {
            _body = DecodeLineInfo(Reader);
            _condition = DecodeNode(Reader);
        }

        public override string GetString()
        {
            string label = _body.LblStatement();
            string inner = _body.CreateBody();
            string result = label + "do {\n";
            result += "  " + inner + '\n';
            result += "} while (" + _condition.GetString() + ')';
            return result;
        }
    }
}
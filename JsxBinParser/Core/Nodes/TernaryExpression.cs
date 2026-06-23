namespace Parser.Nodes
{
    public sealed class TernaryExpression : AstNode
    {
        private AstNode _condition;
        private AstNode _nodeTrue;
        private AstNode _nodeFalse;

        public TernaryExpression(Reader reader) : base(reader) { }

        public override NodeType Type => NodeType.TernaryExpression;

        private static bool NeedParenthesis(AstNode node)
        {
            return node.Type == NodeType.TernaryExpression || node.Type == NodeType.ListExpression;
        }

        public override void Parse()
        {
            _condition = DecodeNode(Reader);
            _nodeTrue = DecodeNode(Reader);
            _nodeFalse = DecodeNode(Reader);
        }

        public override string GetString()
        {
            return _condition.GetString() + " ? " +
                   (NeedParenthesis(_nodeTrue) ? "(" + _nodeTrue.GetString() + ")" : _nodeTrue.GetString())
                   + " : "
                   + (NeedParenthesis(_nodeFalse) ? "(" + _nodeFalse.GetString() + ")" : _nodeFalse.GetString());
        }
    }
}
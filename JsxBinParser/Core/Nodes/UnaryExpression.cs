namespace Parser.Nodes
{
    public sealed class UnaryExpression : AstNode
    {
        private string _op;
        private AstNode _expression;

        public UnaryExpression(Reader reader) : base(reader)
        {

        }

        public override NodeType Type => NodeType.UnaryExpression;

        public override void Parse()
        {
            _op = Decoders.DOperator(Reader);
            _expression = DecodeNode(Reader);
        }

        public override string GetString()
        {
            bool needParenthesis = _expression.Type != NodeType.Identifier
                                   && _expression.Type != NodeType.LocalIdentifier
                                   && _expression.Type != NodeType.CallExpression
                                   && _expression.Type != NodeType.MemberExpression
                                   && _expression.Type != NodeType.IndexingExpression;

            // 如果是一元 "+"，直接忽略
            if (_op != "+")
            {
                return _op + (needParenthesis ? "(" + _expression.GetString() + ")" : _expression.GetString());
            }
            else
            {
                return needParenthesis ? "(" + _expression.GetString() + ")" : _expression.GetString();
            }
        }
    }
}
namespace Parser.Nodes
{
    public sealed class LogicalExpression : AstNode
    {
        private string _opName;
        private AstNode _leftExpr;
        private AstNode _rightExpr;
        private string _leftLiteral;
        private string _rightLiteral;

        public LogicalExpression(Reader reader) : base(reader) { }

        public override NodeType Type => NodeType.LogicalExpression;

        private static string GetExpr(AstNode node, string literal)
        {
            return "(" + (node == null ? literal : node.GetString()) + ")";
        }

        public override void Parse()
        {
            _opName = Decoders.DOperator(Reader);
            _leftExpr = DecodeNode(Reader);
            _rightExpr = DecodeNode(Reader);
            _leftLiteral = Decoders.DVariant(Reader);
            _rightLiteral = Decoders.DVariant(Reader);
        }

        public override string GetString()
        {
            return GetExpr(_leftExpr, _leftLiteral) + " " + _opName + " " + GetExpr(_rightExpr, _rightLiteral);
        }
    }
}
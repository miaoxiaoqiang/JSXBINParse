namespace Parser.Nodes
{
    public sealed class VoidExpression : AstNode
    {
        private AstNode _defaultNamespaceFxnCall;

        public VoidExpression(Reader reader) : base(reader) { }

        public override NodeType Type => NodeType.VoidExpression;

        public override void Parse()
        {
            _defaultNamespaceFxnCall = DecodeNode(Reader);
        }

        public override string GetString()
        {
            return _defaultNamespaceFxnCall.GetString();
        }
    }
}
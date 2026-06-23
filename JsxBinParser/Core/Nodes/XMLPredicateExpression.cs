namespace Parser.Nodes
{
    public sealed class XMLPredicateExpression : AstNode
    {
        private Reference _reference;
        private AstNode _object;
        private AstNode _member;

        public XMLPredicateExpression(Reader reader) : base(reader) { }

        public override NodeType Type => NodeType.XMLPredicateExpression;

        public override void Parse()
        {
            _reference = Decoders.DLiteralRef(Reader);
            _object = DecodeNode(Reader);
            _member = DecodeNode(Reader);
        }

        public override string GetString()
        {
            bool needParenthesis = _member.Type == NodeType.BinaryExpression
                                   || _member.Type == NodeType.LogicalExpression
                                   || _member.Type == NodeType.UnaryExpression;
            return _object.GetString() + "."
                   + (needParenthesis ? "(" + _member.GetString() + ")" : _member.GetString());
        }
    }
}
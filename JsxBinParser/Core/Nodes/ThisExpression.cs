namespace Parser.Nodes
{
    public sealed class ThisExpression : AstNode
    {
        private Reference _reference;

        public ThisExpression(Reader reader) : base(reader) { }

        public override NodeType Type => NodeType.ThisExpression;

        public override void Parse()
        {
            _reference = Decoders.DLiteralRef(Reader);
        }

        public override string GetString()
        {
            return "this";
        }
    }
}
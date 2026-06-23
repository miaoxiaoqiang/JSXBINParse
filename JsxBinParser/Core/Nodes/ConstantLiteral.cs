namespace Parser.Nodes
{
    public sealed class ConstantLiteral : AstNode
    {
        private string _value;

        public ConstantLiteral(Reader reader) : base(reader) { }

        public override NodeType Type => NodeType.ConstantLiteral;

        public override void Parse()
        {
            _value = Decoders.DVariant(Reader);
        }

        public override string GetString()
        {
            return _value;
        }
    }
}
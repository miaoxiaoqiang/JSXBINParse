namespace Parser.Nodes
{
    public sealed class UnaryRefExpression : AstNode
    {
        private string _name;
        private AstNode _argument;

        public UnaryRefExpression(Reader reader) : base(reader) { }

        public override NodeType Type => NodeType.UnaryRefExpression;

        public override void Parse()
        {
            _name = Decoders.DOperator(Reader);
            _argument = DecodeNode(Reader);
        }

        public override string GetString()
        {
            return _name + " " + _argument.GetString();
        }
    }
}
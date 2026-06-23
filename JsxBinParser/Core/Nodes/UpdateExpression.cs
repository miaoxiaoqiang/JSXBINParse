namespace Parser.Nodes
{
    public sealed class UpdateExpression : AstNode
    {
        private AstNode _variable;
        private int _operation = 0;
        private bool _postfix = false;

        public UpdateExpression(Reader reader) : base(reader) { }

        public override NodeType Type => NodeType.UpdateExpression;

        public override void Parse()
        {
            _variable = DecodeNode(Reader);
            _operation = Decoders.DLiteralNum(Reader);
            _postfix = Reader.GetBoolean();
        }

        public override string GetString()
        {
            string op = _operation == 1 ? "++" : "--";
            return _postfix ? _variable.GetString() + op : op + _variable.GetString();
        }
    }
}
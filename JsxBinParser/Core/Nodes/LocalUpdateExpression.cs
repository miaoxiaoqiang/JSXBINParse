namespace Parser.Nodes
{
    public sealed class LocalUpdateExpression : AstNode
    {
        private string _id;
        private long _length = 0;
        private string _operation;
        private bool _postfix = false;

        public LocalUpdateExpression(Reader reader) : base(reader) { }

        public override NodeType Type => NodeType.LocalUpdateExpression;

        public override void Parse()
        {
            _id = Decoders.DSid(Reader);
            _length = Decoders.DLength(Reader);
            _operation = Decoders.DNumber(Reader);
            _postfix = Reader.GetBoolean();
        }

        public override string GetString()
        {
            string op = _operation == "1" ? "++" : "--";
            return _postfix ? _id + op : op + _id;
        }
    }
}
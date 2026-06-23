namespace Parser.Nodes
{
    public sealed class ConstAssignment : AstNode
    {
        private string _name;
        private long _length;      // 原 size_t，但未使用
        private AstNode _expression;
        private string _literal;
        private bool _boolean1;
        private bool _boolean2;

        public ConstAssignment(Reader reader) : base(reader) { }

        public override NodeType Type => NodeType.ConstAssignment;

        public override void Parse()
        {
            _name = Decoders.DSid(Reader);
            _length = Decoders.DLength(Reader);
            _expression = DecodeNode(Reader);
            _literal = Decoders.DVariant(Reader);
            _boolean1 = Reader.GetBoolean();
            _boolean2 = Reader.GetBoolean();
        }

        public override string GetString()
        {
            return "const " + _name + " = "
                   + (_expression == null ? _literal : _expression.GetString());
        }
    }
}
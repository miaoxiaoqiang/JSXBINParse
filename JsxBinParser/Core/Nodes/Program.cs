namespace Parser.Nodes
{
    public sealed class Program : AstNode
    {
        private AstNode _body;

        public Program(Reader reader) : base(reader) { }

        public override NodeType Type => NodeType.Program;

        public override void Parse()
        {
            _body = DecodeNode(Reader);
        }

        public override string GetString()
        {
            return _body.GetString();
        }
    }
}
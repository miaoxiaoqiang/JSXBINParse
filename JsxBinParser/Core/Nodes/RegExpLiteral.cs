namespace Parser.Nodes
{
    public sealed class RegExpLiteral : AstNode
    {
        private string _regex;
        private string _flags;

        public RegExpLiteral(Reader reader) : base(reader) { }

        public override NodeType Type => NodeType.RegExpLiteral;

        public override void Parse()
        {
            _regex = Utils.ToString(Reader.GetString());
            _flags = Utils.ToString(Reader.GetString());
        }

        public override string GetString()
        {
            return "/" + _regex + "/" + _flags;
        }
    }
}
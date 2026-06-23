namespace Parser.Nodes
{
    public sealed class Identifier : AstNode
    {
        private string _id;
        private bool _unknown = false;

        public Identifier(Reader reader) : base(reader) { }

        public override NodeType Type => NodeType.Identifier;

        public override void Parse()
        {
            _id = Decoders.DSid(Reader);
            if (Reader.Version >= JsxbinVersion.v20)
            {
                _unknown = Reader.GetBoolean();
            }
        }

        public override string GetString()
        {
            return _id;
        }
    }
}
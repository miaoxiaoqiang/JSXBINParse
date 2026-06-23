namespace Parser.Nodes
{
    public sealed class LocalIdentifier : AstNode
    {
        private Reference _reference;
        private int _type = 0;

        public LocalIdentifier(Reader reader) : base(reader) { }

        public override NodeType Type => NodeType.LocalIdentifier;

        public override void Parse()
        {
            _reference = Decoders.DIdRef(Reader);
            _type = (int)Decoders.DLength(Reader);
        }

        public override string GetString()
        {
            return Utils.ToString(_reference.Id);
        }
    }
}
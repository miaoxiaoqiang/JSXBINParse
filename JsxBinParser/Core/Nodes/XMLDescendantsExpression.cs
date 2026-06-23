namespace Parser.Nodes
{
    public sealed class XMLDescendantsExpression : AstNode
    {
        private Reference _descendants;
        private AstNode _object;

        public XMLDescendantsExpression(Reader reader) : base(reader) { }

        public override NodeType Type => NodeType.XMLDescendantsExpression;

        public override void Parse()
        {
            _descendants = Decoders.DLiteralRef(Reader);
            _object = DecodeNode(Reader);
            // 两个未使用的节点
            DecodeNode(Reader);
            DecodeNode(Reader);
        }

        public override string GetString()
        {
            return _object.GetString() + ".." + Utils.ToString(_descendants.Id);
        }
    }
}
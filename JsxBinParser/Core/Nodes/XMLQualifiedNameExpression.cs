namespace Parser.Nodes
{
    public sealed class XMLQualifiedNameExpression : AstNode
    {
        private Reference _namespaceObject;
        private AstNode _object;
        private string _xmlId;

        public XMLQualifiedNameExpression(Reader reader) : base(reader) { }

        public override NodeType Type => NodeType.XMLQualifiedNameExpression;

        public override void Parse()
        {
            _namespaceObject = Decoders.DLiteralRef(Reader);
            _object = DecodeNode(Reader);
            // 两个未使用的节点
            DecodeNode(Reader);
            DecodeNode(Reader);
            _xmlId = Decoders.DSid(Reader);
        }

        public override string GetString()
        {
            string nsId = Utils.ToString(_namespaceObject.Id);
            string ns = _namespaceObject.Flag ? "@" + nsId : nsId;
            return _object.GetString() + "." + ns + "::" + _xmlId;
        }
    }
}
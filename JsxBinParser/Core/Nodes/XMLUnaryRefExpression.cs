namespace Parser.Nodes
{
    public sealed class XMLUnaryRefExpression : AstNode
    {
        private string _id;
        private AstNode _node;

        public XMLUnaryRefExpression(Reader reader) : base(reader) { }

        public override NodeType Type => NodeType.XMLUnaryRefExpression;

        public override void Parse()
        {
            _id = Decoders.DSid(Reader);
            _node = DecodeNode(Reader);
        }

        public override string GetString()
        {
            return "// XMLUnaryRefExpression has no known syntactic representation.\n" +
                   "// To help add this, please create an issue on the Jsxer GitHub repository with the binary version\n" +
                   "// of this script attached, so that it can be researched. Thank you! <3\n" +
                   "// Create an issue here: https://github.com/AngeloD2022/jsxer";
        }
    }
}
namespace Parser.Nodes
{
    public sealed class IndexingExpression : AstNode
    {
        private string _arrayName;
        private string _expression;

        public IndexingExpression(Reader reader) : base(reader) { }

        public override NodeType Type => NodeType.IndexingExpression;

        public override void Parse()
        {
            // 原 C++ 中读取了 ref 但未使用，仅消费数据
            var refData = Decoders.DLiteralRef(Reader);
            var name = DecodeNode(Reader);
            var expr = DecodeNode(Reader);

            _arrayName = name.GetString();
            _expression = expr.GetString();
        }

        public override string GetString()
        {
            return _arrayName + "[" + _expression + "]";
        }
    }
}
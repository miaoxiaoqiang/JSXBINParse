namespace Parser.Nodes
{
    /// <summary>
    /// 对应 C++ 的 ArrayExpression 节点
    /// </summary>
    public sealed class ArrayExpression : AstNode
    {
        private ListExpression _argumentList;

        public ArrayExpression(Reader reader) : base(reader) { }

        public override NodeType Type => NodeType.ArrayExpression;

        public override void Parse()
        {
            AstNode node = DecodeNode(Reader);
            _argumentList = node as ListExpression;
        }

        public override string GetString()
        {
            if (_argumentList == null)
            {
                return "[]";
            }

            return "[" + _argumentList.GetString() + "]";
        }

        public ListExpression ArgumentList => _argumentList;
    }
}
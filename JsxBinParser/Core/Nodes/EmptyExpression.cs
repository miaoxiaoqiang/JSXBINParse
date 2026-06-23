namespace Parser.Nodes
{
    public sealed class EmptyExpression : AstNode
    {
        public EmptyExpression(Reader reader) : base(reader) { }

        public override NodeType Type => NodeType.EmptyExpression;

        public override void Parse()
        {
            // EmptyExpression 没有额外数据需要解析
        }

        public override string GetString()
        {
            // 空表达式在输出中通常不产生任何文本
            // 但根据原 C++ 注释，它可能用于某些占位场景
            // 这里返回空字符串，确保不破坏输出格式
            return "";
        }
    }
}
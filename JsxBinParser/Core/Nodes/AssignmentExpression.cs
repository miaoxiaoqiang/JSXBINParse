namespace Parser.Nodes
{
    /// <summary>
    /// 对应 C++ 的 AssignmentExpression 节点
    /// </summary>
    public sealed class AssignmentExpression : AstNode
    {
        private AstNode _variable;
        private AstNode _expression;
        private string _literal;
        private bool _shorthand;

        public AssignmentExpression(Reader reader) : base(reader) { }

        public override NodeType Type => NodeType.AssignmentExpression;

        public override void Parse()
        {
            _variable = DecodeNode(Reader);
            _expression = DecodeNode(Reader);
            _literal = Decoders.DVariant(Reader);
            _shorthand = Reader.GetBoolean();
        }

        public override string GetString()
        {
            if (_shorthand)
            {
                // 尝试将 expression 转换为 BinaryExpression
                if (_expression is BinaryExpression binaryExpr)
                {
                    string valueAssigned = string.IsNullOrEmpty(_literal) ? binaryExpr.GetOp() : _literal;
                    return _variable.GetString() + ' ' + binaryExpr.GetOpName() + "= " + valueAssigned;
                }
                else
                {
                    // 如果转换失败（理论上不应该发生），回退到普通赋值
                    string valueAssigned = string.IsNullOrEmpty(_literal) ? _expression?.GetString() ?? "" : _literal;
                    return _variable.GetString() + " = " + valueAssigned;
                }
            }

            string normalValueAssigned = string.IsNullOrEmpty(_literal) ? _expression?.GetString() ?? "" : _literal;
            return _variable.GetString() + " = " + normalValueAssigned;
        }

        public AstNode Variable => _variable;
        public AstNode Expression => _expression;
        public string Literal => _literal;
        public bool Shorthand => _shorthand;
    }
}
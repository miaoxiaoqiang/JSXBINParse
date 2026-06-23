namespace Parser.Nodes
{
    /// <summary>
    /// 对应 C++ 的 LocalAssignmentExpression 节点
    /// </summary>
    public sealed class LocalAssignmentExpression : AstNode
    {
        private string _varName;
        private string _literal;
        private AstNode _expression;
        private bool _declarativeSuppress = false;
        private bool _shorthand = false;
        private bool _declaration = false;

        public LocalAssignmentExpression(Reader reader) : base(reader) { }

        public override NodeType Type => NodeType.LocalAssignmentExpression;

        public override void Parse()
        {
            _varName = Decoders.DSid(Reader);
            // 读取长度（原代码中命名为 type，但未使用，仅消费数据）
            var type = Decoders.DLength(Reader);
            _expression = DecodeNode(Reader);
            _literal = Decoders.DVariant(Reader);
            _shorthand = Reader.GetBoolean();
            _declaration = Reader.GetBoolean();
        }

        public override string GetString()
        {
            string result = (_declaration && !_declarativeSuppress) ? "var " : "";

            if (_shorthand)
            {
                // 尝试将 expression 转换为 BinaryExpression
                if (_expression as BinaryExpression != null)
                {
                    string valueAssigned = string.IsNullOrEmpty(_literal) ? (_expression as BinaryExpression).GetOp() : _literal;
                    result += _varName + ' ' + (_expression as BinaryExpression).GetOpName() + "= " + valueAssigned;
                }
                else
                {
                    // 如果转换失败（理论上不应该发生），回退到普通赋值
                    string valueAssigned = string.IsNullOrEmpty(_literal) ? _expression?.GetString() ?? "" : _literal;
                    result += _varName + " = " + valueAssigned;
                }
                return result;
            }

            string valueAssignedNormal = string.IsNullOrEmpty(_literal) ? _expression?.GetString() ?? "" : _literal;
            result += _varName + " = " + valueAssignedNormal;
            return result;
        }

        public void SuppressDeclarativeKeyword(bool value)
        {
            _declarativeSuppress = value;
        }
    }
}
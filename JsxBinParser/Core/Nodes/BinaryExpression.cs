using System;

namespace Parser.Nodes
{
    /// <summary>
    /// 对应 C++ 的 BinaryExpression 节点
    /// </summary>
    public sealed class BinaryExpression : AstNode
    {
        private string _opName;
        private string _op;
        private AstNode _left;
        private AstNode _right;
        private string _literalLeft;
        private string _literalRight;

        public BinaryExpression(Reader reader) : base(reader) { }

        public override NodeType Type => NodeType.BinaryExpression;

        public override void Parse()
        {
            // 解码操作符名称（注意：原代码先获取 op_name，之后通过它构建 op）
            _opName = Decoders.DOperator(Reader);

            // 解码左右子节点
            _left = DecodeNode(Reader);
            _right = DecodeNode(Reader);

            // 解码左右字面量（变体）
            _literalLeft = Decoders.DVariant(Reader);
            _literalRight = Decoders.DVariant(Reader);

            // 构建左右表达式字符串
            string leftExp = CreateExpr(_literalLeft, _left);
            string rightExp = CreateExpr(_literalRight, _right);

            // 根据原 C++ 逻辑：如果一侧为空，则直接拼接（可能用于一元操作符），否则用空格分隔操作符
            if ((!string.IsNullOrEmpty(leftExp) && string.IsNullOrEmpty(rightExp)) ||
                (string.IsNullOrEmpty(leftExp) && !string.IsNullOrEmpty(rightExp)))
            {
                _op = leftExp + rightExp;
            }
            else
            {
                _op = leftExp + ' ' + _opName + ' ' + rightExp;
            }
        }

        public override string GetString()
        {
            return _op;
        }

        /// <summary>
        /// 获取操作符字符串
        /// </summary>
        public string GetOp() => _op;

        /// <summary>
        /// 获取操作符名称
        /// </summary>
        public string GetOpName() => _opName;

        // 属性版本
        public string Op => _op;

        public string OpName => _opName;

        /// <summary>
        /// 创建表达式的字符串表示，根据需要添加括号
        /// </summary>
        private string CreateExpr(string literal, AstNode exprNode)
        {
            bool needParenthesis = false;
            string expression = "";

            if (exprNode != null && exprNode.Type == NodeType.BinaryExpression)
            {
                // 尝试转换为 BinaryExpression 以获取其操作符名称
                if (exprNode as BinaryExpression != null)
                {
                    expression = (exprNode as BinaryExpression).GetOp();
                    // 检查是否具有结合性：如果左右操作符相同且为 '*' 或 '+'
                    bool associative = (string.Equals((exprNode as BinaryExpression).GetOpName(), "*", StringComparison.Ordinal) &&
                                        string.Equals(_opName, "*", StringComparison.Ordinal)) ||
                                       (string.Equals((exprNode as BinaryExpression).GetOpName(), "+", StringComparison.Ordinal) &&
                                        string.Equals(_opName, "+", StringComparison.Ordinal));
                    needParenthesis = !associative;
                }
            }
            else if (exprNode != null &&
                     (exprNode.Type == NodeType.LocalAssignmentExpression ||
                      exprNode.Type == NodeType.AssignmentExpression))
            {
                needParenthesis = true;
                expression = exprNode.GetString();
            }
            else
            {
                expression = exprNode == null ? literal : exprNode.GetString();
            }

            return needParenthesis ? "(" + expression + ")" : expression;
        }
    }
}
using System.Collections.Generic;
using System.Text;

namespace Parser.Nodes
{
    public sealed class ListExpression : AstNode
    {
        private bool _forLoop = false;
        private bool _sequenceExpr = false;

        public List<AstNode> Arguments { get; set; } = new List<AstNode>();

        public ListExpression(Reader reader) : base(reader) { }

        public override NodeType Type => NodeType.ListExpression;

        public override void Parse()
        {
            // 解码子节点列表
            Arguments = DecodeChildren(Reader);
            // 读取 sequence_expr 标志
            _sequenceExpr = Reader.GetBoolean();
        }

        public override string GetString()
        {
            var result = new StringBuilder();
            string delimiter = ", ";

            // 遍历参数
            for (int i = 0; i < Arguments.Count; i++)
            {
                var arg = Arguments[i];

                // 如果是 for 循环上下文，且参数是 LocalAssignmentExpression，则抑制声明关键字
                if (_forLoop && i > 0 && arg.Type == NodeType.LocalAssignmentExpression)
                {
                    var localAssign = arg as LocalAssignmentExpression;
                    localAssign?.SuppressDeclarativeKeyword(true);
                }

                result.Append(arg.GetString());
                if (i + 1 < Arguments.Count)
                {
                    result.Append(delimiter);
                }
            }

            string output = result.ToString();

            // 如果 sequence_expr 为 true 且不是 for 循环，则用括号包裹
            if (_sequenceExpr && !_forLoop)
            {
                output = "(" + output + ")";
            }

            return output;
        }

        public void SetForLoop(bool value)
        {
            _forLoop = value;
        }
    }
}
namespace Parser.Nodes
{
    public sealed class MemberExpression : AstNode
    {
        private Reference _memberInfo;
        private AstNode _objInfo;

        public MemberExpression(Reader reader) : base(reader) { }

        public override NodeType Type => NodeType.MemberExpression;

        public override void Parse()
        {
            _memberInfo = Decoders.DLiteralRef(Reader);
            _objInfo = DecodeNode(Reader);
        }

        public override string GetString()
        {
            string result = (_objInfo == null ? "" : _objInfo.GetString());

            // 如果是数字或二元表达式，加括号
            if (Decoders.IsInteger(result) || (_objInfo != null && _objInfo.Type == NodeType.BinaryExpression))
            {
                result = "(" + result + ")";
            }

            if (_objInfo != null && (_objInfo.Type == NodeType.AssignmentExpression ||
                                     _objInfo.Type == NodeType.LocalAssignmentExpression))
            {
                result = "(" + result + ")";
            }

            // 检查成员ID是否合法
            if (Decoders.ValidId(_memberInfo.Id))
            {
                result += "." + Utils.ToString(_memberInfo.Id);
                return result;
            }

            if (Decoders.ValidXmlAttribute(_memberInfo.Id))
            {
                result += "." + Utils.ToString(_memberInfo.Id);
                return result;
            }

            result += "[";
            if (Decoders.IsInteger(_memberInfo.Id))
            {
                result += Utils.ToString(_memberInfo.Id);
            }
            else
            {
                result += Utils.ToStringLiteral(_memberInfo.Id);
            }
            result += "]";

            return result;
        }
    }
}
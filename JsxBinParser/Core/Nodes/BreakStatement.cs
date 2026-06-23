namespace Parser.Nodes
{
    /// <summary>
    /// 对应 C++ 的 BreakStatement 节点
    /// </summary>
    public sealed class BreakStatement : AstNode
    {
        private LineInfo _labelInfo;
        private string _jmpLocation;
        private bool _breakStatement = false;

        public BreakStatement(Reader reader) : base(reader) { }

        public override NodeType Type => NodeType.BreakStatement;

        public override void Parse()
        {
            _labelInfo = DecodeLineInfo(Reader);
            _jmpLocation = Decoders.DSid(Reader);
            _breakStatement = Reader.GetBoolean();
        }

        public override string GetString()
        {
            string result = _labelInfo.LblStatement();

            if (_breakStatement)
            {
                result += "break ";
            }
            else
            {
                result += "continue ";
            }

            result += _jmpLocation + ';';

            return result;
        }

        // 可选：提供对私有成员的访问
        public LineInfo LabelInfo => _labelInfo;

        public string JmpLocation => _jmpLocation;

        public bool IsBreakStatement => _breakStatement;
    }
}
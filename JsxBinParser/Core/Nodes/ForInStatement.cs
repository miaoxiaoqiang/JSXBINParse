namespace Parser.Nodes
{
    public sealed class ForInStatement : AstNode
    {
        private LineInfo _bodyInfo;
        private AstNode _loopVariable;
        private AstNode _objExpression;
        private long _length;
        private string _id;
        private bool _forEach = false;

        public ForInStatement(Reader reader) : base(reader) { }

        public override NodeType Type => NodeType.ForInStatement;

        public override void Parse()
        {
            _bodyInfo = DecodeLineInfo(Reader);
            _loopVariable = DecodeNode(Reader);
            _objExpression = DecodeNode(Reader);
            _length = Decoders.DLength(Reader);
            _id = Decoders.DSid(Reader);

            if (Reader.Version >= JsxbinVersion.v20)
            {
                _forEach = Reader.GetBoolean();
            }
        }

        public override string GetString()
        {
            string result = _bodyInfo.LblStatement();
            result += _forEach ? "for each (var " : "for (var ";
            result += _loopVariable.GetString();
            result += " in ";
            result += _objExpression.GetString();
            result += ") { \n" + _bodyInfo.CreateBody() + "\n}";
            return result;
        }
    }
}
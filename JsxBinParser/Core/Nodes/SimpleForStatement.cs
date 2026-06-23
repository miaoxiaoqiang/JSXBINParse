using System;

namespace Parser.Nodes
{
    public sealed class SimpleForStatement : AstNode
    {
        private LineInfo _bodyInfo;
        private AstNode _loopVar;
        private string _iteratorInitial;
        private AstNode _upperBound;
        private string _stepSize;
        private long _length;
        private string _comparisonOperator;

        public SimpleForStatement(Reader reader) : base(reader) { }

        public override NodeType Type => NodeType.SimpleForStatement;

        public override void Parse()
        {
            _bodyInfo = DecodeLineInfo(Reader);
            _loopVar = DecodeNode(Reader);
            _iteratorInitial = Decoders.DNumber(Reader);
            _upperBound = DecodeNode(Reader);
            _stepSize = Decoders.DNumber(Reader);
            _length = Decoders.DLength(Reader);
            _comparisonOperator = Decoders.DSid(Reader);
        }

        public override string GetString()
        {
            string varname = _loopVar.GetString();
            string result = _bodyInfo.LblStatement();
            result += "for (var ";
            result += varname + " = " + _iteratorInitial + "; ";
            result += varname + " " + _comparisonOperator + " " + _upperBound.GetString() + "; ";
            result += varname + " += " + _stepSize;
            result += ") { \n" + _bodyInfo.CreateBody() + "}";
            return result;
        }
    }
}
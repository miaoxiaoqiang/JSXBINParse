namespace Parser.Nodes
{
    public sealed class CallExpression : AstNode
    {
        private AstNode _function;
        private AstNode _args;
        private bool _constructorCall;

        public CallExpression(Reader reader) : base(reader) { }

        public override NodeType Type => NodeType.CallExpression;

        public override void Parse()
        {
            _function = DecodeNode(Reader);
            _args = DecodeNode(Reader);
            _constructorCall = Reader.GetBoolean();
        }

        public override string GetString()
        {
            string functionName = _function.GetString();
            var arguments = _args as ListExpression;
            bool needWrap = _function.Type == NodeType.FunctionExpression;

            if (functionName == "eval" && arguments != null && arguments.Arguments.Count == 1 && !_constructorCall)
            {
                string payload = Utils.FromStringLiteral(arguments.Arguments[0].GetString());
                Reader internalReader = new Reader(payload, Reader.ShouldUnblind);

                if (internalReader.VerifySignature())
                {
                    Program internalAst = new Program(internalReader)
                    {
                        PrintStructure = PrintStructure
                    };
                    internalAst.Parse();

                    string result1 = internalAst.GetString();
                    if (result1.EndsWith(";"))
                    {
                        result1 = result1.Substring(0, result1.Length - 1);
                    }

                    return result1;
                }
            }

            string result = (_constructorCall ? "new " : "");
            result += (needWrap ? "(" : "")
                      + _function.GetString()
                      + (needWrap ? ")" : "")
                      + (arguments != null ? "(" + arguments.GetString() + ")" : "()");
            return result;
        }
    }
}
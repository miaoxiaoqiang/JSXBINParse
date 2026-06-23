namespace Parser.Nodes
{
    public enum NodeType : byte
    {
        Invalid = 255, // (uint8_t)-1
        Program = 0,   // '\x00'
        ArrayExpression      = 65,  // 'A'
        AssignmentExpression = 66,  // 'B'
        BinaryExpression     = 67,  // 'C'
        BreakStatement       = 68,  // 'D'
        CallExpression       = 69,  // 'E'
        ConstantLiteral      = 70,  // 'F'
        ConstAssignment      = 71,  // 'G'
        DebuggerStatement    = 72,  // 'H'
        DoWhileStatement     = 73,  // 'I'
        ExpressionStatement  = 74,  // 'J'
        ForStatement         = 75,  // 'K'
        ForInStatement       = 76,  // 'L'
        FunctionDeclaration  = 77,  // 'M'
        FunctionExpression   = 78,  // 'N'
        IfStatement          = 79,  // 'O'
        UpdateExpression     = 80,  // 'P'
        IndexingExpression   = 81,  // 'Q'
        ListExpression       = 82,  // 'R'
        LocalAssignmentExpression = 83, // 'S'
        LocalUpdateExpression      = 84, // 'T'
        LogicalExpression    = 85,  // 'U'
        LocalIdentifier      = 86,  // 'V'
        ObjectExpression     = 87,  // 'W'
        MemberExpression     = 88,  // 'X'
        RegExpLiteral        = 89,  // 'Y'
        ReturnStatement      = 90,  // 'Z'
        SimpleForStatement   = 97,  // 'a'
        StatementList        = 98,  // 'b'
        SwitchStatement      = 99,  // 'c'
        TernaryExpression    = 100, // 'd'
        ThisExpression       = 101, // 'e'
        ThrowStatement       = 102, // 'f'
        TryStatement         = 103, // 'g'
        UnaryExpression      = 104, // 'h'
        UnaryRefExpression   = 105, // 'i'
        Identifier           = 106, // 'j'
        VoidExpression       = 107, // 'k'
        WhileStatement       = 108, // 'l'
        WithStatement        = 109, // 'm'
        EmptyExpression      = 110, // 'n'
        XMLConstantExpression    = 111, // 'o'
        XMLQualifiedNameExpression = 112, // 'p'
        XMLDescendantsExpression = 113, // 'q'
        XMLPredicateExpression   = 114, // 'r'
        XMLUnaryRefExpression    = 115, // 's'
    }

    public static class NodeFactory
    {
        public static AstNode Get(NodeType type, Reader reader)
        {
            switch (type)
            {
                case NodeType.ArrayExpression: return new ArrayExpression(reader);
                case NodeType.AssignmentExpression: return new AssignmentExpression(reader);
                case NodeType.BinaryExpression: return new BinaryExpression(reader);
                case NodeType.BreakStatement: return new BreakStatement(reader);
                case NodeType.CallExpression: return new CallExpression(reader);
                case NodeType.ConstantLiteral: return new ConstantLiteral(reader);
                case NodeType.ConstAssignment: return new ConstAssignment(reader);
                case NodeType.DebuggerStatement: return new DebuggerStatement(reader);
                case NodeType.DoWhileStatement: return new DoWhileStatement(reader);
                case NodeType.ExpressionStatement: return new ExpressionStatement(reader);
                case NodeType.ForStatement: return new ForStatement(reader);
                case NodeType.ForInStatement: return new ForInStatement(reader);
                case NodeType.FunctionDeclaration: return new FunctionDeclaration(reader);
                case NodeType.FunctionExpression: return new FunctionExpression(reader);
                case NodeType.IfStatement: return new IfStatement(reader);
                case NodeType.UpdateExpression: return new UpdateExpression(reader);
                case NodeType.IndexingExpression: return new IndexingExpression(reader);
                case NodeType.ListExpression: return new ListExpression(reader);
                case NodeType.LocalAssignmentExpression: return new LocalAssignmentExpression(reader);
                case NodeType.LocalUpdateExpression: return new LocalUpdateExpression(reader);
                case NodeType.LogicalExpression: return new LogicalExpression(reader);
                case NodeType.LocalIdentifier: return new LocalIdentifier(reader);
                case NodeType.ObjectExpression: return new ObjectExpression(reader);
                case NodeType.MemberExpression: return new MemberExpression(reader);
                case NodeType.RegExpLiteral: return new RegExpLiteral(reader);
                case NodeType.ReturnStatement: return new ReturnStatement(reader);
                case NodeType.SimpleForStatement: return new SimpleForStatement(reader);
                case NodeType.StatementList: return new StatementList(reader);
                case NodeType.SwitchStatement: return new SwitchStatement(reader);
                case NodeType.TernaryExpression: return new TernaryExpression(reader);
                case NodeType.ThisExpression: return new ThisExpression(reader);
                case NodeType.ThrowStatement: return new ThrowStatement(reader);
                case NodeType.TryStatement: return new TryStatement(reader);
                case NodeType.UnaryExpression: return new UnaryExpression(reader);
                case NodeType.UnaryRefExpression: return new UnaryRefExpression(reader);
                case NodeType.Identifier: return new Identifier(reader);
                case NodeType.VoidExpression: return new VoidExpression(reader);
                case NodeType.WhileStatement: return new WhileStatement(reader);
                case NodeType.WithStatement: return new WithStatement(reader);
                case NodeType.XMLConstantExpression: return new XMLConstantExpression(reader);
                case NodeType.XMLQualifiedNameExpression: return new XMLQualifiedNameExpression(reader);
                case NodeType.XMLDescendantsExpression: return new XMLDescendantsExpression(reader);
                case NodeType.XMLPredicateExpression: return new XMLPredicateExpression(reader);
                case NodeType.XMLUnaryRefExpression: return new XMLUnaryRefExpression(reader);
                // 其他节点（如需要，继续添加）
                default: return null;
            }
        }
    }
}
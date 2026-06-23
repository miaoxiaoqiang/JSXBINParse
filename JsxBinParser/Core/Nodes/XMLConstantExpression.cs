using System.Collections.Generic;

namespace Parser.Nodes
{
    public sealed class XMLConstantExpression : AstNode
    {
        private Dictionary<AstNode, ulong> _children = new Dictionary<AstNode, ulong>();

        public XMLConstantExpression(Reader reader) : base(reader) { }

        public override NodeType Type => NodeType.XMLConstantExpression;

        public override void Parse()
        {
            long length = Decoders.DLength(Reader);
            for (int i = 0; i < length; i++)
            {
                AstNode childNode = DecodeNode(Reader);
                long childLength = Decoders.DLength(Reader);
                _children[childNode] = (ulong)childLength;
            }
        }

        public override string GetString()
        {
            const int TYPE_NORMAL = 0;
            //const int TYPE_ELEM_PLACEHOLDER = 1;
            //const int TYPE_ATTR_PLACEHOLDER = 2;
            //const int TYPE_VALUE_PLACEHOLDER = 3;

            var normals = new List<AstNode>();
            var placeholders = new List<AstNode>();

            foreach (var kv in _children)
            {
                if (kv.Value == TYPE_NORMAL)
                {
                    normals.Add(kv.Key);
                }
                else
                {
                    placeholders.Add(kv.Key);
                }
            }

            if (normals.Count != placeholders.Count + 1 && _children.Count > 1)
            {
                return "// Jsxer: XMLConstantExpression syntax recovery failed.";
            }

            string result = "";
            int normalIdx = 0, placeholderIdx = 0;
            for (int i = 0; i < _children.Count; i++)
            {
                if ((i & 1) == 0) // even index
                {
                    var normal = normals[normalIdx++];
                    result += Utils.FromStringLiteral(normal.GetString());
                }
                else
                {
                    string placeholder = placeholders[placeholderIdx++].GetString();
                    if (placeholder.Length > 0)
                    {
                        placeholder = placeholder.Substring(0, placeholder.Length - 1);
                    }

                    result += "{" + placeholder + "}";
                }
            }
            return result;
        }
    }
}
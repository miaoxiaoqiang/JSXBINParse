using System.Collections.Generic;

namespace Parser.Nodes
{
    public sealed class ObjectExpression : AstNode
    {
        private string _objectId;
        private Dictionary<string, AstNode> _properties = new Dictionary<string, AstNode>();

        public ObjectExpression(Reader reader) : base(reader) { }

        public override NodeType Type => NodeType.ObjectExpression;

        public override void Parse()
        {
            _objectId = Decoders.DSid(Reader);

            long childCount = Decoders.DLength(Reader);

            for (int i = 0; i < childCount; i++)
            {
                string id = Decoders.DSid(Reader);
                AstNode node = DecodeNode(Reader);
                _properties[id] = node;
            }
        }

        public override string GetString()
        {
            string result = "{";

            if (_properties.Count > 0)
            {
                int i = 0;
                foreach (var entry in _properties)
                {
                    if (!Decoders.ValidId(entry.Key))
                    {
                        result += Utils.ToStringLiteral(entry.Key);
                    }
                    else
                    {
                        result += entry.Key;
                    }

                    result += ": " + entry.Value.GetString();

                    if (i + 1 < _properties.Count)
                    {
                        result += ", ";
                    }

                    i++;
                }
            }

            return result + "}";
        }
    }
}
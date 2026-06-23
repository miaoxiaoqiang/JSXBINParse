using Parser.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Parser.Nodes
{
    public abstract class AstNode
    {
        private readonly static List<(AstNode Node, int Level)> _nodeList = new List<(AstNode, int)>();
        //private static readonly StringBuilder _builder = new StringBuilder();

        protected Reader Reader
        {
            get;
        }

        protected AstNode(Reader reader)
        {
            Reader = reader ?? throw new ArgumentNullException(nameof(reader));
        }

        public abstract NodeType Type
        {
            get;
        }

        public abstract string GetString();

        public abstract void Parse();

        public virtual int IndentLevel
        {
            get;
            set;
        }

        public virtual bool PrintStructure
        {
            get;
            set;
        }

        public static Tuple<string, List<Model.TreeStructure>> Decode(string jsxbintext, bool unblind = false, bool printStructure = false)
        {
            Reader reader = new Reader(jsxbintext, unblind);
            _nodeList.Clear();
            //_builder.Clear();

            if (!reader.VerifySignature())
            {
                throw new Exception("[!]: The input file has an invalid signature.\r\nJSXBIN signature verification failed!");
            }

            Program ast = new Program(reader)
            {
                PrintStructure = printStructure
            };
            ast.Parse();

            string code = ast.GetString();
            string output = PrependHeader(code, reader.Version, unblind);

            return Tuple.Create(output, printStructure ? BuildTreeFromLevels() : null);
        }

        protected AstNode DecodeNode(Reader reader)
        {
            byte marker = reader.Get();
            AstNode node = NodeFactory.Get((NodeType)marker, reader);

            if (node != null)
            {
                bool ignoreHeaderFunction = Type == NodeType.Program;
                if (PrintStructure && !ignoreHeaderFunction)
                {
                    //System.Diagnostics.Debug.WriteLine(new string(' ', 4 * IndentLevel) + node.Type.ToString());
                    node.IndentLevel = IndentLevel + 1;
                    _nodeList.Add((node, node.IndentLevel));
                }

                node.PrintStructure = PrintStructure;
                node.Parse();
                return node;
            }

            return null;
        }

        protected List<AstNode> DecodeChildren(Reader reader)
        {
            long length = Decoders.DLength(reader);
            List<AstNode> result = new List<AstNode>();

            for (int i = 0; i < length; i++) //&& reader.Error == ParseError.None
            {
                AstNode child = DecodeNode(reader);
                if (child != null)
                {
                    child.PrintStructure = PrintStructure;
                    result.Add(child);
                }
            }

            return result;
        }

        protected LineInfo DecodeLineInfo(Reader reader)
        {
            //AstNode node = DecodeNode(reader);
            //node?.PrintStructure = PrintStructure;

            LineInfo result = new LineInfo
            {
                LineNumber = Decoders.DLength(reader),
                Child = DecodeNode(reader)
            };
            result.Child?.PrintStructure = PrintStructure;

            long labelCount = Decoders.DLength(reader);
            result.Labels = new List<string>();
            for (int i = 0; i < labelCount; i++)
            {
                string label = Decoders.DSid(reader);
                result.Labels.Add(label ?? "");
            }
            return result;
        }

        private static string PrependHeader(string code, JsxbinVersion version, bool unblind)
        {
            string versionStr = version switch
            {
                JsxbinVersion.v10 => "1.0",
                JsxbinVersion.v20 => "2.0",
                JsxbinVersion.v21 => "2.1",
                _ => "VERSION UNKNOWN"
            };

            string header = "/*\n"
                          + "* Decompiled with JsxBinParse\n"
                          + $"* Time: {DateTime.Now.ToString("yyyy-MM-dd HH:MM:ss")}\n"
                          + "* Version: 1.0.0.0\n"
                          + "* JSXBIN " + versionStr + "\n";

            if (unblind)
            {
                header += "* Jsxblind Deobfuscation Enabled (EXPERIMENTAL)\n";
            }

            header += "*/\n\n";

            return header + code;
        }

        private static List<Model.TreeStructure> BuildTreeFromLevels()
        {
            var result = new List<TreeStructure>();
            var lastNodeAtLevel = new Dictionary<int, TreeStructure>();// 每层最后一个节点

            foreach (var (node, level) in _nodeList)
            {
                var wrapper = new TreeStructure
                {
                    NodeName = node.Type.ToString(),
                    NodeLevel = node.IndentLevel,
                    ParentLevel = node.IndentLevel - 1
                };

                if (level == 0)
                {
                    result.Add(wrapper);
                }
                else
                {
                    // 查找父节点：上一层的最后一个节点
                    if (lastNodeAtLevel.TryGetValue(level - 1, out var parent))
                    {
                        parent.Children.Add(wrapper);
                    }
                    else
                    {
                        // 如果找不到父节点，可能是数据异常，作为根节点处理（容错）
                        result.Add(wrapper);
                    }
                }

                // 更新当前层的最新节点
                lastNodeAtLevel[level] = wrapper;
            }

            return result;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace JsxBinParser
{
    public sealed partial class TreeForm : Form
    {
        private readonly Action<string, List<Parser.Model.TreeStructure>> updatetree;

        public TreeForm(ref Action<string, List<Parser.Model.TreeStructure>> action)
        {
            InitializeComponent();

            action += UpdateTreeStructure;
            updatetree = action;
        }

        private void UpdateTreeStructure(string filename, List<Parser.Model.TreeStructure> nodes)
        {
            this.Text = filename;

            var lookup = nodes.ToLookup(c => c.ParentLevel);
            foreach (var item in nodes)
            {
                if (lookup.Contains(item.NodeLevel))
                {
                    item.Children.AddRange(lookup[item.NodeLevel]);
                }
            }

            var rootCategories = lookup[0].ToList();

            // 2. 清空并开始构建树
            treeView1.Nodes.Clear();
            foreach (var root in rootCategories)
            {
                TreeNode rootNode = new TreeNode(root.NodeName)
                {
                    Tag = root
                };
                treeView1.Nodes.Add(rootNode);
                AddChildNodes(rootNode, root.Children);
            }
        }

        private void AddChildNodes(TreeNode parentNode, List<Parser.Model.TreeStructure> children)
        {
            foreach (var child in children)
            {
                TreeNode childNode = new TreeNode(child.NodeName)
                {
                    Tag = child
                };
                parentNode.Nodes.Add(childNode);

                if (child.Children.Any())
                {
                    AddChildNodes(childNode, child.Children);
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;

namespace Parser.Model
{
    [Serializable]
    public sealed class TreeStructure
    {
        public TreeStructure()
        {
            Children = new List<TreeStructure>();
        }

        public int ParentLevel
        {
            get;
            set;
        }

        public int NodeLevel
        {
            get;
            set;
        }

        public string NodeName
        {
            get;
            set;
        }

        public List<TreeStructure> Children
        {
            get;
            set;
        }
    }
}

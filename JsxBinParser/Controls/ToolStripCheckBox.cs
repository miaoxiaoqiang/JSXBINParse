using System;
using System.Drawing;
using System.Windows.Forms;

namespace JsxBinParser
{
    public sealed class ToolStripCheckBox : ToolStripControlHost
    {
        public ToolStripCheckBox() : base(new CheckBox())
        {
            CheckBoxControl.BackColor = Color.Transparent;
            // 可选择使用系统样式（视觉上更统一）
            CheckBoxControl.FlatStyle = FlatStyle.Standard;
        }

        public CheckBox CheckBoxControl => Control as CheckBox;
    }
}

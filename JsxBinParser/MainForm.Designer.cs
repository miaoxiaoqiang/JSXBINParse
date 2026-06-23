namespace JsxBinParser
{
    partial class MainForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.TSB_AddFile = new System.Windows.Forms.ToolStripButton();
            this.TSB_AddFolder = new System.Windows.Forms.ToolStripButton();
            this.TSB_OpenOutFolder = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.TSB_Convert = new System.Windows.Forms.ToolStripButton();
            this.TSB_Stop = new System.Windows.Forms.ToolStripButton();
            this.TSB_ClearMsg = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.TSB_About = new System.Windows.Forms.ToolStripButton();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.Col_Status = new System.Windows.Forms.DataGridViewImageColumn();
            this.Col_Check = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Col_Treestrcture = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Col_File = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToResizeRows = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Col_Status,
            this.Col_Check,
            this.Col_Treestrcture,
            this.Col_File});
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(4, 4);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersWidth = 62;
            this.dataGridView1.RowTemplate.Height = 30;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(710, 358);
            this.dataGridView1.TabIndex = 0;
            // 
            // toolStrip1
            // 
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TSB_AddFile,
            this.TSB_AddFolder,
            this.TSB_OpenOutFolder,
            this.toolStripSeparator1,
            this.TSB_Convert,
            this.TSB_Stop,
            this.TSB_ClearMsg,
            this.toolStripSeparator2,
            this.TSB_About});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Padding = new System.Windows.Forms.Padding(5, 0, 2, 0);
            this.toolStrip1.Size = new System.Drawing.Size(718, 33);
            this.toolStrip1.TabIndex = 5;
            // 
            // TSB_AddFile
            // 
            this.TSB_AddFile.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.TSB_AddFile.Image = ((System.Drawing.Image)(resources.GetObject("TSB_AddFile.Image")));
            this.TSB_AddFile.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.TSB_AddFile.Name = "TSB_AddFile";
            this.TSB_AddFile.Size = new System.Drawing.Size(34, 28);
            this.TSB_AddFile.ToolTipText = "添加文件";
            this.TSB_AddFile.Click += new System.EventHandler(this.TSB_AddFile_Click);
            // 
            // TSB_AddFolder
            // 
            this.TSB_AddFolder.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.TSB_AddFolder.Image = ((System.Drawing.Image)(resources.GetObject("TSB_AddFolder.Image")));
            this.TSB_AddFolder.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.TSB_AddFolder.Name = "TSB_AddFolder";
            this.TSB_AddFolder.Size = new System.Drawing.Size(34, 28);
            this.TSB_AddFolder.ToolTipText = "选择文件夹";
            this.TSB_AddFolder.Click += new System.EventHandler(this.TSB_AddFolder_Click);
            // 
            // TSB_OpenOutFolder
            // 
            this.TSB_OpenOutFolder.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.TSB_OpenOutFolder.Image = ((System.Drawing.Image)(resources.GetObject("TSB_OpenOutFolder.Image")));
            this.TSB_OpenOutFolder.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.TSB_OpenOutFolder.Name = "TSB_OpenOutFolder";
            this.TSB_OpenOutFolder.Size = new System.Drawing.Size(34, 28);
            this.TSB_OpenOutFolder.ToolTipText = "打开输出目录";
            this.TSB_OpenOutFolder.Click += new System.EventHandler(this.TSB_OpenOutFolder_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 33);
            // 
            // TSB_Convert
            // 
            this.TSB_Convert.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.TSB_Convert.Enabled = false;
            this.TSB_Convert.Image = ((System.Drawing.Image)(resources.GetObject("TSB_Convert.Image")));
            this.TSB_Convert.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.TSB_Convert.Name = "TSB_Convert";
            this.TSB_Convert.Size = new System.Drawing.Size(34, 28);
            this.TSB_Convert.ToolTipText = "开始转换";
            this.TSB_Convert.Click += new System.EventHandler(this.TSB_Convert_Click);
            // 
            // TSB_Stop
            // 
            this.TSB_Stop.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.TSB_Stop.Image = ((System.Drawing.Image)(resources.GetObject("TSB_Stop.Image")));
            this.TSB_Stop.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.TSB_Stop.Name = "TSB_Stop";
            this.TSB_Stop.Size = new System.Drawing.Size(34, 28);
            this.TSB_Stop.ToolTipText = "停止转换";
            this.TSB_Stop.Click += new System.EventHandler(this.TSB_Stop_Click);
            // 
            // TSB_ClearMsg
            // 
            this.TSB_ClearMsg.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.TSB_ClearMsg.Image = ((System.Drawing.Image)(resources.GetObject("TSB_ClearMsg.Image")));
            this.TSB_ClearMsg.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.TSB_ClearMsg.Name = "TSB_ClearMsg";
            this.TSB_ClearMsg.Size = new System.Drawing.Size(34, 28);
            this.TSB_ClearMsg.ToolTipText = "清空输出内容";
            this.TSB_ClearMsg.Click += new System.EventHandler(this.TSB_ClearMsg_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 33);
            // 
            // TSB_About
            // 
            this.TSB_About.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.TSB_About.Image = ((System.Drawing.Image)(resources.GetObject("TSB_About.Image")));
            this.TSB_About.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.TSB_About.Name = "TSB_About";
            this.TSB_About.Size = new System.Drawing.Size(34, 28);
            this.TSB_About.ToolTipText = "关于本工具";
            this.TSB_About.Click += new System.EventHandler(this.TSB_About_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(0, 33);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.dataGridView1);
            this.splitContainer1.Panel1.Padding = new System.Windows.Forms.Padding(4);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.richTextBox1);
            this.splitContainer1.Panel2.Padding = new System.Windows.Forms.Padding(4);
            this.splitContainer1.Size = new System.Drawing.Size(718, 736);
            this.splitContainer1.SplitterDistance = 366;
            this.splitContainer1.SplitterWidth = 1;
            this.splitContainer1.TabIndex = 8;
            // 
            // richTextBox1
            // 
            this.richTextBox1.BackColor = System.Drawing.SystemColors.WindowText;
            this.richTextBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBox1.ForeColor = System.Drawing.SystemColors.Window;
            this.richTextBox1.Location = new System.Drawing.Point(4, 4);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.ReadOnly = true;
            this.richTextBox1.ShortcutsEnabled = false;
            this.richTextBox1.Size = new System.Drawing.Size(710, 361);
            this.richTextBox1.TabIndex = 0;
            this.richTextBox1.Text = "";
            // 
            // Col_Status
            // 
            this.Col_Status.Frozen = true;
            this.Col_Status.HeaderText = "状态";
            this.Col_Status.MinimumWidth = 8;
            this.Col_Status.Name = "Col_Status";
            this.Col_Status.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Col_Status.Width = 50;
            // 
            // Col_Check
            // 
            this.Col_Check.Frozen = true;
            this.Col_Check.HeaderText = "选择";
            this.Col_Check.MinimumWidth = 8;
            this.Col_Check.Name = "Col_Check";
            this.Col_Check.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Col_Check.Width = 110;
            // 
            // Col_Treestrcture
            // 
            this.Col_Treestrcture.Frozen = true;
            this.Col_Treestrcture.HeaderText = "树结构";
            this.Col_Treestrcture.MinimumWidth = 8;
            this.Col_Treestrcture.Name = "Col_Treestrcture";
            this.Col_Treestrcture.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Col_Treestrcture.Width = 110;
            // 
            // Col_File
            // 
            this.Col_File.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Col_File.HeaderText = "文件名";
            this.Col_File.MinimumWidth = 8;
            this.Col_File.Name = "Col_File";
            this.Col_File.ReadOnly = true;
            this.Col_File.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(718, 769);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.toolStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Adobe二进制脚本转换";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton TSB_About;
        private System.Windows.Forms.ToolStripButton TSB_AddFile;
        private System.Windows.Forms.ToolStripButton TSB_AddFolder;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton TSB_Convert;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ToolStripButton TSB_Stop;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.ToolStripButton TSB_ClearMsg;
        private System.Windows.Forms.ToolStripButton TSB_OpenOutFolder;
        private System.Windows.Forms.DataGridViewImageColumn Col_Status;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Col_Check;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Col_Treestrcture;
        private System.Windows.Forms.DataGridViewTextBoxColumn Col_File;
    }
}


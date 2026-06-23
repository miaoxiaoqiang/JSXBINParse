using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using JsxBinParser.Model;
using Parser.Nodes;

namespace JsxBinParser
{
    public sealed partial class MainForm : Form
    {
        private readonly OpenFileDialog ofd;
        private readonly FolderBrowserDialog fbd;
        private readonly BindingList<JSXBINFile> files;
        private readonly ContextMenuStrip contextMenuStrip;
        private readonly TreeForm treeForm;
        //private readonly ToolStripCheckBox toolStripCheckBox;

        private bool isDragging = false;
        private int dragRowIndex = -1;
        private int insertRowIndex = -1;
        private bool dragStarted = false;
        private Point dragStartPoint;
        private CheckBoxState converterheaderCheckBoxState = CheckBoxState.UncheckedNormal;
        private CheckBoxState treeheaderCheckBoxState = CheckBoxState.UncheckedNormal;
        private int contextMenuRowIndex = -1;
        private bool iscompiling = false;
        private CancellationTokenSource _cts;
        private event Action<string, List<Parser.Model.TreeStructure>> UpdateTreeStructure;

        public MainForm()
        {
            InitializeComponent();

            //toolStripCheckBox = new ToolStripCheckBox
            //{
            //    Enabled = true
            //};
            //toolStripCheckBox.CheckBoxControl.Text = "打印树结构";
            //toolStripCheckBox.CheckBoxControl.AutoCheck = false;
            //toolStripCheckBox.CheckBoxControl.Click += (s, e) =>
            //{
            //    if(iscompiling)
            //    {
            //        return;
            //    }
            //    var cb = s as CheckBox;
            //    cb.Checked = !cb.Checked;
            //};
            //toolStrip1.Items.Insert(0, toolStripCheckBox);

            contextMenuStrip = new ContextMenuStrip();

            ToolStripMenuItem showtreeItem = new ToolStripMenuItem("查看树结构");
            showtreeItem.Click += MenuItem_Click;
            showtreeItem.Image = Properties.Resources.Tree;
            contextMenuStrip.Items.Add(showtreeItem);

            ToolStripSeparator separator1 = new ToolStripSeparator();
            contextMenuStrip.Items.Add(separator1);

            ToolStripMenuItem deleteItem = new ToolStripMenuItem("删除记录");
            deleteItem.Click += MenuItem_Click;
            deleteItem.Image = Properties.Resources.Remove;
            contextMenuStrip.Items.Add(deleteItem);

            ToolStripMenuItem cleardataItem = new ToolStripMenuItem("清空所有记录");
            cleardataItem.Click += MenuItem_Click;
            cleardataItem.Image = Properties.Resources.ClearData;
            contextMenuStrip.Items.Add(cleardataItem);

            ToolStripSeparator separator = new ToolStripSeparator();
            contextMenuStrip.Items.Add(separator);

            ToolStripMenuItem cancelselectitem = new ToolStripMenuItem("取消选择");
            cancelselectitem.Click += MenuItem_Click;
            cancelselectitem.Image = Properties.Resources.CancelSelected;
            contextMenuStrip.Items.Add(cancelselectitem);

            ToolStripMenuItem clearselectitem = new ToolStripMenuItem("清除所有选择");
            clearselectitem.Click += MenuItem_Click;
            clearselectitem.Image = Properties.Resources.ClearAllSelect;
            contextMenuStrip.Items.Add(clearselectitem);

            dataGridView1.ContextMenuStrip = contextMenuStrip;
            contextMenuStrip.Opening += ContextMenuStrip1_Opening;

            ContextMenuStrip contextMenu = new ContextMenuStrip();
            ToolStripMenuItem copyItem = new ToolStripMenuItem("复制")
            {
                Image = Properties.Resources.Copy
            };
            copyItem.Click += (sender, e) => richTextBox1.Copy();
            contextMenu.Items.Add(copyItem);

            contextMenu.Opening += (sender, e) =>
            {
                if(richTextBox1.SelectionLength <= 0)
                {
                    e.Cancel = true;
                    return;
                }
                e.Cancel = false;
            };
            richTextBox1.ContextMenuStrip = contextMenu;

            files = new BindingList<JSXBINFile>();
            files.ListChanged += (s, e) =>
            {
                if (e.ListChangedType == ListChangedType.ItemAdded ||
                    e.ListChangedType == ListChangedType.ItemDeleted ||
                    e.ListChangedType == ListChangedType.Reset)
                {
                    AdjustRowHeaderWidth();
                }
                //else if(e.ListChangedType == ListChangedType.ItemChanged)
                //{
                //    // 获取变化的行索引
                //    int rowIndex = e.NewIndex;
                //    // 跳过无效索引
                //    if (rowIndex < 0 || rowIndex >= dataGridView1.Rows.Count)
                //    {
                //        return;
                //    }

                //    DataGridViewRow row = dataGridView1.Rows[rowIndex];

                //    if (row.IsNewRow)
                //    {
                //        return;
                //    }

                //    if (row.DataBoundItem is JSXBINFile entity && !row.Cells[1].ReadOnly)
                //    {
                //        row.Cells[1].ReadOnly = (entity.Status == JSXBINFile.ParseStatus.Success && entity.IsChecked);
                //    }
                //}
            };

            Icon = Properties.Resources.AppIcon;

            ofd = new OpenFileDialog()
            {
                Multiselect = true,
                InitialDirectory = AppDomain.CurrentDomain.BaseDirectory,
                Filter = "Adobe二进制脚本(*.jsxbin)|*.jsxbin",
                Title = "脚本文件",
                RestoreDirectory = false,
            };
            fbd = new FolderBrowserDialog()
            {
                ShowNewFolderButton = false,
                SelectedPath = AppDomain.CurrentDomain.BaseDirectory
            };

            TSB_About.Image = Properties.Resources.About;
            TSB_AddFile.Image = Properties.Resources.AddFile;
            TSB_AddFolder.Image = Properties.Resources.AddFolder;
            TSB_OpenOutFolder.Image = Properties.Resources.AddFolder;
            TSB_Convert.Image = Properties.Resources.Convertert;
            TSB_ClearMsg.Image = Properties.Resources.ClearMsg;
            TSB_Stop.Image = Properties.Resources.Stop;
            TSB_Stop.Enabled = false;

            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.Columns[0].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[1].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[2].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[3].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[0].DataPropertyName = "Image";
            dataGridView1.Columns[0].ToolTipText = "反编译状态";
            dataGridView1.Columns[1].DataPropertyName = "IsChecked";
            dataGridView1.Columns[1].ToolTipText = "通过选择是否对文件进行反编译";
            dataGridView1.Columns[2].DataPropertyName = "PrintTreeStructure";
            dataGridView1.Columns[2].ToolTipText = "通过选择反编译时是否输出树结构";
            dataGridView1.Columns[3].DataPropertyName = "FileName";
            dataGridView1.DataSource = files;

            dataGridView1.MouseDown += DataGridView1_MouseDown;
            dataGridView1.MouseMove += DataGridView1_MouseMove;
            dataGridView1.MouseUp += DataGridView1_MouseUp;
            dataGridView1.Paint += DataGridView1_Paint;
            dataGridView1.CellPainting += DataGridView1_CellPainting;
            dataGridView1.CellClick += DataGridView1_CellClick;
            dataGridView1.CellContentClick += DataGridView1_CellContentClick;
            dataGridView1.RowPostPaint += DataGridView1_RowPostPaint;

            treeForm = new TreeForm(ref UpdateTreeStructure);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            _cts?.Dispose();
            base.OnClosing(e);
        }

        private void TSB_About_Click(object sender, EventArgs e)
        {
            AboutForm aboutForm = new AboutForm();
            aboutForm.ShowDialog();
        }

        private void TSB_AddFile_Click(object sender, EventArgs e)
        {
            if(iscompiling)
            {
                return;
            }

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string[] _selectfiles = ofd.FileNames;
                string[] result = _selectfiles.Where(n => !files.Any(p => string.Equals(p.FilePath, n, StringComparison.OrdinalIgnoreCase))).ToArray();
                if (result.Length > 0)
                {
                    ofd.InitialDirectory = Path.GetDirectoryName(result[0]);
                    foreach (string file in result)
                    {
                        files.Add(new JSXBINFile() { FileName = Path.GetFileName(file), IsChecked = false, PrintTreeStructure = false, FilePath = file });
                    }
                }
            }
        }

        private void TSB_AddFolder_Click(object sender, EventArgs e)
        {
            if (iscompiling)
            {
                return;
            }

            if (fbd.ShowDialog() == DialogResult.OK)
            {
                string[] _files = Directory.GetFiles(fbd.SelectedPath, "*.jsxbin", SearchOption.AllDirectories);
                string[] result = _files.Where(n => !files.Any(p => string.Equals(p.FilePath, n, StringComparison.OrdinalIgnoreCase))).ToArray();
                foreach (string file in result)
                {
                    files.Add(new JSXBINFile() { FileName = Path.GetFileName(file), IsChecked = false, PrintTreeStructure = false, FilePath = file });
                }
            }
        }

        private void TSB_OpenOutFolder_Click(object sender, EventArgs e)
        {
            Directory.CreateDirectory("Output");
            ExplorerHelper.OpenOrFocusFolder(AppDomain.CurrentDomain.BaseDirectory + "Output");
        }

        private async void TSB_Convert_Click(object sender, EventArgs e)
        {
            Directory.CreateDirectory("Output");
            Directory.CreateDirectory("ErrorLog");

            _cts = new CancellationTokenSource();
            iscompiling = true;
            TSB_Convert.Enabled = false;
            TSB_Stop.Enabled = true;

            richTextBox1.SuspendLayout();
            try
            {
                await Decompile(_cts.Token);
            }
            catch (OperationCanceledException)
            {
                richTextBox1.AppendText($"已取消反编译\r\n");
                ChangeWordColor(richTextBox1, 7, Color.Yellow);
            }
            catch (Exception ex)
            {
                richTextBox1.AppendText($"{ex.Message}\r\n");
                ChangeWordColor(richTextBox1, 1 + ex.Message.Length, Color.Red);
            }
            finally
            {
                _cts?.Dispose();
                TSB_Convert.Enabled = true;
                TSB_Stop.Enabled = false;
                iscompiling = false;
                richTextBox1.ResumeLayout();
            }
        }

        private async Task Decompile(CancellationToken token)
        {
            await Task.Run(() =>
            {
                foreach (JSXBINFile file in files)
                {
                    token.ThrowIfCancellationRequested();
                    int targetRowIndex = files.IndexOf(file);

                    if (file.IsChecked && file.Status != JSXBINFile.ParseStatus.Success)
                    {
                        try
                        {
                            richTextBox1.Invoke(() =>
                            {
                                if (targetRowIndex >= 0 && targetRowIndex < dataGridView1.Rows.Count)
                                {
                                    dataGridView1.ClearSelection();
                                    dataGridView1.Rows[targetRowIndex].Selected = true;
                                    dataGridView1.FirstDisplayedScrollingRowIndex = targetRowIndex;
                                }

                                richTextBox1.AppendText($"正对 [{file.FileName}] 反编译中...\r\n");
                                file.Status = JSXBINFile.ParseStatus.Solving;
                            });

                            Tuple<string, List<Parser.Model.TreeStructure>> result = AstNode.Decode(File.ReadAllText(file.FilePath, System.Text.Encoding.ASCII), false, file.PrintTreeStructure);
                            File.WriteAllText($"Output\\{Path.GetFileNameWithoutExtension(file.FilePath)}.jsx", result.Item1);

                            richTextBox1.Invoke(() =>
                            {
                                file.Status = JSXBINFile.ParseStatus.Success;

                                if (file.PrintTreeStructure)
                                {
                                    richTextBox1.AppendText("树结构解析完成：\r\n");
                                    file.Structures = result.Item2;
                                }

                                richTextBox1.AppendText("反编译完成\r\n\r\n");
                                ChangeWordColor(richTextBox1, 7, Color.Green);
                            });
                        }
                        catch (Exception ex)
                        {
                            richTextBox1.Invoke(() =>
                            {
                                file.Status = JSXBINFile.ParseStatus.Error;
                                richTextBox1.AppendText($"反编译失败\r\n\r\n");
                                ChangeWordColor(richTextBox1, 7, Color.Red);
                            });
                            File.WriteAllText($"ErrorLog\\errordecompile_{Path.GetFileNameWithoutExtension(file.FilePath)}.log", ex.Message);
                        }
                    }
                }
            }, token);
        }

        private void TSB_Stop_Click(object sender, EventArgs e)
        {
            _cts?.Cancel();
            TSB_Convert.Enabled = true;
            TSB_Stop.Enabled = false;
            iscompiling = false;
        }

        private void TSB_ClearMsg_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
        }

        private void MenuItem_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                return;
            }

            if (sender is ToolStripMenuItem clickedItem && clickedItem.Owner is ContextMenuStrip owner)
            {
                int index = owner.Items.IndexOf(clickedItem);
                if(index == 0)
                {
                    if (contextMenuRowIndex >= 0 && contextMenuRowIndex < dataGridView1.Rows.Count)
                    {
                        if(dataGridView1.Rows[contextMenuRowIndex].DataBoundItem is JSXBINFile file)
                        {
                            if (file.Status == JSXBINFile.ParseStatus.Success && file.PrintTreeStructure)
                            {
                                UpdateTreeStructure?.Invoke($"树结构：{Path.GetFileName(file.FilePath)}", file.Structures);
                                treeForm.ShowDialog();
                            }
                            else
                            {
                                MessageBox.Show("尚未解析树或树解析错误", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                    }
                }
                else if(index == 2)
                {
                    List<JSXBINFile> itemsToRemove = new List<JSXBINFile>();
                    foreach (DataGridViewRow row in dataGridView1.SelectedRows)
                    {
                        if (row.DataBoundItem is JSXBINFile item)
                        {
                            itemsToRemove.Add(item);
                        }
                    }

                    foreach (var item in itemsToRemove)
                    {
                        files.Remove(item);
                    }

                    UpdateHeaderCheckBoxState();
                    AdjustRowHeaderWidth();
                }
                else if (index == 3)
                {
                    files.Clear();

                    UpdateHeaderCheckBoxState();
                    AdjustRowHeaderWidth();
                }
                else if (index == 5)
                {
                    if (contextMenuRowIndex >= 0 && contextMenuRowIndex < dataGridView1.Rows.Count)
                    {
                        dataGridView1.Rows[contextMenuRowIndex].Selected = false;
                    }
                }
                else if (index == 6)
                {
                    dataGridView1.ClearSelection();
                }
            }
        }

        private void ContextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            if (iscompiling)
            {
                e.Cancel = true;
                return;
            }

            if (dataGridView1.Rows.Count == 0)
            {
                e.Cancel = true;
                return;
            }

            Point cursorPos = dataGridView1.PointToClient(Control.MousePosition);
            DataGridView.HitTestInfo hit = dataGridView1.HitTest(cursorPos.X, cursorPos.Y);

            if (hit.Type != DataGridViewHitTestType.Cell || hit.RowIndex < 0 || hit.ColumnIndex < 0)
            {
                e.Cancel = true;
                return;
            }

            DataGridViewRow clickedRow = dataGridView1.Rows[hit.RowIndex];
            if (!clickedRow.Selected)
            {
                e.Cancel = true;
                return;
            }

            contextMenuRowIndex = hit.RowIndex;

            ContextMenuStrip context = sender as ContextMenuStrip;
            if(dataGridView1.SelectedRows.Count > 1)
            {
                context.Items[0].Enabled = false;
            }
            else if (dataGridView1.SelectedRows.Count == 1)
            {
                context.Items[0].Enabled = true;
            }

            e.Cancel = false;
        }

        private void DataGridView1_MouseDown(object sender, MouseEventArgs e)
        {
            if (iscompiling)
            {
                return;
            }

            if (e.Button == MouseButtons.Left)
            {
                DataGridView.HitTestInfo hit = dataGridView1.HitTest(e.X, e.Y);
                if (hit.Type == DataGridViewHitTestType.RowHeader && hit.RowIndex >= 0)
                {
                    dragRowIndex = hit.RowIndex;
                    dragStartPoint = e.Location;
                    dragStarted = false;
                }
            }
        }

        private void DataGridView1_MouseMove(object sender, MouseEventArgs e)
        {
            if (iscompiling)
            {
                return;
            }

            if (e.Button == MouseButtons.Left && dragRowIndex >= 0)
            {
                // 检测是否达到拖拽启动阈值
                if (!dragStarted)
                {
                    if (Math.Abs(e.X - dragStartPoint.X) > SystemInformation.DragSize.Width ||
                        Math.Abs(e.Y - dragStartPoint.Y) > SystemInformation.DragSize.Height)
                    {
                        dragStarted = true;
                        isDragging = true;
                        Cursor = Cursors.SizeAll;
                    }
                    else
                    {
                        return;
                    }
                }

                // 计算目标插入位置
                DataGridView.HitTestInfo hit = dataGridView1.HitTest(e.X, e.Y);
                int targetRow = -1;
                if (hit.Type == DataGridViewHitTestType.Cell || hit.Type == DataGridViewHitTestType.RowHeader)
                {
                    targetRow = hit.RowIndex;
                }
                else if (hit.Type == DataGridViewHitTestType.None)
                {
                    // 鼠标在网格底部以下，插入到末尾
                    if (dataGridView1.Rows.Count > 0)
                    {
                        int lastRowIndex = dataGridView1.Rows.Count - 1;
                        Rectangle lastRowRect = dataGridView1.GetRowDisplayRectangle(lastRowIndex, true);
                        if (e.Y > lastRowRect.Bottom)
                        {
                            targetRow = dataGridView1.Rows.Count; // 插入到末尾
                        }
                    }
                }

                // 更新插入位置，并重绘以显示插入线
                if (targetRow != insertRowIndex)
                {
                    insertRowIndex = targetRow;
                    dataGridView1.Invalidate(); // 触发 Paint 事件
                }
            }
        }

        private void DataGridView1_MouseUp(object sender, MouseEventArgs e)
        {
            if (iscompiling)
            {
                return;
            }

            if (e.Button == MouseButtons.Left && isDragging)
            {
                // 执行重排序
                if (dragRowIndex >= 0 && insertRowIndex >= 0 && dragRowIndex != insertRowIndex)
                {
                    // 如果 insertRowIndex 等于行数，表示插入到末尾
                    int targetIndex = insertRowIndex;
                    if (targetIndex >= files.Count)
                    {
                        targetIndex = files.Count - 1;
                    }

                    ReorderRows(dragRowIndex, targetIndex);
                }

                isDragging = false;
                dragRowIndex = -1;
                insertRowIndex = -1;
                Cursor = Cursors.Default;
                dataGridView1.Invalidate();
            }
        }

        private void DataGridView1_Paint(object sender, PaintEventArgs e)
        {
            if (iscompiling)
            {
                return;
            }

            if (isDragging && insertRowIndex >= 0)
            {
                int y;
                if (insertRowIndex < dataGridView1.Rows.Count)
                {
                    // 获取目标行的顶部位置
                    Rectangle rowRect = dataGridView1.GetRowDisplayRectangle(insertRowIndex, true);
                    y = rowRect.Top;
                }
                else
                {
                    // 插入到末尾：获取最后一行底部
                    int lastRow = dataGridView1.Rows.Count - 1;
                    Rectangle lastRowRect = dataGridView1.GetRowDisplayRectangle(lastRow, true);
                    y = lastRowRect.Bottom;
                }

                // 绘制红色插入线（跨整个表格宽度）
                using (Pen pen = new Pen(Color.Red, 3))
                {
                    e.Graphics.DrawLine(pen, dataGridView1.Left, y, dataGridView1.Right, y);
                }
            }
        }

        private void DataGridView1_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            // 行号从 1 开始
            string rowNumber = (e.RowIndex + 1).ToString();

            // 在行头区域内绘制，右对齐并垂直居中
            Rectangle headerRect = new Rectangle(
                e.RowBounds.Left,
                e.RowBounds.Top,
                dataGridView1.RowHeadersWidth,
                e.RowBounds.Height
            );

            TextFormatFlags flags = TextFormatFlags.VerticalCenter | TextFormatFlags.Right | TextFormatFlags.SingleLine;

            // 使用行头默认字体和颜色
            using (Brush brush = new SolidBrush(dataGridView1.RowHeadersDefaultCellStyle.ForeColor))
            {
                TextRenderer.DrawText(
                    e.Graphics,
                    rowNumber,
                    dataGridView1.RowHeadersDefaultCellStyle.Font,
                    headerRect,
                    dataGridView1.RowHeadersDefaultCellStyle.ForeColor,
                    flags
                );
            }
        }

        private void DataGridView1_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            void DrawHeaderCheckBox(int ColIndex)
            {
                // 1. 绘制背景和边框
                e.Paint(e.CellBounds, DataGridViewPaintParts.Background | DataGridViewPaintParts.Border);
                // 2. 获取复选框大小（系统默认大小）
                Size glyphSize = CheckBoxRenderer.GetGlyphSize(e.Graphics, CheckBoxState.UncheckedNormal);

                string text = ColIndex == 1 ? "反编译" : "树结构";
                Font font = e.CellStyle.Font ?? dataGridView1.Font;
                Size textSize = TextRenderer.MeasureText(e.Graphics, text, font);
                // 间距
                int spacing = 4;
                // 总宽度 = 复选框宽度 + 间距 + 文字宽度
                int totalWidth = glyphSize.Width + spacing + textSize.Width;
                // 垂直居中
                int centerY = e.CellBounds.Y + (e.CellBounds.Height - Math.Max(glyphSize.Height, textSize.Height)) / 2;
                // 水平起始X
                int startX = e.CellBounds.X + (e.CellBounds.Width - totalWidth) / 2;

                // 4. 绘制复选框
                Point glyphLocation = new Point(startX, centerY);
                CheckBoxRenderer.DrawCheckBox(e.Graphics, glyphLocation, ColIndex == 1 ? converterheaderCheckBoxState : treeheaderCheckBoxState);
                //ControlPaint.DrawCheckBox(e.Graphics, checkBoxRect, isHeaderCheckBoxChecked ? ButtonState.Checked : ButtonState.Normal);

                // 5. 绘制文字（在复选框右侧）
                Point textLocation = new Point(startX + glyphSize.Width + spacing, centerY);
                TextRenderer.DrawText(e.Graphics, text, font, textLocation, e.CellStyle.ForeColor);

                e.Handled = true;
            }

            if (e.RowIndex == -1 && (e.ColumnIndex == dataGridView1.Columns[1].Index || e.ColumnIndex == dataGridView1.Columns[2].Index))
            {
                DrawHeaderCheckBox(e.ColumnIndex);
            }

            // 处理行头列头（左上角）显示 "序号"
            if (e.RowIndex == -1 && e.ColumnIndex == -1)
            {
                // 1. 绘制背景和边框（保持与默认一致）
                e.Paint(e.CellBounds, DataGridViewPaintParts.Background | DataGridViewPaintParts.Border);

                // 2. 绘制文字
                string text = "序号";
                Font font = dataGridView1.ColumnHeadersDefaultCellStyle.Font ?? dataGridView1.Font;
                Color foreColor = dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor;

                // 水平居中、垂直居中
                TextFormatFlags flags = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine;

                TextRenderer.DrawText(e.Graphics, text, font, e.CellBounds, foreColor, flags);

                // 3. 阻止默认绘制
                e.Handled = true;
            }
        }

        private void DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (iscompiling)
            {
                return;
            }

            if (e.RowIndex == -1 && (e.ColumnIndex == dataGridView1.Columns[1].Index || e.ColumnIndex == dataGridView1.Columns[2].Index))
            {
                // 如果当前是选中或混合状态，则全不选；否则全选
                bool newState;

                if(e.ColumnIndex == 1)
                {
                    newState = !(converterheaderCheckBoxState == CheckBoxState.CheckedNormal || converterheaderCheckBoxState == CheckBoxState.MixedNormal);
                }
                else
                {
                    newState = !(treeheaderCheckBoxState == CheckBoxState.CheckedNormal || converterheaderCheckBoxState == CheckBoxState.MixedNormal);
                }

                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (!row.IsNewRow)
                    {
                        if (row.DataBoundItem is JSXBINFile item)
                        {
                            if(e.ColumnIndex == 1)
                            {
                                item.IsChecked = newState;
                            }
                            else
                            {
                                item.PrintTreeStructure = newState;
                            }
                        }
                    }
                }

                UpdateHeaderCheckBoxState();
            }
        }

        private void DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if(iscompiling)
            {
                return;
            }

            if(e.RowIndex >= 0)
            {
                if (e.ColumnIndex == dataGridView1.Columns[1].Index)
                {
                    DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                    if (row.DataBoundItem is JSXBINFile item)
                    {
                        item.IsChecked = !item.IsChecked;
                        UpdateHeaderCheckBoxState();
                    }
                }
                else if (e.ColumnIndex == dataGridView1.Columns[2].Index)
                {
                    DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                    if (row.DataBoundItem is JSXBINFile item)
                    {
                        item.PrintTreeStructure = !item.PrintTreeStructure;
                        UpdateHeaderCheckBoxState();
                    }
                }
            }
        }

        private void UpdateHeaderCheckBoxState()
        {
            int checkedCount = 0;
            int totalCount = 0;

            int checkedCount1 = 0;
            int totalCount1 = 0;

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (!row.IsNewRow)
                {
                    totalCount++;
                    totalCount1++;

                    if (row.DataBoundItem is JSXBINFile item)
                    {
                        if(item.IsChecked)
                        {
                            checkedCount++;
                        }

                        if (item.PrintTreeStructure)
                        {
                            checkedCount1++;
                        }
                    }
                }
            }

            if (totalCount == 0)
            {
                TSB_Convert.Enabled = false;
                converterheaderCheckBoxState = CheckBoxState.UncheckedNormal;
            }
            else if (checkedCount == totalCount)
            {
                TSB_Convert.Enabled = true;
                converterheaderCheckBoxState = CheckBoxState.CheckedNormal;
            }
            else if (checkedCount == 0)
            {
                TSB_Convert.Enabled = false;
                converterheaderCheckBoxState = CheckBoxState.UncheckedNormal;
            }
            else
            {
                TSB_Convert.Enabled = true;
                converterheaderCheckBoxState = CheckBoxState.MixedNormal;
            }

            if (totalCount1 == 0)
            {
                treeheaderCheckBoxState = CheckBoxState.UncheckedNormal;
            }
            else if (checkedCount1 == totalCount1)
            {
                treeheaderCheckBoxState = CheckBoxState.CheckedNormal;
            }
            else if (checkedCount1 == 0)
            {
                treeheaderCheckBoxState = CheckBoxState.UncheckedNormal;
            }
            else
            {
                treeheaderCheckBoxState = CheckBoxState.MixedNormal;
            }

            // 强制重绘列头
            dataGridView1.InvalidateColumn(dataGridView1.Columns[1].Index);
            dataGridView1.InvalidateColumn(dataGridView1.Columns[2].Index);
        }

        private void AdjustRowHeaderWidth()
        {
            int rowCount = files.Count();
            if (rowCount == 0)
            {
                return;
            }

            string maxNumber = rowCount.ToString();
            string headerText = "序号";
            Font font = dataGridView1.RowHeadersDefaultCellStyle.Font ?? dataGridView1.Font;
            // 取行号文本和列头文本两者宽度较大者
            Size size1 = TextRenderer.MeasureText(maxNumber, font);
            Size size2 = TextRenderer.MeasureText(headerText, font);
            int maxWidth = Math.Max(size1.Width, size2.Width);

            int padding = 10;
            int newWidth = maxWidth + padding;
            if (newWidth < 20)
            {
                newWidth = 20;
            }

            dataGridView1.RowHeadersWidth = newWidth;
        }

        private void ReorderRows(int fromIndex, int toIndex)
        {
            if (fromIndex == toIndex || files.Count == 0)
            {
                return;
            }

            if (toIndex < 0)
            {
                toIndex = 0;
            }

            if (toIndex >= files.Count)
            {
                toIndex = files.Count - 1;
            }

            var item = files[fromIndex];
            files.RemoveAt(fromIndex);
            // 如果目标位置在移除位置之后，索引要减一
            if (toIndex > fromIndex)
            {
                toIndex--;
            }

            files.Insert(toIndex, item);

            // 选中移动后的行
            dataGridView1.Rows[toIndex].Selected = true;
        }

        private void ChangeWordColor(RichTextBox rtb, int lastindex, Color color)
        {
            if(lastindex > 0)
            {
                rtb.Select(rtb.TextLength - lastindex, rtb.TextLength);
                rtb.SelectionColor = color;

                rtb.SelectionLength = 0;
                rtb.ScrollToCaret();
            }
        }
    }
}

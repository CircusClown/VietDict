using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.Threading;

namespace VietDict
{
    public partial class XtraForm1 : DevExpress.XtraEditors.XtraForm
    {
        process mainProc;
        TreeNode curNode;
        string selectCollection = "";
        string tempSelectCollection = "";
        bool isBookmarked;
        public XtraForm1()
        {

            InitializeComponent();
            mainProc = new process();
            List<string> result = mainProc.wordListing();
            treeView1.BeginUpdate();
            foreach (var entry in result)
            {
                treeView1.Nodes.Add(entry);
            }
            treeView1.EndUpdate();

            List<string> colList = mainProc.collectionListing();
            if (colList.Count > 0)
            {
                foreach (var entry in colList)
                {
                    listView2.Items.Add(new ListViewItem(new string[] { entry }, "hiclipart.com (4).png", System.Drawing.SystemColors.Window, System.Drawing.Color.Empty, null));
                }
            }
            this.KeyPreview = true;
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
        }

        private void Button5_Click(object sender, EventArgs e)
        {
            //retrieve Next History
            traverseHistory(1);
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            //retrieve Previous History
            traverseHistory(0);
        }

        private void traverseHistory(int dir)
        {
            button3.Enabled = false;
            button5.Enabled = false;
            string Query;
            if (dir == 1) Query = mainProc.retrieveNextHistory();
            else Query = mainProc.retrievePrevHistory();
            textBox1.Text = Query;
            List<string> result = mainProc.wordQuery(Query);
            treeView1.BeginUpdate();
            treeView1.Nodes.Clear();
            foreach (var entry in result)
            {
                treeView1.Nodes.Add(entry);
            }
            treeView1.EndUpdate();
            if (treeView1.Nodes.Count > 0)
            {
                curNode = treeView1.Nodes[0];
                reloadMeaningPanel();
            }
            button3.Enabled = true;
            button5.Enabled = true;
        }

        private void TextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void Label3_Click(object sender, EventArgs e)
        {

        }

        private void Button6_Click(object sender, EventArgs e)
        {
            //get next entry in result list
            traverseWordList(1);
        }

        private void Button7_Click(object sender, EventArgs e)
        {
            //get previous entry in result list
            traverseWordList(0);
        }

        private void traverseWordList(int dir)
        {
            if (dir == 1)
            {
                if (curNode.NextNode != null)
                    curNode = curNode.NextNode;
            }
            else
            {
                if (curNode.PrevNode != null)
                    curNode = curNode.PrevNode;
            }

            reloadMeaningPanel();
        }

        private void Button4_Click(object sender, EventArgs e)
        {
            //Bookmark button
            //If isBookmarked == true unbookmark
            //If isBookmarked == false bookmark
            bool res;
            if (curNode == null)
            {
                DevExpress.XtraBars.Docking2010.Views.WindowsUI.FlyoutAction action2 = new DevExpress.XtraBars.Docking2010.Views.WindowsUI.FlyoutAction() { Caption = "Thông báo", Description = "Xin hãy chọn từ trước khi bookmark" };
                Predicate<DialogResult> predicate2 = canCloseFunc;
                DevExpress.XtraBars.Docking2010.Views.WindowsUI.FlyoutCommand command1_2 = new DevExpress.XtraBars.Docking2010.Views.WindowsUI.FlyoutCommand() { Text = "OK", Result = System.Windows.Forms.DialogResult.OK };
                action2.Commands.Add(command1_2);
                DevExpress.XtraBars.Docking2010.Views.WindowsUI.FlyoutProperties properties2 = new DevExpress.XtraBars.Docking2010.Views.WindowsUI.FlyoutProperties();
                properties2.ButtonSize = new Size(100, 40);
                properties2.Style = DevExpress.XtraBars.Docking2010.Views.WindowsUI.FlyoutStyle.MessageBox;
                DevExpress.XtraBars.Docking2010.Customization.FlyoutDialog.Show(this, action2, properties2, predicate2);
                return;
            }
            if (isBookmarked)
            {
                res = mainProc.removeFromBookmark(curNode.Text);
                if (res)
                {
                    isBookmarked = false;
                    button4.Image = global::VietDict.Properties.Resources.untitled__13_;
                }
            }
            else
            {
                res = mainProc.addToBookmark(curNode.Text);
                {
                    isBookmarked = true;
                    button4.Image = global::VietDict.Properties.Resources.untitled__23_;
                }
            }
        }

        private void FormatText()
        {
            int selectionstart = richTextBox1.SelectionStart;
            int totallength = richTextBox1.Lines.Length;
            for (int i = selectionstart; i < totallength; i++)
            {
                richTextBox1.Select(richTextBox1.GetFirstCharIndexFromLine(i), richTextBox1.Lines[i].Length);

                if (richTextBox1.SelectedText.Contains("+") == true)
                {
                    richTextBox1.SelectionColor = Color.FromArgb(191, 201, 38);
                    richTextBox1.Select(richTextBox1.GetFirstCharIndexFromLine(i), 5);
                    richTextBox1.SelectionColor = Color.Aqua;
                }
                if (richTextBox1.SelectedText.Contains("*") == true)
                {
                    richTextBox1.SelectionColor = Color.FromArgb(191, 150, 100);
                    richTextBox1.Select(richTextBox1.GetFirstCharIndexFromLine(i), 5);
                    richTextBox1.SelectionColor = Color.Aqua;
                    richTextBox1.SelectedText = "\t\t =";
                }
                if ((richTextBox1.SelectedText.Contains("-") == true) && (richTextBox1.SelectedText.Contains("->") == false))
                {
                    richTextBox1.SelectionColor = Color.LightBlue;
                    richTextBox1.Select(richTextBox1.GetFirstCharIndexFromLine(i), 4);
                    richTextBox1.SelectionColor = Color.Aqua;
                }
                if (richTextBox1.SelectedText.Contains("->") == true)
                {
                    richTextBox1.SelectionColor = Color.Aqua;
                    richTextBox1.SelectionFont = new Font("Segoe UI", 16, FontStyle.Regular);
                    richTextBox1.Select(richTextBox1.GetFirstCharIndexFromLine(i), 2);
                    richTextBox1.SelectedText = "\u21D2";
                }

                if (richTextBox1.SelectedText.Contains("->") == true)
                {
                    richTextBox1.SelectionColor = Color.Yellow;
                    richTextBox1.SelectionFont = new Font("Segoe UI", 16, FontStyle.Regular);
                    richTextBox1.Select(richTextBox1.GetFirstCharIndexFromLine(i), 2);
                    richTextBox1.SelectedText = "\u21D2";
                }
            }

        }

        private void Button1_Click(object sender, EventArgs e)
        {
            onSearch();
        }

    

        private void reloadMeaningPanel()
        {
            string pronounce;
            string img_path;
            richTextBox1.Text = mainProc.outputWordBaseInfo(curNode.Text, out pronounce, out img_path);
            FormatText();
            if (img_path == "") pictureBox1.Visible = false;
            else pictureBox1.Visible = true;
            
            if (img_path.StartsWith("img_data/"))       //default image data
            {
                //EDIT THIS BEFORE FINAL BUILD WITH: "/resources/" + img_path
                pictureBox1.ImageLocation = AppDomain.CurrentDomain.GetData("SolutionDirectory") + "/Resources/" + img_path;
            }
            else pictureBox1.ImageLocation = img_path;
            richTextBox2.Text = mainProc.outputWordSpecialInfo(curNode.Text);
            label3.Text = curNode.Text;
            label4.Text = pronounce;
            isBookmarked = mainProc.isBookmarked(curNode.Text);

            //Bookmark checking
            if (isBookmarked) { button4.Image = global::VietDict.Properties.Resources.untitled__23_; }
            else button4.Image = global::VietDict.Properties.Resources.untitled__13_;
        }

        private void onSearch()
        {
            List<string> result;
            if (selectCollection != "") result = mainProc.wordCollectionQuery(textBox1.Text, selectCollection);
            else result = mainProc.wordQuery(textBox1.Text);
            treeView1.BeginUpdate();
            treeView1.Nodes.Clear();
            foreach (var entry in result)
            {
                treeView1.Nodes.Add(entry);
            }
            treeView1.EndUpdate();
            if (treeView1.Nodes.Count > 0)
            {
                curNode = treeView1.Nodes[0];
                reloadMeaningPanel();
            }
            mainProc.addToHistory(textBox1.Text);
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            mainProc.speakWord(label3.Text);
        }

        private void XtraForm1_Load(object sender, EventArgs e)
        {
            Thread.Sleep(2000);
        }

        private static bool canCloseFunc(DialogResult parameter)
        {
            return parameter != DialogResult.Cancel;
        }

        private void XtraForm1_FormClosing(object sender, FormClosingEventArgs e)
        {
            DevExpress.XtraBars.Docking2010.Views.WindowsUI.FlyoutAction action = new DevExpress.XtraBars.Docking2010.Views.WindowsUI.FlyoutAction() { Caption = "Xác nhận", Description = "Bạn muốn thoát khỏi ứng dụng?" };
            Predicate<DialogResult> predicate = canCloseFunc;
            DevExpress.XtraBars.Docking2010.Views.WindowsUI.FlyoutCommand command1 = new DevExpress.XtraBars.Docking2010.Views.WindowsUI.FlyoutCommand() { Text = "Có", Result = System.Windows.Forms.DialogResult.Yes };
            DevExpress.XtraBars.Docking2010.Views.WindowsUI.FlyoutCommand command2 = new DevExpress.XtraBars.Docking2010.Views.WindowsUI.FlyoutCommand() { Text = "Không", Result = System.Windows.Forms.DialogResult.No };
            action.Commands.Add(command1);
            action.Commands.Add(command2);
            DevExpress.XtraBars.Docking2010.Views.WindowsUI.FlyoutProperties properties = new DevExpress.XtraBars.Docking2010.Views.WindowsUI.FlyoutProperties();
            properties.ButtonSize = new Size(100, 40);
            properties.Style = DevExpress.XtraBars.Docking2010.Views.WindowsUI.FlyoutStyle.MessageBox;
            if (DevExpress.XtraBars.Docking2010.Customization.FlyoutDialog.Show(this, action, properties, predicate) == System.Windows.Forms.DialogResult.Yes) e.Cancel = false;
            else e.Cancel = true;
        }

        private void ListView3_Click(object sender, EventArgs e)
        {
            if (listView3.SelectedIndices.Count < 1) return;
            if (listView3.SelectedIndices[0] == 2)
            {
                //string s = "test";
                //DevExpress.XtraBars.Docking2010.Views.WindowsUI.FlyoutAction action = new DevExpress.XtraBars.Docking2010.Views.WindowsUI.FlyoutAction() { Caption = "Thông tin nhà phát triển", Description = s };
                //DevExpress.XtraBars.Docking2010.Views.WindowsUI.FlyoutCommand command1 = new DevExpress.XtraBars.Docking2010.Views.WindowsUI.FlyoutCommand() { Text = "OK", Result = System.Windows.Forms.DialogResult.OK };
                //action.Commands.Add(command1);
                //DevExpress.XtraBars.Docking2010.Customization.FlyoutDialog.Show(this, action);

                flyoutPanel2.ShowPopup();
            }
            if (listView3.SelectedIndices[0] == 1)
            {
                flyoutPanel6.ShowPopup();
            }
        }

        private void ListView2_Click(object sender, EventArgs e)
        {
            if (listView2.SelectedIndices.Count < 1) return;
            if (listView2.SelectedIndices[0] == 0)
            {
                selectCollection = "";
                onSearch();

            }
            else
            {
                textBox1.Text = "";
                selectCollection = listView2.SelectedItems[0].Text;
                onSearch();
            }
        }

        private void ListView1_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedIndices.Count < 1) return;
            if (listView1.SelectedIndices[0] == 0)
            {
                //string s = "test";
                //DevExpress.XtraBars.Docking2010.Views.WindowsUI.FlyoutAction action = new DevExpress.XtraBars.Docking2010.Views.WindowsUI.FlyoutAction() { Caption = "Thông tin nhà phát triển", Description = s };
                //DevExpress.XtraBars.Docking2010.Views.WindowsUI.FlyoutCommand command1 = new DevExpress.XtraBars.Docking2010.Views.WindowsUI.FlyoutCommand() { Text = "OK", Result = System.Windows.Forms.DialogResult.OK };
                //action.Commands.Add(command1);
                //DevExpress.XtraBars.Docking2010.Customization.FlyoutDialog.Show(this, action);
                listView6.Items.Clear();
                selectCollection = "";
                List<string> colList = mainProc.collectionListing();
                if (colList.Count > 0)
                {
                    foreach (var entry in colList)
                    {
                        listView6.Items.Add(new ListViewItem(new string[] { entry }, "hiclipart.com (4).png", System.Drawing.SystemColors.Window, System.Drawing.Color.Empty, null));
                    }
                }
                flyoutPanel3.Height = (int)(ClientSize.Height * 0.8);
                flyoutPanel3.ShowPopup();
            }
            if (listView1.SelectedIndices[0] == 1)
            {
                //string s = "test";
                //DevExpress.XtraBars.Docking2010.Views.WindowsUI.FlyoutAction action = new DevExpress.XtraBars.Docking2010.Views.WindowsUI.FlyoutAction() { Caption = "Thông tin nhà phát triển", Description = s };
                //DevExpress.XtraBars.Docking2010.Views.WindowsUI.FlyoutCommand command1 = new DevExpress.XtraBars.Docking2010.Views.WindowsUI.FlyoutCommand() { Text = "OK", Result = System.Windows.Forms.DialogResult.OK };
                //action.Commands.Add(command1);
                //DevExpress.XtraBars.Docking2010.Customization.FlyoutDialog.Show(this, action);
                flyoutPanel1.ShowPopup();
            }
            if (listView1.SelectedIndices[0]==2)
            {
                flyoutPanel4.Height = (int)(panel8.Height);
                flyoutPanel4.ShowPopup();
            }
        }

        private void Button14_Click(object sender, EventArgs e)
        {
            if (textBox4.Text == "") return;
            textBox5.Text = mainProc.translatePhrase(textBox4.Text);
        }

        private void Button15_Click(object sender, EventArgs e)
        {
            if (textBox4.Text == "") return;
            mainProc.speakWord(textBox4.Text);
        }

        private void Button16_Click(object sender, EventArgs e)
        {
            contextMenuStrip1.Show(button16, button16.Size.Width, button16.Size.Height);
            foreach (ToolStripMenuItem i in contextMenuStrip1.Items)
            {
                i.ForeColor = Color.White;
            }
        }

        private void Button19_Click(object sender, EventArgs e)
        {

        }

        //flyoutPanel5, xtraTabControl3: meaning, textBox7-8: pronounce+word, button26-27-28: ?-save-image, richTextBox5-6: base-special meaningBox 
        private void Button25_Click(object sender, EventArgs e)
        {
            //word edit
            if (curNode == null) return;
            flyoutPanel5.Height = panel8.Height;
            flyoutPanel5.ShowPopup();
            string pronounce;
            string img_path;
            richTextBox5.Text = mainProc.outputWordBaseInfo(curNode.Text, out pronounce, out img_path);
            richTextBox6.Text = mainProc.outputWordSpecialInfo(curNode.Text);
            textBox7.Text = pronounce;
            textBox30.Text = img_path;
            textBox8.Text = curNode.Text;
            textBox8.Enabled = false;
        }

        private void Button23_Click(object sender, EventArgs e)
        {
            //word insert
            flyoutPanel5.Height = panel8.Height;
            flyoutPanel5.ShowPopup();
            richTextBox5.Text = "-> Danh từ\n\t- Nghĩa\n\t\t+ Ví dụ\n\t\t* Nghĩa ví dụ";
            richTextBox6.Text = "";
            textBox7.Text = "";
            textBox8.Text = "";
            textBox8.Enabled = true;
        }

        private void Button26_Click(object sender, EventArgs e)
        {
            contextMenuStrip2.Show(button26, button26.Size.Width, button26.Size.Height);
            foreach (ToolStripMenuItem i in contextMenuStrip2.Items)
            {
                i.ForeColor = Color.White;
            }
        }

        //right mouse on word
        private void TreeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            curNode = e.Node;
            reloadMeaningPanel();
            if (e.Button==MouseButtons.Right)
            {
                Danhsach.DropDownItems.Clear();
                List<string> colList = mainProc.collectionListing();
                if (colList.Count > 0)
                {
                    foreach (var entry in colList)
                    {
                        Danhsach.DropDownItems.Add(entry);
                    }
                    foreach (ToolStripMenuItem i in Danhsach.DropDownItems)
                    {
                        //IF DARK MODE
                        i.BackColor = Color.FromArgb(41, 45, 51);
                        i.ForeColor = Color.White;
                    }
                }
                
                contextMenuStrip3.Show(treeView1,e.Location);
            }
        }

        private void TreeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            treeView1.SelectedNode = null;
        }

        private void ContextMenuStrip3_MouseClick(object sender, MouseEventArgs e)
        {
        }

        private void Button9_Click(object sender, EventArgs e)
        {
            //delete collection
            DevExpress.XtraBars.Docking2010.Views.WindowsUI.FlyoutAction action = new DevExpress.XtraBars.Docking2010.Views.WindowsUI.FlyoutAction() { Caption = "Xác nhận", Description = "Bạn muốn xóa bộ sưu tập này??" };
            Predicate<DialogResult> predicate = canCloseFunc;
            DevExpress.XtraBars.Docking2010.Views.WindowsUI.FlyoutCommand command1 = new DevExpress.XtraBars.Docking2010.Views.WindowsUI.FlyoutCommand() { Text = "Có", Result = System.Windows.Forms.DialogResult.Yes };
            DevExpress.XtraBars.Docking2010.Views.WindowsUI.FlyoutCommand command2 = new DevExpress.XtraBars.Docking2010.Views.WindowsUI.FlyoutCommand() { Text = "Không", Result = System.Windows.Forms.DialogResult.No };
            action.Commands.Add(command1);
            action.Commands.Add(command2);
            DevExpress.XtraBars.Docking2010.Views.WindowsUI.FlyoutProperties properties = new DevExpress.XtraBars.Docking2010.Views.WindowsUI.FlyoutProperties();
            properties.ButtonSize = new Size(100, 40);
            properties.Style = DevExpress.XtraBars.Docking2010.Views.WindowsUI.FlyoutStyle.MessageBox;
            if (DevExpress.XtraBars.Docking2010.Customization.FlyoutDialog.Show(this, action, properties, predicate) == System.Windows.Forms.DialogResult.Yes)
            {
                mainProc.removeCollection(tempSelectCollection);
                listView7.Items.Clear();
                tempSelectCollection = "";
                listView2.Items.Clear();
                listView6.Items.Clear();
                listView2.Items.Add(new ListViewItem(new string[] { "Tất cả các từ" }, "hiclipart.com (4).png", System.Drawing.SystemColors.Window, System.Drawing.Color.Empty, null));
                List<string> colList = mainProc.collectionListing();
                if (colList.Count > 0)
                {
                    foreach (var entry in colList)
                    {
                        listView2.Items.Add(new ListViewItem(new string[] { entry }, "hiclipart.com (4).png", System.Drawing.SystemColors.Window, System.Drawing.Color.Empty, null));
                        listView6.Items.Add(new ListViewItem(new string[] { entry }, "hiclipart.com (4).png", System.Drawing.SystemColors.Window, System.Drawing.Color.Empty, null));
                    }
                }
            }
        }

        private void Button10_Click(object sender, EventArgs e)
        {
            if (textBox2.Text == "")
            {
                return;
            }
            mainProc.insertCollection(textBox2.Text);
            listView2.Items.Clear();
            listView6.Items.Clear();
            listView2.Items.Add(new ListViewItem(new string[] { "Tất cả các từ" }, "hiclipart.com (4).png", System.Drawing.SystemColors.Window, System.Drawing.Color.Empty, null));
            List<string> colList = mainProc.collectionListing();
            if (colList.Count > 0)
            {
                foreach (var entry in colList)
                {
                    listView2.Items.Add(new ListViewItem(new string[] { entry }, "hiclipart.com (4).png", System.Drawing.SystemColors.Window, System.Drawing.Color.Empty, null));
                    listView6.Items.Add(new ListViewItem(new string[] { entry }, "hiclipart.com (4).png", System.Drawing.SystemColors.Window, System.Drawing.Color.Empty, null));
                }
            }
        }

        private void Button24_Click(object sender, EventArgs e)
        {
            //delete word (permanent or from collection - context based)


            if (curNode == null)
            {
                DevExpress.XtraBars.Docking2010.Views.WindowsUI.FlyoutAction action2 = new DevExpress.XtraBars.Docking2010.Views.WindowsUI.FlyoutAction() { Caption = "Thông báo", Description = "Xin hãy chọn từ trước khi xóa" };
                Predicate<DialogResult> predicate2 = canCloseFunc;
                DevExpress.XtraBars.Docking2010.Views.WindowsUI.FlyoutCommand command1_2 = new DevExpress.XtraBars.Docking2010.Views.WindowsUI.FlyoutCommand() { Text = "OK", Result = System.Windows.Forms.DialogResult.OK };
                action2.Commands.Add(command1_2);
                DevExpress.XtraBars.Docking2010.Views.WindowsUI.FlyoutProperties properties2 = new DevExpress.XtraBars.Docking2010.Views.WindowsUI.FlyoutProperties();
                properties2.ButtonSize = new Size(100, 40);
                properties2.Style = DevExpress.XtraBars.Docking2010.Views.WindowsUI.FlyoutStyle.MessageBox;
                DevExpress.XtraBars.Docking2010.Customization.FlyoutDialog.Show(this, action2, properties2, predicate2);
                return;
            }
            if (selectCollection != "")
            {
                DevExpress.XtraBars.Docking2010.Views.WindowsUI.FlyoutAction col_action = new DevExpress.XtraBars.Docking2010.Views.WindowsUI.FlyoutAction() { Caption = "Xác nhận", Description = "Bạn muốn xóa từ này ra khỏi bộ sưu tập?" };
                Predicate<DialogResult> col_predicate = canCloseFunc;
                DevExpress.XtraBars.Docking2010.Views.WindowsUI.FlyoutCommand col_command1 = new DevExpress.XtraBars.Docking2010.Views.WindowsUI.FlyoutCommand() { Text = "Có", Result = System.Windows.Forms.DialogResult.Yes };
                DevExpress.XtraBars.Docking2010.Views.WindowsUI.FlyoutCommand col_command2 = new DevExpress.XtraBars.Docking2010.Views.WindowsUI.FlyoutCommand() { Text = "Không", Result = System.Windows.Forms.DialogResult.No };
                col_action.Commands.Add(col_command1);
                col_action.Commands.Add(col_command2);
                DevExpress.XtraBars.Docking2010.Views.WindowsUI.FlyoutProperties col_properties = new DevExpress.XtraBars.Docking2010.Views.WindowsUI.FlyoutProperties();
                col_properties.ButtonSize = new Size(100, 40);
                col_properties.Style = DevExpress.XtraBars.Docking2010.Views.WindowsUI.FlyoutStyle.MessageBox;
                if (DevExpress.XtraBars.Docking2010.Customization.FlyoutDialog.Show(this, col_action, col_properties, col_predicate) == System.Windows.Forms.DialogResult.Yes)
                {
                    mainProc.removeFromCollection(curNode.Text, selectCollection);
                    onSearch();
                }
            }
            else
            {
                DevExpress.XtraBars.Docking2010.Views.WindowsUI.FlyoutAction action = new DevExpress.XtraBars.Docking2010.Views.WindowsUI.FlyoutAction() { Caption = "Xác nhận", Description = "Bạn muốn xóa từ này vĩnh viễn??" };
                Predicate<DialogResult> predicate = canCloseFunc;
                DevExpress.XtraBars.Docking2010.Views.WindowsUI.FlyoutCommand command1 = new DevExpress.XtraBars.Docking2010.Views.WindowsUI.FlyoutCommand() { Text = "Có", Result = System.Windows.Forms.DialogResult.Yes };
                DevExpress.XtraBars.Docking2010.Views.WindowsUI.FlyoutCommand command2 = new DevExpress.XtraBars.Docking2010.Views.WindowsUI.FlyoutCommand() { Text = "Không", Result = System.Windows.Forms.DialogResult.No };
                action.Commands.Add(command1);
                action.Commands.Add(command2);
                DevExpress.XtraBars.Docking2010.Views.WindowsUI.FlyoutProperties properties = new DevExpress.XtraBars.Docking2010.Views.WindowsUI.FlyoutProperties();
                properties.ButtonSize = new Size(100, 40);
                properties.Style = DevExpress.XtraBars.Docking2010.Views.WindowsUI.FlyoutStyle.MessageBox;
                if (DevExpress.XtraBars.Docking2010.Customization.FlyoutDialog.Show(this, action, properties, predicate) == System.Windows.Forms.DialogResult.Yes)
                {
                    mainProc.removeWord(curNode.Text);
                    onSearch();
                }
            }
        }

        private void ListView6_Click(object sender, EventArgs e)
        {
            if (listView6.SelectedItems.Count < 1) return;
            tempSelectCollection = listView6.SelectedItems[0].Text;
            List<string> result = mainProc.collectionWordListing(listView6.SelectedItems[0].Text);
            listView7.Items.Clear();
            listView7.BeginUpdate();
            foreach (var entry in result)
            {
                listView7.Items.Add(entry);
            }
            listView7.EndUpdate();
        }

        private void XtraForm1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                onSearch();
            else if (e.KeyCode == Keys.Down)
                traverseWordList(1);
            else if (e.KeyCode == Keys.Up)
                traverseWordList(0);
            //else if (e.KeyCode == Keys.Left)
            //    traverseHistory(0);
            //else if (e.KeyCode == Keys.Right)
            //    traverseHistory(1);
        }

        private void Danhsach_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            mainProc.addToCollection(curNode.Text, e.ClickedItem.Text);
        }

        private void Button12_Click(object sender, EventArgs e)
        {
            if (listView7.SelectedItems.Count == 0)
            {
                DevExpress.XtraBars.Docking2010.Views.WindowsUI.FlyoutAction action2 = new DevExpress.XtraBars.Docking2010.Views.WindowsUI.FlyoutAction() { Caption = "Thông báo", Description = "Hãy chọn ít nhất 1 từ để xóa khỏi bộ sưu tập" };
                Predicate<DialogResult> predicate2 = canCloseFunc;
                DevExpress.XtraBars.Docking2010.Views.WindowsUI.FlyoutCommand command1_2 = new DevExpress.XtraBars.Docking2010.Views.WindowsUI.FlyoutCommand() { Text = "OK", Result = System.Windows.Forms.DialogResult.OK };
                action2.Commands.Add(command1_2);
                DevExpress.XtraBars.Docking2010.Views.WindowsUI.FlyoutProperties properties2 = new DevExpress.XtraBars.Docking2010.Views.WindowsUI.FlyoutProperties();
                properties2.ButtonSize = new Size(100, 40);
                properties2.Style = DevExpress.XtraBars.Docking2010.Views.WindowsUI.FlyoutStyle.MessageBox;
                DevExpress.XtraBars.Docking2010.Customization.FlyoutDialog.Show(this, action2, properties2, predicate2);
                return;
            } //replace with flyout dialog
            foreach(ListViewItem word in listView7.SelectedItems)
            {
                mainProc.removeFromCollection(word.Text, tempSelectCollection);
                List<string> result = mainProc.collectionWordListing(listView6.SelectedItems[0].Text);
                listView7.Items.Clear();
                listView7.BeginUpdate();
                foreach (var entry in result)
                {
                    listView7.Items.Add(entry);
                }
                listView7.EndUpdate();
            }
        }

        private void Button27_Click(object sender, EventArgs e)
        {
            //Save button
            bool res;
            string resultMessage = "Lỗi không xác định xảy ra";
            if (textBox8.Enabled == false)
            {
                //save edited entry
                res = mainProc.saveWord(textBox8.Text, textBox30.Text, textBox7.Text, richTextBox5.Text, richTextBox6.Text, textBox8.Text);
            }
            else
            {
                //save new entry
                res = mainProc.saveWord(textBox8.Text, textBox30.Text, textBox7.Text, richTextBox5.Text, richTextBox6.Text);
            }

            if (res == true) resultMessage = "Lưu từ vựng thành công";
            else resultMessage = "Lưu từ vựng không thành công";

            DevExpress.XtraBars.Docking2010.Views.WindowsUI.FlyoutAction action = new DevExpress.XtraBars.Docking2010.Views.WindowsUI.FlyoutAction() { Caption = "Thông báo", Description = resultMessage };
            Predicate<DialogResult> predicate = canCloseFunc;
            DevExpress.XtraBars.Docking2010.Views.WindowsUI.FlyoutCommand command1 = new DevExpress.XtraBars.Docking2010.Views.WindowsUI.FlyoutCommand() { Text = "OK", Result = System.Windows.Forms.DialogResult.OK };
            action.Commands.Add(command1);
            DevExpress.XtraBars.Docking2010.Views.WindowsUI.FlyoutProperties properties = new DevExpress.XtraBars.Docking2010.Views.WindowsUI.FlyoutProperties();
            properties.ButtonSize = new Size(100, 40);
            properties.Style = DevExpress.XtraBars.Docking2010.Views.WindowsUI.FlyoutStyle.MessageBox;
            DevExpress.XtraBars.Docking2010.Customization.FlyoutDialog.Show(this, action, properties, predicate);

            if (textBox8.Enabled == false) reloadMeaningPanel();
            flyoutPanel5.HidePopup();

            textBox30.Text = "";

            this.Activate();
            
        }

        private void Button28_Click(object sender, EventArgs e)
        {
            string filePath = string.Empty;
            OpenFileDialog a = new OpenFileDialog();
            a.InitialDirectory = "c:\\";
            a.Filter = "Image File (*.png, *.jpg, *.bmp)|*.jpg;*.png;*.bmp";
            a.FilterIndex = 1;
            a.RestoreDirectory = true;

            if (a.ShowDialog() == DialogResult.OK)
            {
                //Get the path of specified file
                textBox30.Text = a.FileName;
            }
        }
        //collection panel, collection list: listview6, col edit-delete-insert: button8-9-10, word list: listview7, bookmark name box: textbox2
    }
}
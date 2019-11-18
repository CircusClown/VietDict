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
            if (e.KeyChar == (char)Keys.Enter)
                onSearch();
            else if (e.KeyChar == (char)Keys.Down)
                traverseWordList(1);
            else if (e.KeyChar == (char)Keys.Up)
                traverseWordList(0);
            else if (e.KeyChar == (char)Keys.Left)
                traverseHistory(0);
            else if (e.KeyChar == (char)Keys.Right)
                traverseHistory(1);
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
            }
            
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            onSearch();
        }

        private void TreeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            curNode = e.Node;
            reloadMeaningPanel();
        }

        private void reloadMeaningPanel()
        {
            string pronounce;
            richTextBox1.Text = mainProc.outputWordBaseInfo(curNode.Text, out pronounce);
            FormatText();
            richTextBox2.Text = mainProc.outputWordSpecialInfo(curNode.Text);
            label3.Text = curNode.Text;
            label4.Text = pronounce;
            isBookmarked = mainProc.isBookmarked(curNode.Text);
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
        }

        private void ListView2_Click(object sender, EventArgs e)
        {
            if (listView2.SelectedIndices.Count < 1) return;
            if (listView2.SelectedIndices[0] == 0)
            {
                selectCollection = "";
                List<string> result = mainProc.wordListing();
                treeView1.BeginUpdate();
                treeView1.Nodes.Clear();
                foreach (var entry in result)
                {
                    treeView1.Nodes.Add(entry);
                }
                treeView1.EndUpdate();
                
            }
            else
            {
                selectCollection = listView2.SelectedItems[0].Text;
                List<string> result = mainProc.collectionWordListing(selectCollection);
                treeView1.BeginUpdate();
                treeView1.Nodes.Clear();
                foreach (var entry in result)
                {
                    treeView1.Nodes.Add(entry);
                }
                treeView1.EndUpdate();
            }
        }
    }
}
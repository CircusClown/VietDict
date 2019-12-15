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
using System.Text.RegularExpressions;
using System.Configuration;


namespace VietDict
{
    public partial class XtraForm1 : DevExpress.XtraEditors.XtraForm
    {
        process mainProc;
        TreeNode curNode;
        string selectCollection = "";
        bool isBookmarked;
        int light = 1;
        Color backcolor = Color.White;
        Color forecolor = Color.Black;
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
                    ListViewItem newlistviewitem = new ListViewItem(new string[] { entry }, 0, System.Drawing.SystemColors.Window, System.Drawing.Color.Empty, null);
                    newlistviewitem.Tag = "control";
                    listView2.Items.Add(newlistviewitem);
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
                    button4.ImageIndex=button4.ImageIndex+2;
                }
            }
            else
            {
                res = mainProc.addToBookmark(curNode.Text);
                {
                    isBookmarked = true;
                    button4.ImageIndex= button4.ImageIndex-2;
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
                string rtbstr = richTextBox1.SelectedText;
                

                if (Regex.Match(rtbstr,@"^\s*\+").Length != 0) 
                {
                    richTextBox1.SelectionColor = Color.FromArgb(191, 201, 38);               
                    richTextBox1.Select(richTextBox1.GetFirstCharIndexFromLine(i), 5);
                    richTextBox1.SelectionColor = Color.Aqua;
                }
                if (Regex.Match(rtbstr, @"^\s*\*").Length != 0)
                {
                    richTextBox1.SelectionColor = Color.FromArgb(191, 150, 100);
                    richTextBox1.Select(richTextBox1.GetFirstCharIndexFromLine(i), 5);
                    richTextBox1.SelectionColor = Color.Aqua;
                    richTextBox1.SelectedText = "\t\t =";
                }
                if ((Regex.Match(rtbstr, @"^\s*\-").Length != 0) && (Regex.Match(rtbstr, @"^\s*\->").Length == 0))
                {
                    richTextBox1.SelectionColor = Color.LightBlue;
                    richTextBox1.Select(richTextBox1.GetFirstCharIndexFromLine(i), 4);
                    richTextBox1.SelectionColor = Color.Aqua;
                }
                if (Regex.Match(rtbstr, @"^\s*\->").Length != 0)
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
            light = Int32.Parse(ConfigurationManager.AppSettings["LightMode"]);
            if (light == 0) todarkmode();
            else tolightmode();
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
            if (DevExpress.XtraBars.Docking2010.Customization.FlyoutDialog.Show(this, action, properties, predicate) == System.Windows.Forms.DialogResult.Yes)
            {
                e.Cancel = false;
                UpdateAppSettings("LightMode", light.ToString());                
            }
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
                flyoutPanel6.Width = panel1.Width;
                flyoutPanel6.ShowPopup();
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
            if (listView1.SelectedIndices[0] == 2)
            {
                flyoutPanel4.Height = (int)(panel8.Height);
                flyoutPanel4.ShowPopup();
            }
            if (listView1.SelectedIndices[0] == 3)
            {
                flyoutPanel7.Width = panel1.Width;
                flyoutPanel7.ShowPopup();
            }
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

        private void Button25_Click(object sender, EventArgs e)
        {
            flyoutPanel5.Height = panel8.Height;
            flyoutPanel5.ShowPopup();
        }

        private void Button23_Click(object sender, EventArgs e)
        {
            flyoutPanel5.Height = panel8.Height;
            flyoutPanel5.ShowPopup();
        }

        private void Button26_Click(object sender, EventArgs e)
        {
            contextMenuStrip2.Show(button26, button26.Size.Width, button26.Size.Height);
            foreach (ToolStripMenuItem i in contextMenuStrip2.Items)
            {
                i.ForeColor = Color.White;
            }
        }

        private void TreeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            curNode = e.Node;
            reloadMeaningPanel();
            if (e.Button == MouseButtons.Right)
            {
                contextMenuStrip3.Show(treeView1, e.Location);
                foreach (ToolStripMenuItem i in contextMenuStrip3.Items)
                {
                    i.ForeColor = Color.White;
                }
            }
        }

        private void TreeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            treeView1.SelectedNode = null;
        }

        private void ContextMenuStrip3_MouseClick(object sender, MouseEventArgs e)
        {
        }

        private void Button24_Click(object sender, EventArgs e)
        {

        }


       

        private void ButtonLightmode(Control parent)
        {
            foreach (Control control in parent.Controls)
            {
                if (control.Tag == null) return;
                if (((control.Tag.ToString() == "control") || ((control.Tag.ToString() == "controlbtn"))))
                {
                    control.BackColor = backcolor;
                    control.ForeColor = forecolor;
                }
                if (control is ListView)
                {
                    foreach(ListViewItem i in (control as ListView).Items)
                    {
                        i.BackColor = backcolor;
                        i.ForeColor = forecolor;
                    }
                }
                if(control.Tag.ToString() == "controlbtn")
                {
                    if(light ==0)
                    (control as Button).ImageList = imageList8;
                    else if(light==1)
                    (control as Button).ImageList = imageList9;
                }
                
                ButtonLightmode(control);
            }
        }
        private void todarkmode()
        {
            DevExpress.LookAndFeel.UserLookAndFeel.Default.SetSkinStyle("The Bezier", "Office White");
            backcolor = Color.White;
            forecolor = Color.Black;
            light = 0;
            ButtonLightmode(this);
            ButtonLightmode(flyoutPanel1);
            ButtonLightmode(flyoutPanel2);
            ButtonLightmode(flyoutPanel3);
            ButtonLightmode(flyoutPanel4);
            ButtonLightmode(flyoutPanel5);
            ButtonLightmode(flyoutPanel6);
            ButtonLightmode(flyoutPanel7);
            listView1.LargeImageList = imageList4;
            listView2.LargeImageList = imageList6;
            listView3.LargeImageList = imageList4;
            button4.ImageIndex=0;
        }
        private void tolightmode()
        {
            DevExpress.LookAndFeel.UserLookAndFeel.Default.SetSkinStyle("The Bezier", "Dark mode");
            light = 1;
            backcolor = Color.FromArgb(41, 45, 51);
            forecolor = Color.White;
            ButtonLightmode(this);
            ButtonLightmode(flyoutPanel1);
            ButtonLightmode(flyoutPanel2);
            ButtonLightmode(flyoutPanel3);
            ButtonLightmode(flyoutPanel4);
            ButtonLightmode(flyoutPanel5);
            ButtonLightmode(flyoutPanel6);
            ButtonLightmode(flyoutPanel7);
            listView1.LargeImageList = imageList5;
            listView2.LargeImageList = imageList2;
            listView3.LargeImageList = imageList5;
            button4.ImageIndex=2;
        }
        private void ToggleSwitch1_Toggled(object sender, EventArgs e)
        {
            if (light == 1)
            {
                todarkmode();
            }
            else if (light == 0)
            {
                tolightmode();
            }
        }

        private void BindingSource1_CurrentChanged(object sender, EventArgs e)
        {

        }
        private void UpdateAppSettings(string theKey, string theValue)
        {
            Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            if (ConfigurationManager.AppSettings.AllKeys.Contains(theKey))
            {
                configuration.AppSettings.Settings[theKey].Value = theValue;
            }

            configuration.Save(ConfigurationSaveMode.Modified);

            ConfigurationManager.RefreshSection("appSettings");
        }
    }


}

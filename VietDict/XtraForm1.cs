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
        }

        private void Button5_Click(object sender, EventArgs e)
        {

        }

        private void TextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void Label3_Click(object sender, EventArgs e)
        {

        }

        private void Button7_Click(object sender, EventArgs e)
        {

        }

        private void Button4_Click(object sender, EventArgs e)
        {

        }

        private void Button1_Click(object sender, EventArgs e)
        {
            List<string> result = mainProc.wordQuery(textBox1.Text);
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
                string pronounce;
                richTextBox1.Text = mainProc.outputWordBaseInfo(curNode.Text, out pronounce);
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
                    if ((richTextBox1.SelectedText.Contains("-") == true)&& (richTextBox1.SelectedText.Contains("->") == false))
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
                richTextBox2.Text = mainProc.outputWordSpecialInfo(curNode.Text);
                label3.Text = curNode.Text;
                label4.Text = pronounce;
            }
        }

        private void TreeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            curNode = e.Node;
            string pronounce;
            richTextBox1.Text = mainProc.outputWordBaseInfo(curNode.Text, out pronounce);
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
            richTextBox2.Text = mainProc.outputWordSpecialInfo(curNode.Text);
            label3.Text = curNode.Text;
            label4.Text = pronounce;
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            mainProc.speakWord(label3.Text);
        }

        private void XtraForm1_Load(object sender, EventArgs e)
        {
           Thread.Sleep(2000);
        }

    }
}
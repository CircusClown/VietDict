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

namespace VietDict
{
    //gửi Hiệu, do giờ tau chưa đưa mi cái db đc nên chịu khó comment out hết đống này để design mà không vướng code của tau 
    public partial class Vietdict : DevExpress.XtraEditors.XtraForm
    {
        process mainProc;
        public Vietdict()
        {
            InitializeComponent();
            mainProc = new process();
            List<string> allWord = mainProc.wordListing();
            if (allWord == null) return;
            this.treeView1.BeginUpdate();
            foreach(var x in allWord)
            {
                treeView1.Nodes.Add(x);
            }
            this.treeView1.EndUpdate();
        }

        private void Label1_Click(object sender, EventArgs e)
        {

        }

        private void ListView2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void SimpleButton4_Click(object sender, EventArgs e)
        {
            string query = textBox1.Text;
            List<string> allWord = mainProc.wordQuery(query);
            if (allWord == null) return;
            this.treeView1.BeginUpdate();
            treeView1.Nodes.Clear();
            foreach (var x in allWord)
            {
                treeView1.Nodes.Add(x);
            }
            this.treeView1.EndUpdate();
        }

        private void Vietdict_Load(object sender, EventArgs e)
        {

        }

        private void MenuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void TextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void TreeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            TreeNode x = e.Node;
            richTextBox1.Text = mainProc.outputWordInfo(x.Text);
        }
    }
}
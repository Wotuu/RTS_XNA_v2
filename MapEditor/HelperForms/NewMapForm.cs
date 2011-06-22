using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MapEditor.HelperForms
{
    public partial class NewMapForm : Form
    {
        public int MapWidth;
        public int MapHeight;
        public NewMapForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //TODO
            // Check Numeric values
            this.MapWidth = int.Parse(TbWidth.Text);
            this.MapHeight = int.Parse(TbHeight.Text);
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Camera_Record
{
    public partial class IMGBOX : Form
    {
        public IMGBOX()
        {
            InitializeComponent();
        }

        public IMGBOX(Image img)
        {
            InitializeComponent();
            pictureBox1.Image = img;
        }    

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

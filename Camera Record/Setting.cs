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
    public partial class Setting : Form
    {
        public Setting()
        {
            InitializeComponent();
        }

        private void Light_Tbar_Scroll(object sender, EventArgs e)
        {
            Light_TB.Text = Light_Tbar.Value.ToString();

        }

        private void Contrasting_Tbar_Scroll(object sender, EventArgs e)
        {
            Contrasting_TB.Text = Contrasting_Tbar.Value.ToString();
        }

        private void Tone_Tbar_Scroll(object sender, EventArgs e)
        {
            Tone_TB.Text = Tone_Tbar.Value.ToString();
        }

        private void Saturation_TBar_Scroll(object sender, EventArgs e)
        {
            Saturation_TB.Text = Saturation_TBar.Value.ToString();
        }

        private void Clarity_Tbar_Scroll(object sender, EventArgs e)
        {
            Clarity_TB.Text = Clarity_Tbar.Value.ToString();
        }

        private void Correction_Tbar_Scroll(object sender, EventArgs e)
        {
            Correction_TB.Text = Correction_Tbar.Value.ToString();
        }

        private void WB_Tbar_Scroll(object sender, EventArgs e)
        {
            WB_TB.Text = WB_Tbar.Value.ToString();
        }

        private void Backlight_Tbar_Scroll(object sender, EventArgs e)
        {
            Backlight_TB.Text = Backlight_Tbar.Value.ToString();
        }

        private void Gain_Tbar_Scroll(object sender, EventArgs e)
        {
            Gain_TB.Text = Gain_Tbar.Value.ToString();
        }

        private void FL_Tbar_Scroll(object sender, EventArgs e)
        {
            FL_TB.Text = FL_Tbar.Value.ToString();
        }

        private void Setting_Load(object sender, EventArgs e)
        {

        }

        private void init()
        {

        }

        private void readini()
        {

        }

    }
}

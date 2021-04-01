using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace Lines
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            comboBox1.SelectedIndex = 0;
            lines1.onScoreEdited += Lines1_onScoreEdited;
        }

        private void Lines1_onScoreEdited()
        {
            label1.Text = $"Счёт: {lines1.score}";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            lines1.SetBalls();
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (lines1.Width > ClientSize.Width && ClientSize.Width!=0) 
            {
                lines1.Width = ClientSize.Width;
                Refresh();
            }
            if (lines1.Height > ClientSize.Height && ClientSize.Height != 0) 
            {
                lines1.Height = ClientSize.Height;
                Refresh();
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    lines1.sizeField = 10;
                    break;
                case 1:
                    lines1.sizeField = 12;
                    break;
                case 2:
                    lines1.sizeField = 14;
                    break;  
            }
        }
    }
}

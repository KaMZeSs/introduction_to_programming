using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab3
{
    public partial class Form2 : Form
    {
        public SortedList<string, object> parameters { get; set; }

        public Form2()
        {
            InitializeComponent();
            parameters = new SortedList<string, object>();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            parameters.Add("Contrast", trackBar_Contrast.Value);
            parameters.Add("Brightness", trackBar_Brightness.Value);
            parameters.Add("R", trackBar_R.Value);
            parameters.Add("G", trackBar_G.Value);
            parameters.Add("B", trackBar_B.Value);
            this.DialogResult = DialogResult.OK;
        }
    }
}

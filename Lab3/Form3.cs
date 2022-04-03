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
    public partial class Form3 : Form
    {
        public Form3(Bitmap bmp)
        {
            InitializeComponent();
            this.pictureBox1.Image = bmp;
            this.pictureBox1.Size = bmp.Size;
        }
    }
}

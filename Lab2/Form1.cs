using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab2
{
    public partial class Form1 : Form
    {
        Bitmap image;
        Bitmap result;
        int handlerNum = 0;
        ImageProcess imageProcess;

        public Form1()
        {
            InitializeComponent();
        }

        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Image Files(*.BMP;*.JPG;*.PNG)|*.BMP;*.JPG;*.PNG";
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                image = new Bitmap(fileDialog.FileName);
                this.Source.Size = image.Size;
                this.Source.Image = image;
                фильтрToolStripMenuItem.Enabled = true;
            }
        }

        private void сохранитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Image Files(*.BMP;*.JPG;*.PNG)|*.BMP;*.JPG;*.PNG";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                result.Save(saveFileDialog.FileName);
            }
        }

        private async void фильтрToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2 form = new Form2();
            form.ShowDialog();
            if (form.DialogResult == DialogResult.OK)
            {
                handlerNum++;
                imageProcess = new ImageProcess(handlerNum);
                imageProcess.Source = new Bitmap(image);
                imageProcess.init(form.parameters);

                await Task.Factory.StartNew(() => imageProcess.startHandle(changeProgress));
                
                result = imageProcess.Result;
                this.Result.Size = result.Size;
                this.Result.Image = result;
            }
        }

        private void changeProgress(double progress)
        {
            progressBar1.BeginInvoke(new ImageWorker.ProgressDelegate((x) => { progressBar1.Value = (int)(x * 100d); }), progress);
        }
    }
}

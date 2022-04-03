using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Lab3
{
    public partial class Form1 : Form
    {
        Bitmap[] images;
        Bitmap[] results;

        public Form1()
        {
            InitializeComponent();
        }

        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Multiselect = true;
            fileDialog.Filter = "Image Files(*.BMP;*.JPG;*.PNG)|*.BMP;*.JPG;*.PNG";
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                if (images == null)
                {
                    images = new Bitmap[fileDialog.FileNames.Length];
                    for (int i = 0; i < images.Length; i++)
                        images[i] = new Bitmap(fileDialog.FileNames[i]);
                }
                else
                {
                    Bitmap[] temp = new Bitmap[fileDialog.FileNames.Length];
                    for (int i = 0; i < temp.Length; i++)
                        temp[i] = new Bitmap(fileDialog.FileNames[i]);
                    List<Bitmap> list = new List<Bitmap>();
                    list.AddRange(images);
                    list.AddRange(temp);
                    images = list.ToArray();
                }
                dataGridView1.Rows.Clear();
                for (int i = 0; i < images.Length; i++)
                {
                    dataGridView1.Rows.Add(System.IO.Path.GetFileName(fileDialog.FileNames[i]), images[i].Width, images[i].Height, 0, i);
                }
            }
        }

        private void сохранитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.Description = "Выберите папку для сохранения файлов";
            folderBrowserDialog.ShowNewFolderButton = true;
            folderBrowserDialog.UseDescriptionForTitle = true;
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                for (int i = 0; i < images.Length; i++)
                {
                    results[i].Save($"{folderBrowserDialog.SelectedPath}\\result_{dataGridView1["NameColumn", i].Value}");
                }
            }
        }
        
        ImageProcess[] imageProcesses;
        bool isWorking;
        private async void фильтрToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2 form = new Form2();
            form.ShowDialog();
            if (form.DialogResult == DialogResult.OK)
            {
                if (images == null)
                    return;
                if (images.Length == 0)
                    return;
                isWorking = true;
                файлToolStripMenuItem.Enabled = false;
                фильтрToolStripMenuItem.Enabled = false;
                int handlerNum = 0;
                results = new Bitmap[images.Length];
                imageProcesses = new ImageProcess[images.Length];

                Task[] tasks = new Task[images.Length];

                for (int i = 0; i < images.Length; i++, handlerNum++)
                {
                    imageProcesses[i] = new ImageProcess(handlerNum);
                    imageProcesses[i].Source = new Bitmap(images[i]);
                    imageProcesses[i].init(form.parameters);
                    var temp = i;
                    tasks[i] = new Task(() => imageProcesses[temp].startHandle(changeProgress));
                }
                for (int i = 0; i < tasks.Length; i++)
                    tasks[i].Start();

                await Task.WhenAll(tasks);

                //using (ManualResetEvent resetEvent = new ManualResetEvent(false))
                //{
                //    for (int i = 0; i < imageProcesses.Length; i++)
                //    {
                //        int temp = i;
                //        ThreadPool.QueueUserWorkItem(new WaitCallback(delegate (object state)
                //        {
                //            imageProcesses[temp].startHandle(changeProgress);
                //            if (Interlocked.Decrement(ref handlerNum) == 0)
                //                resetEvent.Set();
                //        }), null);
                //    }
                //    await Task.Factory.StartNew(() => { resetEvent.WaitOne(); });
                //}

                for (int i = 0; i < results.Length; i++)
                    results[i] = new Bitmap(imageProcesses[i].Result);
                imageProcesses = null;
                isWorking = false;

                MessageBox.Show("Completed");
                файлToolStripMenuItem.Enabled = true;
                фильтрToolStripMenuItem.Enabled = true;
            }
        }
        private void changeProgress(double progress, int number)
        {
            BeginInvoke(new ImageWorker.ProgressDelegate((x, number) => 
            {
                DataGridViewColumn column = dataGridView1.Columns["Id"];
                for (int i = 0, index = dataGridView1.Columns["Id"].Index; i < dataGridView1.Rows.Count; i++)
                {
                    if (dataGridView1[index, i].Value.ToString() == number.ToString())
                    {
                        dataGridView1["ProgressColumn", i].Value = (int)(x * 100d);
                        break;
                    }
                }
                 
            }), progress, number);
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.Columns[e.ColumnIndex].Name == "Buttons")
            {
                if (dataGridView1["ProgressColumn", e.RowIndex].Value.ToString() == 100.ToString())
                {
                    if (isWorking == true)
                        new Form3(imageProcesses[(int)dataGridView1["Id", e.RowIndex].Value].Result).Show();
                    else
                        new Form3(results[(int)dataGridView1["Id", e.RowIndex].Value]).Show();
                }
            }
        }

        private async void построитьГрафикToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.Description = "Выберите папку с изображениями";
            folderBrowserDialog.ShowNewFolderButton = true;
            folderBrowserDialog.UseDescriptionForTitle = true;
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                //Открытие

                dataGridView1.Rows.Clear();
                images = null;

                String[] files = Directory.GetFiles(folderBrowserDialog.SelectedPath);
                List<String> temp = new List<string>();
                for (int i = 0; i < files.Length; i++)
                {
                    String file = Path.GetFileName(files[i]).ToLower();
                    if (file.EndsWith(".jpg") | file.EndsWith(".bmp") | file.EndsWith(".png"))
                        temp.Add(files[i]);
                }

                images = new Bitmap[temp.Count];

                for (int i = 0; i < images.Length; i++)
                {
                    images[i] = new Bitmap(temp[i]);
                    dataGridView1.Rows.Add(Path.GetFileName(temp[i]), images[i].Width, images[i].Height, 0, i);
                }

                //Обработка

                Form2 form = new Form2();
                form.ShowDialog();
                if (form.DialogResult == DialogResult.OK)
                {
                    if (images == null)
                        return;
                    if (images.Length == 0)
                        return;
                    isWorking = true;
                    файлToolStripMenuItem.Enabled = false;
                    фильтрToolStripMenuItem.Enabled = false;
                    int handlerNum = 0;
                    results = new Bitmap[images.Length];
                    imageProcesses = new ImageProcess[images.Length];

                    Task[] tasks = new Task[images.Length];

                    for (int i = 0; i < images.Length; i++, handlerNum++)
                    {
                        imageProcesses[i] = new ImageProcess(handlerNum);
                        imageProcesses[i].Source = new Bitmap(images[i]);
                        imageProcesses[i].init(form.parameters);
                        var t = i;
                        tasks[i] = new Task(() => imageProcesses[t].startHandle(changeProgress));
                    }


                    Stopwatch sw = new Stopwatch();
                    long[] elapsedTime = new long[images.Length];

                    for (int i = 0; i < tasks.Length; i++)
                    {
                        sw.Start();

                        tasks[i].Start();

                        await Task.WhenAny(tasks[i]);
                        
                        sw.Stop();
                        elapsedTime[i] = sw.ElapsedMilliseconds;
                        sw.Reset();
                    }

                    

                    //for (int i = 0; i < tasks.Length; i++)
                    //    tasks[i].Start();

                    //await Task.WhenAll(tasks);

                    //using (ManualResetEvent resetEvent = new ManualResetEvent(false))
                    //{
                    //    for (int i = 0; i < imageProcesses.Length; i++)
                    //    {
                    //        int temp = i;
                    //        ThreadPool.QueueUserWorkItem(new WaitCallback(delegate (object state)
                    //        {
                    //            imageProcesses[temp].startHandle(changeProgress);
                    //            if (Interlocked.Decrement(ref handlerNum) == 0)
                    //                resetEvent.Set();
                    //        }), null);
                    //    }
                    //    await Task.Factory.StartNew(() => { resetEvent.WaitOne(); });
                    //}

                    for (int i = 0; i < results.Length; i++)
                        results[i] = new Bitmap(imageProcesses[i].Result);
                    imageProcesses = null;
                    isWorking = false;

                    MessageBox.Show("Completed");
                    файлToolStripMenuItem.Enabled = true;
                    фильтрToolStripMenuItem.Enabled = true;

                    Form4 form4 = new Form4(images, elapsedTime);
                    form4.Show();
                }

            }
        }
    }
}

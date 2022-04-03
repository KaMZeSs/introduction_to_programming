using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;
using System.Xml.Serialization;
using System.IO;
using System.Xml;

namespace Lab5
{
    public partial class Form1 : Form
    {
        public class XMLTest
        {
            public String str;
            public int[] intArray;
            public SortedList sortedList;
            public Clas clas;
            public Bitmap bmp;
        }

        public class Clas
        {
            public int int_num;
            public double double_num;
        }

        public class Bitmap
        {
            private System.Drawing.Bitmap img;
            public String bmp;

            public System.Drawing.Bitmap GetBitmap()
            {
                return img;
            }
            public void SetBitmap(System.Drawing.Bitmap img)
            {
                this.img = img;
                using (var ms = new MemoryStream())
                {
                    img.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                    byte[] arr = ms.ToArray();
                    this.bmp = Convert.ToBase64String(arr);
                }
            }
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            String[] splitted = textBox2.Text.Split(' ');
            foreach (String s in splitted)
            {
                if (s.Length == 0 | !Int32.TryParse(s, out int val))
                {
                    textBox2.ForeColor = Color.Red;
                    return;
                }
                else
                    textBox2.ForeColor = Color.Black;
            }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            String[] splitted = textBox3.Text.Split(' ');
            foreach (String s in splitted)
            {
                if (s.Length == 0 | !Int32.TryParse(s, out int val))
                {
                    textBox3.ForeColor = Color.Red;
                    return;
                }
                else
                    textBox3.ForeColor = Color.Black;
            }
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            String[] splitted = textBox4.Text.Split(' ');
            foreach (String s in splitted)
            {
                if (s.Length == 0 | !Int32.TryParse(s, out int val))
                {
                    textBox4.ForeColor = Color.Red;
                    return;
                }
                else
                    textBox4.ForeColor = Color.Black;
            }
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            String[] splitted = textBox5.Text.Split(' ');
            foreach (String s in splitted)
            {
                if (s.Length == 0 | !Double.TryParse(s.Replace('.', ','), out double val))
                {
                    textBox5.ForeColor = Color.Red;
                    return;
                }
                else
                    textBox5.ForeColor = Color.Black;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Image Files(*.BMP;*.JPG;*.PNG)|*.BMP;*.JPG;*.PNG";
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Image = new System.Drawing.Bitmap(fileDialog.FileName);
                pictureBox1.Size = pictureBox1.Image.Size;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            XMLTest xmlTest = new XMLTest();

            xmlTest.str = textBox1.Text;

            if (textBox2.Text.Length != 0)
            {
                String[] split = textBox2.Text.Split(' ');
                xmlTest.intArray = new int[split.Length];

                for (int i = 0; i < split.Length; i++)
                {
                    xmlTest.intArray[i] = int.Parse(split[i]);
                }
            }

            if (textBox3.Text.Length != 0)
            {
                String[] splitted = textBox3.Text.Split(' ');
                xmlTest.sortedList = new SortedList();
                for (int i = 0; i < splitted.Length; i++)
                {
                    xmlTest.sortedList.Add(int.Parse(splitted[i]));
                }
            }

            xmlTest.clas = new Clas();
            if (textBox4.Text.Length != 0)
                xmlTest.clas.int_num = int.Parse(textBox4.Text);
            if (textBox5.Text.Length != 0)
                xmlTest.clas.double_num = double.Parse(textBox5.Text.Replace('.', ','));

            if (pictureBox1.Image != null)
            {
                xmlTest.bmp = new Bitmap();
                xmlTest.bmp.SetBitmap(new System.Drawing.Bitmap(pictureBox1.Image));
            }
                


            WriteUsingXMLWriter(xmlTest);
        }

        private async void WriteUsingXMLWriter(XMLTest val)
        {
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = "XML(*.XML)|*.XML";
            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                settings.Async = true;
                using (XmlWriter writer = XmlWriter.Create(new StreamWriter(saveDialog.FileName), settings))
                {
                    await writer.WriteStartDocumentAsync();
                    await writer.WriteStartElementAsync(null, nameof(XMLTest), null);

                    await writer.WriteStartElementAsync(null, "str", null);
                    await writer.WriteAttributeStringAsync(null, "val", null, val.str);
                    await writer.WriteEndElementAsync();

                    await writer.WriteStartElementAsync(null, "intArray", null);
                    if (val.intArray != null)
                        await writer.WriteStringAsync(String.Join(' ', val.intArray));
                    await writer.WriteEndElementAsync();

                    await writer.WriteStartElementAsync(null, "sortedList", null);
                    if (val.sortedList != null)
                        await writer.WriteStringAsync(val.sortedList.ToString());
                    await writer.WriteEndElementAsync();

                    await writer.WriteStartElementAsync(null, nameof(Clas), null);
                    await writer.WriteAttributeStringAsync(null, "int_num", null, val.clas.int_num.ToString());
                    await writer.WriteAttributeStringAsync(null, "double_num", null, val.clas.double_num.ToString());
                    await writer.WriteEndElementAsync();



                    await writer.WriteStartElementAsync(null, "bitmap", null);
                    if (val.bmp != null)
                    {
                        using (var ms = new MemoryStream())
                        {
                            val.bmp.GetBitmap().Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                            byte[] arr = ms.ToArray();
                            await writer.WriteBase64Async(arr, 0, arr.Length);
                        }

                    }
                    await writer.WriteEndElementAsync();

                    await writer.WriteEndElementAsync();
                    await writer.FlushAsync();
                }
            }
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "XML(*.XML)|*.XML";
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                using (var reader = XmlReader.Create(new StreamReader(fileDialog.FileName)))
                {
                    reader.ReadToFollowing("str");
                    textBox1.Text = reader.GetAttribute("val");
                    reader.ReadToFollowing("intArray");
                    textBox2.Text = reader.ReadElementContentAsString();
                    reader.ReadToFollowing("sortedList");
                    textBox3.Text = reader.ReadElementContentAsString();
                    reader.ReadToFollowing("Clas");
                    textBox4.Text = reader.GetAttribute("int_num");
                    textBox5.Text = reader.GetAttribute("double_num");

                    reader.ReadToFollowing("bitmap");
                    String imgBase64String = reader.ReadElementContentAsString();
                    byte[] imageBytes = Convert.FromBase64String(imgBase64String);

                    using (MemoryStream ms1 = new MemoryStream(imageBytes))
                    {
                        if (ms1.Length != 0)
                        {
                            pictureBox1.Image = Image.FromStream(ms1);
                            if (pictureBox1.Image != null)
                                pictureBox1.Size = pictureBox1.Image.Size;
                        }
                    }
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            XMLTest xmlTest = new XMLTest();

            xmlTest.str = textBox1.Text;

            if (textBox2.Text.Length != 0)
            {
                String[] split = textBox2.Text.Split(' ');
                xmlTest.intArray = new int[split.Length];

                for (int i = 0; i < split.Length; i++)
                {
                    xmlTest.intArray[i] = int.Parse(split[i]);
                }
            }

            if (textBox3.Text.Length != 0)
            {
                String[] splitted = textBox3.Text.Split(' ');
                xmlTest.sortedList = new SortedList();
                for (int i = 0; i < splitted.Length; i++)
                {
                    xmlTest.sortedList.Add(int.Parse(splitted[i]));
                }
            }

            xmlTest.clas = new Clas();
            if (textBox4.Text.Length != 0)
                xmlTest.clas.int_num = int.Parse(textBox4.Text);
            if (textBox5.Text.Length != 0)
                xmlTest.clas.double_num = double.Parse(textBox5.Text.Replace('.', ','));


            if (pictureBox1.Image != null)
            {
                xmlTest.bmp = new Bitmap();
                xmlTest.bmp.SetBitmap(new System.Drawing.Bitmap(pictureBox1.Image));
            }
                

            Type[] extraTypes = new Type[2];
            extraTypes[0] = typeof(SortedList);
            extraTypes[1] = typeof(Bitmap);
            XmlSerializer serializer = new XmlSerializer(typeof(XMLTest), extraTypes);

            using (StreamWriter writer = new StreamWriter("qweqwesaasdadqws.xml"))
            {
                serializer.Serialize(writer, xmlTest);
            }
        }
    }
}

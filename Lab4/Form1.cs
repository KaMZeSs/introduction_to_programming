using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Lab4
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void папкаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length == 0 | textBox1.ForeColor == Color.Red)
            {
                MessageBox.Show("Невозможно считать число");
                return;
            }
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.Description = "Выберите папку";
            folderBrowserDialog.UseDescriptionForTitle = true;

            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                listBox1.Items.Clear();
                try 
                {
                    String[] files = Directory.GetFiles(folderBrowserDialog.SelectedPath, "*.*", SearchOption.AllDirectories).
                        Where(file => file.ToLower().EndsWith(".c") | file.ToLower().EndsWith(".cpp")).ToArray();

                    Dictionary<String, int> dict = new Dictionary<string, int>();
                    foreach (String file in files)
                    {
                        using (StreamReader reader = new StreamReader(file))
                        {
                            String text = reader.ReadToEnd();
                            Regex regex = new Regex(@"{|}");
                            Match[] matches = regex.Matches(text).ToArray();
                            Char[] letters = new char[matches.Length];
                            for (int i = 0; i < matches.Length; i++)
                            {
                                letters[i] = matches[i].ToString()[0];
                            }

                            if (Verification(letters) & CountNesting(letters) > Convert.ToInt32(textBox1.Text))
                            {
                                if (((DateTime.Now - File.GetCreationTime(file)).TotalDays / 365.2425) >= 2)
                                {
                                    listBox1.Items.Add($"{file} - удален");
                                    File.Delete(file);
                                }
                                else
                                    listBox1.Items.Add(file);
                            }
                        }
                    }

                    textBox1.Text = listBox1.Items.Count.ToString();

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Непредвиденная ошибка");
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private bool Verification(Char[] chars)
        {
            Stack<Char> stack = new Stack<Char>();

            for (int i = 0; i < chars.Length; i++)
            {
                if (chars[i] == '{')
                    stack.Push(chars[i]);
                else
                {
                    if (stack.Count == 0)
                        return false;
                    else
                        stack.Pop();
                }
            }
            if (stack.Count == 0)
                return true;
            else
                return false;
        }

        private int CountNesting(Char[] chars)
        {
            int max = 0;

            for (int i = 0, cur = 0; i < chars.Length; i++)
            {
                if (chars[i] == '{')
                {
                    cur++;
                    max = max < cur ? cur : max;
                }
                else if (chars[i] == '}')
                    cur--;

            }

            return max;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (Int32.TryParse(textBox1.Text, out int val))
            {
                textBox1.ForeColor = Color.Black;
            }
            else
            {
                textBox1.ForeColor = Color.Red;
            }
        }
    }
}

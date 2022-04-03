using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Lab3
{
    public partial class Form4 : Form
    {
        public Form4(Bitmap[] bmp, long[] mil)
        {
            InitializeComponent();

            for (int i = 0; i < bmp.Length; i++)
            {
                chart1.Series[0].Points.AddXY(bmp[i].Height * bmp[i].Width, mil[i]);
            }
            
            DataPoint[] points = chart1.Series[0].Points.ToArray();
            for (int i = 0; i < points.Length; i++)
            {
                points[i].Label = $"{mil[i]}\n{bmp[i].Width}x{bmp[i].Height}";
            }
            points = points.OrderBy(x => x.XValue).ToArray();

            chart1.Series[0].Points.Clear();
            
            foreach (DataPoint point in points)
                chart1.Series[0].Points.Add(point);
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageRandom256
{
    public partial class Imager : Form
    {
        public Bitmap canvasBitmap { get; set; }
        Graphics canvasGraphics;
        int canvasWidth = 1;
        int canvasHeight = 1;
        int[,] canvasDotArray;
        SolidBrush[] allColorArr = new SolidBrush[18] { new SolidBrush(Color.LightCoral), new SolidBrush(Color.Red), new SolidBrush(Color.LightGoldenrodYellow), new SolidBrush(Color.Yellow), new SolidBrush(Color.LightGreen), new SolidBrush(Color.Lime), new SolidBrush(Color.LightCyan), new SolidBrush(Color.Cyan), new SolidBrush(Color.LightBlue), new SolidBrush(Color.Blue), new SolidBrush(Color.LightPink), new SolidBrush(Color.Magenta), new SolidBrush(Color.DarkCyan), new SolidBrush(Color.Green), new SolidBrush(Color.DarkKhaki), new SolidBrush(Color.DarkMagenta), new SolidBrush(Color.Brown), new SolidBrush(Color.MediumBlue)};
        SolidBrush[] LightColorArr = new SolidBrush[12] { new SolidBrush(Color.LightCoral), new SolidBrush(Color.Red), new SolidBrush(Color.LightGoldenrodYellow), new SolidBrush(Color.Yellow), new SolidBrush(Color.LightGreen), new SolidBrush(Color.Lime), new SolidBrush(Color.LightCyan), new SolidBrush(Color.Cyan), new SolidBrush(Color.LightBlue), new SolidBrush(Color.Blue), new SolidBrush(Color.LightPink), new SolidBrush(Color.Magenta)};
        Random rnd = new Random();
        List<int[]> rollBack;
        int[] lastColors;
        int color;

        public Imager()
        {
            rollBack = new List<int[]>();
            InitializeComponent();
            InitilizeLastColors();
            loadCanvas();
            this.pictureBox1.MouseClick += new MouseEventHandler(this.pictureBox1_Click);
        }
        private void loadCanvas()
        {
            pictureBox1.Width = canvasWidth * (1080 / canvasWidth);
            pictureBox1.Height = canvasHeight * (1080 / canvasHeight);
            canvasBitmap = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            canvasGraphics = Graphics.FromImage(canvasBitmap);
            canvasGraphics.FillRectangle(Brushes.LightGray, 0, 0, canvasBitmap.Width, canvasBitmap.Height);
            pictureBox1.Image = canvasBitmap;
            canvasDotArray = new int[canvasWidth, canvasHeight];
        }
        private void LoopDraw(Brush[] colorArr)
        {
            canvasGraphics = Graphics.FromImage(canvasBitmap);
            for (int i = 0; i < canvasWidth; i++)
            {
                for (int j = 0; j < canvasHeight; j++)
                {
                    Draw(i, j, colorArr);
                }
            }
            pictureBox1.Image = canvasBitmap;
        }
        private void Draw(int i, int j, Brush[] colorArr)
        {
            if (j != 0)
            {
                while (color == lastColors[j - 1] || color == lastColors[j])
                {
                    color = rnd.Next(colorArr.Count());
                }
            }
            else
            {
                while (color == lastColors[j])
                {
                    color = rnd.Next(colorArr.Count());
                }
            }
            canvasGraphics.FillRectangle(colorArr[color], j * (1080 / canvasWidth), i * (1080 / canvasWidth), (1080 / canvasWidth), (1080 / canvasWidth));
            lastColors[j] = color;
        }
        private void pictureBox1_Click(object sender, MouseEventArgs e)
        {
            float x, y;
            if (e.Button == MouseButtons.Right)
            {
                x = (e.X) / (1080 / canvasWidth);
                x = (float)Math.Floor(x);
                y = (e.Y) / (1080 / canvasWidth);
                y = (float)Math.Floor(y);
                if (canvasBitmap.GetPixel((int)(x) * (1080 / canvasWidth), (int)(y) * (1080 / canvasWidth)).Name == "ff000000")
                {
                    return;
                }
                rollBack.Add(new int[3] { (int)x, (int)y, FindColorInArr(canvasBitmap.GetPixel((int)(x) * (1080 / canvasWidth), (int)(y) * (1080 / canvasWidth)))});
                ChangeColor(x, y, Brushes.Black);
            }
        }
        private int FindColorInArr(Color canvasColor)
        {
            int a;
            for(a = 0; a < allColorArr.Count(); a++)
            {
                if (allColorArr[a].Color.ToArgb() == canvasColor.ToArgb())
                {
                    return a;
                }
            }
            return a;
        }
        private void InitilizeLastColors()
        {
            lastColors = new int[canvasHeight];
            for(int p = 0; p < canvasHeight; p++)
            {
                lastColors[p] = 0;
            }
        }
        private void ChangeColor(float x,float y, Brush color)
        {
            canvasGraphics = Graphics.FromImage(canvasBitmap);
            canvasGraphics.FillRectangle(color, x * (1080 / canvasWidth), y * (1080 / canvasWidth), (1080 / canvasWidth), (1080 / canvasWidth));
            pictureBox1.Image = canvasBitmap;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            color = 0;
            rollBack = new List<int[]>();
            canvasWidth = ((int)numericUpDown1.Value);
            canvasHeight = ((int)numericUpDown2.Value);
            InitilizeLastColors();
            loadCanvas();
            LoopDraw(allColorArr);
        }
        private void button3_Click(object sender, EventArgs e)
        {
            color = 0;
            rollBack = new List<int[]>();
            canvasWidth = ((int)numericUpDown1.Value);
            canvasHeight = ((int)numericUpDown2.Value);
            InitilizeLastColors();
            loadCanvas();
            LoopDraw(LightColorArr);
        }
        public void button4_Click(object sender, EventArgs e)
        {
            pictureBox1.Image.Save(textBox2.Text+ @"\"+textBox1.Text+".png",System.Drawing.Imaging.ImageFormat.Png);
        }
        private void button2_Click(object sender, EventArgs e)
        {
            if (rollBack.Count > 0)
            {
                int[] data = rollBack[rollBack.Count - 1];
                ChangeColor(data[0], data[1], allColorArr[data[2]]);
                rollBack.RemoveAt(rollBack.Count - 1);
            }
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {

        }
    }
}

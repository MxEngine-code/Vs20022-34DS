using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;

namespace Bloco_de_notas
{
    public partial class Form1: Form
    {
        bool arrastar = false;
        int i = 0;
        public Form1()
        {
            InitializeComponent();
        }

        private void ArredondarBordas(int raio)
        {
            GraphicsPath path = new GraphicsPath();
            int diametro = raio * 2;

            Rectangle rect = new Rectangle(0, 0, this.Width, this.Height);

            path.AddArc(rect.X, rect.Y, diametro, diametro, 180, 90);
            path.AddArc(rect.Right - diametro, rect.Y, diametro, diametro, 270, 90);
            path.AddArc(rect.Right - diametro, rect.Bottom - diametro, diametro, diametro, 0, 90);
            path.AddArc(rect.X, rect.Bottom - diametro, diametro, diametro, 90, 90);
            path.CloseFigure();

            this.Region = new Region(path);
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            ArredondarBordas(35);
            label1.Text = "";
            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (label1.Text == "Notepad Code" || label1.Text.Length >= "Notepad Code".Length)
            {
                timer1.Stop();
                Bloco jn = new Bloco();
                jn.Show();
            } else
            {
                label1.Text = "Notepad Code".Substring(0, i);
                i++;
            }
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            using (LinearGradientBrush brush = new LinearGradientBrush(this.ClientRectangle, Color.Gray, Color.DarkGray, LinearGradientMode.Vertical))
            {
                g.FillRectangle(brush, this.ClientRectangle);
            }
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) arrastar = true;
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (arrastar)
            {
                this.Location = new Point(this.Left + e.X - this.Width / 2, this.Top + e.Y - this.Height / 2);
            }
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            arrastar = false;
        }
    }
}

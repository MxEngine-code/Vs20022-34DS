using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Bloco_de_notas
{
    public partial class Bloco: Form
    {
        string caminho = "";
        string nomef = "";
        bool arrastando = false;
        public Bloco()
        {
            InitializeComponent();
        }

        private void fonteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fontDialog1.Font = richTextBox1.Font;
            if (fontDialog1.ShowDialog() == DialogResult.OK) richTextBox1.Font = fontDialog1.Font;
        }

        private void xToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void minimizarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void maximizarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (maximizarToolStripMenuItem.Text == "Maximizar") {
                maximizarToolStripMenuItem.Text = "Redefinir";
                this.WindowState = FormWindowState.Maximized;
            } else
            {
                maximizarToolStripMenuItem.Text = "Maximizar";
                this.WindowState = FormWindowState.Normal;
            }
        }

        private void menuStrip1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                arrastando = true;
                menuStrip1.Capture = true;
            }
        }

        private void menuStrip1_MouseUp(object sender, MouseEventArgs e)
        {
            arrastando = false;
            menuStrip1.Capture = false;
        }

        private void menuStrip1_MouseMove(object sender, MouseEventArgs e)
        {
            if (arrastando)
            {
                this.Location = new Point(this.Left + e.X - menuStrip1.Width / 2, this.Top + e.Y - menuStrip1.Height / 2);
            }
        }

        private void salvarToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            if (File.Exists(caminho))
            {
                File.WriteAllText(caminho, richTextBox1.Text);
                notepadCodeToolStripMenuItem.Text = $"Notepad Code {nomef}";
            }
            else
            {
                SaveFileDialog salvar = new SaveFileDialog();
                salvar.Title = "Salvar";
                salvar.DefaultExt = ".txt";
                if (Directory.Exists(caminho)) salvar.InitialDirectory = caminho;
                if (salvar.ShowDialog() == DialogResult.OK)
                {
                    nomef = Path.GetFileName(salvar.FileName);
                    notepadCodeToolStripMenuItem.Text = $"Notepad Code {nomef}";
                }
            }

        }

        private void Bloco_Load(object sender, EventArgs e)
        {
            toolStripComboBox1.SelectedIndex = 0;
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            if (toolStripComboBox1.SelectedIndex == 1) richTextBox1.ScrollToCaret();
            notepadCodeToolStripMenuItem.Text = $"Notepad Code {nomef}*";
        }

        private void abrirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog abrir = new OpenFileDialog();
            if (Directory.Exists(caminho)) abrir.InitialDirectory = caminho;
            abrir.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
           if (abrir.ShowDialog() == DialogResult.OK)
            {
                abrir.Title = "Abrir";
                caminho = abrir.FileName;
                nomef = Path.GetFileName(abrir.FileName);
                notepadCodeToolStripMenuItem.Text = $"Notepad Code {nomef}";
                richTextBox1.Text = File.ReadAllText(abrir.FileName);
            }
        }

        private void salvarComoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog salvar = new SaveFileDialog();
            salvar.Title = "Salvar";
            salvar.DefaultExt = ".txt";
            if (Directory.Exists(caminho)) salvar.InitialDirectory = caminho;
            if (salvar.ShowDialog() == DialogResult.OK)
            {
                nomef = Path.GetFileName(salvar.FileName);
                notepadCodeToolStripMenuItem.Text = $"Notepad Code {nomef}";
            }
        }
    }
}

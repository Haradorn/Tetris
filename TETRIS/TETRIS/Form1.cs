using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Media;

namespace TETRIS
{
    public partial class Form1 : Form
    {
        int MAXX = 10, MAXY = 15, ps = 24;
        bool started = false;
        Brush[] brsh = {Brushes.White, Brushes.Orange, Brushes.Blue, Brushes.Red, Brushes.Green, Brushes.Honeydew, Brushes.Violet,
                       Brushes.Tomato, Brushes.SteelBlue, Brushes.PapayaWhip};
        int score;
        int[,] arr;
        Figure figure;
        Graphics grph;
        SoundPlayer simplesound = new SoundPlayer("chimes.wav"),
        simplesoundEnd = new SoundPlayer("fart.wav"),
        simplesoundStart = new SoundPlayer("start.wav");

        public Form1()
        {
            InitializeComponent();
        }
        private void upd_score()
        {
            label1.Text = score.ToString();
        }
        private void draw(int x, int y, int clr)
        {
            if (clr == 0 || x < 0 || x >= MAXX || y >= MAXY) return;
            grph.FillEllipse(brsh[clr], new Rectangle(2 + x * ps, 2 + y * ps, ps, ps));
        }
        private void draw_field()
        {
            int i, j;
            for (i = 0; i < MAXX; ++i)
                for (j = 0; j < MAXY; ++j)
                    draw(i, j, arr[i, j]);
        }
        private void draw_figure(Figure f)
        {
            int i, j;
            for (i = 0; i < 4; ++i)
                for (j = 0; j < 4; ++j)
                    draw(f.x + i, f.y + j, f.pix[i, j]);
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            if(figure == null)
            {
                figure = new Figure();
                if(!figure_ok())
                {
                    timer1.Enabled = false;
                    simplesoundEnd.Play();
                    MessageBox.Show("Game Over!");
                }
            }
            figure_down();
            panel1.Refresh();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            started = true;
            score = 0;
            if (checkBox1.Checked == true)
            {
                MAXY = 25;
                panel1.Height *= MAXY;
                this.Height = 700;
            }
            else
            {
                MAXY = 15;
                this.Height = 470;
            }
            arr = new int[MAXX, MAXY];
            upd_score();
            figure = null;
            timer1.Enabled = true;
            if (radioButton1.Checked == true) timer1.Interval = 400;
            if (radioButton2.Checked == true) timer1.Interval = 100;
            simplesoundStart.Play();
        }
        private int get_arr(int x, int y)
        {
            if (x < 0 || x >= MAXX || y >= MAXY) return 1;
            if (y < 0) return 0;
            return arr[x, y];
        }
        private void set_arr(int x, int y, int clr)
        {
            if (x < 0 || x >= MAXX || y < 0 || y >= MAXY) return;
            arr[x, y] = clr;
        }
        private bool figure_ok()
        {
            int i, j;
            for(i = 0; i < 4; ++i)
                for(j = 0; j < 4; ++j)
                {
                    if (figure.pix[i, j] != 0 && get_arr(figure.x + i, figure.y + j) != 0)
                        return false;
                }
            return true;
        }
        private void apply_figure()
        {
            int i, j;
            for (i = 0; i < 4; ++i)
                for (j = 0; j < 4; ++j)
                    if (figure.pix[i, j] != 0) set_arr(figure.x + i, figure.y + j, figure.pix[i, j]);
            bool[] lines = new bool[MAXY];
            int goo = 0;
            for(i = 0; i < MAXY; ++i)
            {
                bool qq = true;
                for(j = 0; j < MAXX; ++j)
                {
                    if(arr[j, i] == 0)
                    {
                        qq = false;
                        break;
                    }
                }
                lines[i] = qq;
                if (qq) ++goo;
            }
            if(goo != 0)
            {
                int sc = (goo * (goo + 1) / 2) * 100;
                score += sc;
                upd_score();
                simplesound.Play();
            }
            int[,] nar = new int[MAXX, MAXY];
            int ty = MAXY;
            for(i = MAXY - 1; i >= 0; --i)
            {
                if(lines[i]) continue;
                --ty;
                for(j = 0; j < MAXX; ++j)
                {
                    nar[j, ty] = arr[j, i];
                }
            }
            arr = nar;
        }
        private void figure_down()
        {
            ++figure.y;
            if(!figure_ok())
            {
                --figure.y;
                apply_figure();
                figure = null;
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            if (!started) return;
            grph = e.Graphics;
            grph.DrawRectangle(Pens.LightBlue, new Rectangle(0, 0, ps * MAXX + 4, ps * MAXY + 4));
            grph.FillRectangle(Brushes.DodgerBlue, new Rectangle(1, 1, ps * MAXX + 3, ps * MAXY + 3));
            draw_field();
            if (figure != null) draw_figure(figure);
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (figure == null) return;
            if (e.KeyCode == Keys.A)
            {
                figure.x -= 1;
                if (!figure_ok()) figure.x += 1;
            }
            else if (e.KeyCode == Keys.D)
            {
                figure.x += 1;
                if (!figure_ok()) figure.x -= 1;
            }
            else if (e.KeyCode == Keys.S)
            {
                figure.y += 1;
                if (!figure_ok()) figure.y -= 1;
            }
            else if (e.KeyCode == Keys.W)
            {
                figure.rotate();
                if(!figure_ok())
                {
                    for (int i = 0; i < 3; ++i) figure.rotate();
                }
            }
            panel1.Refresh();
        }

        private void button1_KeyDown(object sender, KeyEventArgs e)
        {
            Form1_KeyDown(sender, e);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            panel1.enableDB();
        }
    }
        public class Figure
        {
            string[] str = {"....1111........", "....111.1.......", ".....11..11.....", ".1...11...1.....",
                           "....111..1......", "....111...1.....", "..1..11..1......"};
            public int x, y;
            public int clr;
            public int[,] pix;
            public Random rnd;
            public Figure()
            {
                x = 3; y -= 2;
                rnd = new Random();
                clr = 1 + rnd.Next(9);
                int n = rnd.Next(str.Length);
                fillpix(out pix, str[n]);
                n = rnd.Next(4);
                for(int i = 0; i < n; ++i) 
                    rotate();
            }
                void fillpix(out int[,] pix, string cc)
                {
                    pix = new int[4, 4];
                    int i, j;
                    for (i = 0; i < 4; ++i)
                        for (j = 0; j < 4; ++j)
                        {
                            if (cc[j * 4 + i] == '1') pix[i, j] = clr;
                            else pix[i, j] = 0;
                        }       
                }
                public void rotate()
                {
                    int[,] p2 = new int[4, 4];
                    int i, j;
                    for(i = 0; i < 4; ++i)
                        for(j = 0; j < 4; ++j)
                        {
                            p2[3 - j, i] = pix[i, j];
                        }
                    pix = p2;    
                }
        }
}

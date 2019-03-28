using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Excel;
using _Excel = Microsoft.Office.Interop.Excel;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra.Factorization;
using System.Windows.Forms;
using System.IO;
namespace raytracer
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
            //textBox2.ScrollBars

        }
        private void label1_Click(object sender, EventArgs e)
        {

        }
        private void folderBrowserDialog1_HelpRequest(object sender, EventArgs e)
        {
            //var formatProvider = (CultureInfo)CultureInfo.InvariantCulture.Clone();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            OpenFileDialog dlg = new OpenFileDialog();
            if (textBox1.Text != "编辑")
                dlg.InitialDirectory = textBox1.Text;
            
            dlg.Filter = "文本文件|*.*|表格文件|*.xlsx";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = dlg.FileName;
                Excel excel = new Excel(dlg.FileName, 1);
                //MessageBox.Show(dlg.FileName);
                //MessageBox.Show(excel.cor(5).ToString());
                double ii = excel.cor(1, 1) + excel.cor(2, 2) + excel.cor(3, 3) - excel.cor(5, 5);
                //MessageBox.Show(ii.ToString());

                // MessageBox.Show(excel.readcell(0, 0).ToString()); 
                //textBox2.Text += excel.readcell(0,0) + Environment.NewLine;
                //textBox2.SelectionStart = textBox2.TextAlign;
                //excel.readexcel( buff, 3, 48, ref excel);
                for (int i = 0; i < 5; i++)
                {
                    var x = excel.solve(i);

                    textBox2.Text += x.ToString();
                }
                var n = excel.solve1();
                textBox3.Text = n.ToString();
            }
           ;
        }
        class Excel
        {
            double[,] all=new double[49,10];
            double[,] buffer = new double[6, 5];
            string filepath;
            _Application excel = new _Excel.Application();
            Workbook WB;
            Worksheet ws;
            int row = 48;
            public Excel(string path, int sheet)
            {
                this.filepath = path;
                WB = excel.Workbooks.Open(path);
                ws = WB.Worksheets[sheet];
            }
            public double readcell(int row, int col)
            {

                if (ws.Cells[row, col].Value2 != null)
                {
                    double a = Convert.ToDouble(ws.Cells[row, col].Value2);
                    return a;
                }
                else
                {
                    return 0;
                }

            }
            public void readall(int row, int col)
            {
                for (int i = 1; i < row + 1; i++)
                    for (int j = 1; j < col + 1; j++)
                    {
                        if (ws.Cells[i, j].Value2 != null)
                            all[i, j] = Convert.ToDouble(ws.Cells[i, j].Value2);
                    }
            }
            public double[] readcol(int coll, int row = 48)//读取以整列，默认行数48
            {
                double[] a = new double[row + 1];
                for (int i = 1; i < row + 1; i++)
                {
                    if (ws.Cells[i, coll].Value2 != null)
                        a[i] = Convert.ToDouble(ws.Cells[i, coll].Value2);
                }
                return a;
            }
            public double[] readrow(int row)
            {
                double[] a = new double[4];
                for (int i = 1; i < 4; i++)
                {
                    if (ws.Cells[row, i].Value2 != null)
                    {
                        a[i] = Convert.ToDouble(ws.Cells[row, i].Value2);
                    }
                }
                return a;
            }
            public double cor(int i, int j)
            {
                return math.multi(this.readcol(i), this.readcol(j));
            }
            public double cor(int i, int j, int k)
            {
                return math.multi(this.readcol(i), this.readcol(j), this.readcol(k));
            }
            public double cor(int i)
            {
                return math.sum(this.readcol(i));
            }
            public double[] Calculation(int i, int j)//j+1
            {
                double[] vs = new double[4];
                vs[1] = buffer[j+1, 1];
                vs[2] = buffer[j+1, 2];
                vs[3] = buffer[j+1, 3];
                return math.delta(readrow(i), vs);
            }
            public double[,] biuldA()//3 shuzu  +1  +2
            {
                var A = new double[48 * 5, 48 * 3 + 5 * 3];
                var buff = new double[4];
                for (int i = 0; i < 5; i++)
                    for (int j = 0; j < 48; j++)
                    {
                        buff = Calculation(j + 1, i);
                        A[i*48+j, j * 3] = Calculation(j + 1, i)[1];
                        A[i*48+j, j * 3 + 1] = Calculation(j + 1, i)[2];
                        A[i*48+j, j * 3 + 2] = Calculation(j + 1, i)[3];
                        A[i*48+j, 48 * 3 + i * 3] = Calculation(j + 1, i)[1];
                        A[i*48+j,( (i + 48) * 3)+1] = Calculation(j + 1, i)[2];
                        A[i*48+j, ((i + 48) * 3)+2] = Calculation(j + 1, i)[3];
                    }
                return A;
            }
            public double[] biuldB()//jisuan
            {
                readall(48, 9);
                var b = new double[48 * 5];
                for(int j=0;j<5;j++)
                for (int i = 0; i < 48 ; i++)
                {
                    double L=calL(i+1,j+1);
                    b[i+j*48] =  buffer[j+1, 4] + all[i + 1, j+5] - L;
                }
                return b;
            }
            public double calL(int i, int j)
            {
                return Math.Sqrt((all[i, 1] - buffer[j, 1]) * (all[i, 1] - buffer[j, 1]) + (all[i, 2] - buffer[j, 2]) * (all[i, 2] - buffer[j, 2]) + (all[i, 3] - buffer[j, 3]) * (all[i, 3] - buffer[j, 3]));
            }
            public Vector<double> solve(int j)
            {

                var A = Matrix<double>.Build.DenseOfArray(new double[,] {//A的方程矩阵
           { 2*this.cor(1,1),2*this.cor(1,2), 2*this.cor(1,3),2*this.cor(1,5+j),-1*this.cor(1) },
           { 2*this.cor(1,2),2*this.cor(2,2), 2*this.cor(2,3),2*this.cor(2,5+j),-1*this.cor(2) },
           { 2*this.cor(1,3),2*this.cor(2,3), 2*this.cor(3,3),2*this.cor(3,5+j),-1*this.cor(3) },
           { 2*this.cor(1,5+j),2*this.cor(2,5+j), 2*this.cor(3,5+j),2*this.cor(5+j,5+j),-1*this.cor(5+j) },
           { -1*this.cor(1),-1*this.cor(2), -1*this.cor(3),-1*this.cor(5+j),this.row/2 },
            });
                //MessageBox.Show(A.ToString());
                //double value5 =-15000000;
                double test = this.cor(1, 1) + this.cor(2, 2) + this.cor(3, 3) - this.cor(5 + j, 5 + j);
                test = -0.5 * test;
                //MessageBox.Show(test.ToString());
                var b = Vector<double>.Build.Dense(new double[] {
              (this.cor(1,1,1)+this.cor(1,2,2)+this.cor(1,3,3)-this.cor(1,5+j,5+j)),
              (this.cor(2,1,1)+this.cor(2,2,2)+this.cor(2,3,3)-this.cor(2,5+j,5+j)),
              (this.cor(3,1,1)+this.cor(3,2,2)+this.cor(3,3,3)-this.cor(3,5+j,5+j)),
              (this.cor(5+j,1,1)+this.cor(5+j,2,2)+this.cor(5+j,3,3)-this.cor(5+j,5+j,5+j)),
              test,
            });


               // MessageBox.Show(b.ToString());
                var x = A.Solve(b);
                for (int i = 1; i < 5; i++)//j是从零开始的
                { buffer[j + 1, i] = x[i-1]; }
                return x;
            }
            public Vector<double> solve1()
            {

                var A = Matrix<double>.Build.DenseOfArray(this.biuldA());
                var b = Vector<double>.Build.Dense(this.biuldB());
                var x = A.Solve(b);
                return x;

            }
            class math //算术类
            {
                static public double multi(double[] m, double[] n, int row = 48)//两整列相乘并相加
                {
                    double sum = 0;
                    for (int i = 1; i < row + 1; i++)
                    { sum += m[i] * n[i]; }
                    return sum;
                }
                static public double multi(double[] m, double[] n, double[] k, int row = 48)//三整列相乘并相加
                {
                    double sum = 0;
                    for (int i = 1; i < row + 1; i++)
                    { sum += m[i] * n[i] * k[i]; }
                    return sum;
                }
                static public double sum(double[] x, int row = 48)//一整列求和
                {
                    double sum = 0;
                    for (int i = 1; i < row + 1; i++)
                    { sum += x[i]; }
                    return sum;
                }
                static public double[] delta(double[] a, double[] b)
                {//a 是小xyz
                    double[] Delta = new double[4];//1 _>i////////////////////
                    for (int i = 1; i < 4; i++)
                    {
                        Delta[i] = (a[i] - b[i]) / Math.Sqrt((a[1] - b[1]) * (a[1] - b[1]) + (a[2] - b[2]) * (a[2] - b[2]) + (a[3] - b[3]) * (a[3] - b[3]));
                    }
                    return Delta;
                }
            }
        }

    }
}

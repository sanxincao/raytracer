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
        Excel excel;//由于全局只使用一张表所以只定义一个excel变量
        Vector<double> n;//第二个方程的解用于可视化
        public Form1()
        {
            InitializeComponent();//初始化窗口
            //textBox2.ScrollBars

        }
        private void label1_Click(object sender, EventArgs e)
        {

        }
        private void folderBrowserDialog1_HelpRequest(object sender, EventArgs e)
        {
            //var formatProvider = (CultureInfo)CultureInfo.InvariantCulture.Clone();
        }

        private void button1_Click(object sender, EventArgs e)//打开文件摁钮
        {

            OpenFileDialog dlg = new OpenFileDialog();
            if (textBox1.Text != "编辑")
                dlg.InitialDirectory = textBox1.Text;
            
            dlg.Filter = "文本文件|*.*|表格文件|*.xlsx";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = dlg.FileName;//路径变量
                excel = new Excel(dlg.FileName, 1);
                //MessageBox.Show(dlg.FileName);
            
                for (int i = 0; i < 5; i++)
                {
                    var x = excel.solve(i);//解第一个方程
                    textBox2.Text += "第" + (i + 1).ToString() + "组解：  " + " X:  " + x[0].ToString() + " Y:  " + x[1].ToString() + " Z:  " + x[2].ToString() + " d:  " + x[3].ToString() + Environment.NewLine;
                    //输出
                }
                
            }
           ;
        }
 
        private void button2_Click(object sender, EventArgs e)//修正值摁钮
        {
            n = excel.solve1();//解第二个方程
            for(int i=0;i<48;i++)
            { textBox3.Text += "第" + (i + 1).ToString() + "组解:  " + " dx:  " + n[i * 3].ToString() + " dy:  " + n[i * 3 + 1].ToString() + " dz:  " + n[i * 3 + 2].ToString()+Environment.NewLine; }
        }

        private void button3_Click(object sender, EventArgs e)//画图摁钮
        {
 
                int i = Convert.ToInt32(textBox4.Text);
                this.chart1.Series["delta"].Points.AddXY("dx[" + i.ToString() + "]", n[i*3]);
                this.chart1.Series["delta"].Points.AddXY("dy[" + i.ToString() + "]", n[i*3+1]);
                this.chart1.Series["delta"].Points.AddXY("dz[" + i.ToString() + "]", n[i*3+2]);
            
        }
        class Excel
        {
            double[,] all=new double[49,10];//用来放excel表里数据，从一开始
            double[,] buffer = new double[6, 5];//用来放第一组解。从一开始
            //下面都是用来初始化的构造函数，不用管
            string filepath;
            _Application excel = new _Excel.Application();
            Workbook WB;
            Worksheet ws;
            int row = 48;
            public Excel()
            {

            }
            public Excel(string path, int sheet)
            {
                this.filepath = path;
                WB = excel.Workbooks.Open(path);
                ws = WB.Worksheets[sheet];
            }       
            public void readall(int row, int col)//读取表内元素放在all数组里
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
            public double[] readrow(int row)//读取每一行前三个元素并返回
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
            public double cor(int i, int j)//计算两列的乘积
            {
                return math.multi(this.readcol(i), this.readcol(j));
            }
            public double cor(int i, int j, int k)//计算三列的乘积
            {
                return math.multi(this.readcol(i), this.readcol(j), this.readcol(k));
            }
            public double cor(int i)//计算某一列的和
            {
                return math.sum(this.readcol(i));
            }
            public double[] Calculation(int i, int j)//计算矩阵里的偏导数
            {
                double[] vs = new double[4];
                vs[1] = buffer[j+1, 1];
                vs[2] = buffer[j+1, 2];
                vs[3] = buffer[j+1, 3];
                return math.delta(readrow(i), vs);
            }
            //由于第二个方程的A,B方程太大，额外使用函数建立。
            public double[,] biuldA()
            {
                var A = new double[48 * 5, 48 * 3 + 5 * 3];//用来放A的元素
                var buff = new double[4];
                for (int i = 0; i < 5; i++)
                    for (int j = 0; j < 48; j++)
                    {
                        //我发现
                        buff = Calculation(j + 1, i);
                        A[i*48+j, j * 3] = buff[1];
                        A[i*48+j, j * 3 + 1] = buff[2];
                        A[i*48+j, j * 3 + 2] = buff[3];
                        A[i*48+j, 48 * 3 + i * 3] =buff[1];
                        A[i*48+j,( (i + 48) * 3)+1] = buff[2];
                        A[i*48+j, ((i + 48) * 3)+2] = buff[3];
                    }
                return A;
            }
            public double[] biuldB()
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
                {
                    double[] Delta = new double[4];
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

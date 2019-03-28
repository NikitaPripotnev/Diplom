using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ILOG.Concert;
using ILOG.CPLEX;

namespace osp
{
    public partial class ospmain : Form
    {
        public ospmain()
        {
            InitializeComponent();
            openFileDialog1.Filter = "TSPLIB Files|*.tsp;*.vrp";
            comboBox1.SelectedIndex = 0;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox3.Text == "")
            {
                MessageBox.Show("Не задано количество коммивояжеров");
                return;
            }

            if(textBox2.Text == "" && comboBox1.SelectedIndex == 1)
            {
                MessageBox.Show("Не задано имя файла");
                return;
            }
                
            int dim = 0;
            int [][] dist;

            int kts = Convert.ToInt16(textBox3.Text);

            if(comboBox1.SelectedIndex == 1)
            {
                TSPLib TSPDoc = new TSPLib(textBox2.Text);
                TSPDoc.ReadProblem();
                dim = TSPDoc.Dimension;
                dist = new int [dim][];
                for (int i = 0; i < dim; i++)
                    dist[i] = new int[dim];
                for(int i = 0; i < dim; i++)
                    for (int j = 0; j < dim; j++)
                        dist[i][j] = (int)TSPDoc.Nodes.CostMatrix[i][j];
            }
            else
            {
                if (textBox4.Text == "" && comboBox1.SelectedIndex == 0)
                {
                    MessageBox.Show("Не задана размерность");
                    return;
                }
                //*****************************************
                //Создание рандомных точек

           
                dim = Convert.ToInt16(textBox4.Text);
                dist = new int [dim][];
                for(int i = 0; i < dim; i++)
                    dist[i] = new int[dim];
                Random p = new Random();
                for (int i = 0; i < dim; i++)
                    for (int j = 0; j < dim; j++)
                        if (i == j) dist[i][j] = 0;
                        else dist[i][j] = p.Next(100);
                SaveToFile(dim, dist);
            }

            for (int i = 0; i < dim; i++)
                dist[i][i] = 100000;

            textBox1.Text = DateTime.Now.ToString() + Environment.NewLine;

            List<int> DecompList = new List<int>();
            List<int> WorkList = new List<int>();
                
            double cost = 0;
            int num_clust = 0;
            if (checkBox1.Checked)
            {
                int j = kts;
                int decomp_count = 0;
                while (j >= 2)
                {
                    j /= 2;
                    decomp_count++;
                }
                for (int m = 0; m < decomp_count; m++)
                    if (m == 0) decomp(dim, 2, dist, WorkList);
                    else
                        for (j = 0; j < Math.Pow(2, m); j++)
                        {
                            int k1 = 0;
                            while (k1 < WorkList.Count)
                            {
                                if (WorkList[k1] == -1)
                                {
                                    int[][] dist1 = new int[k1][];
                                    for (int i = 0; i < k1; i++)
                                        dist1[i] = new int[k1];
                                    for (int i = 0; i < k1; i++)
                                        for (int j1 = 0; j1 < k1; j1++)
                                            dist1[i][j1] = dist[WorkList[i]][WorkList[j1]];
                                    decomp(k1, 2, dist1, DecompList);
                                    for (int i = 0; i < DecompList.Count; i++)
                                        if (DecompList[i] != -1) DecompList[i] = WorkList[DecompList[i]]; //Insert(0,WorkList[DecompList[i]
                                    WorkList.RemoveRange(0, k1 + 1);
                                    WorkList.AddRange(DecompList);
                                    break;
                                }
                                else k1++;
                            }
                        }
            }
            else decomp(dim, kts, dist, WorkList);
            int k = 0;
            int dim1;
            int nach = 0;
            while(k < WorkList.Count)
            {
                if(WorkList[k] == -1)
                {
                    num_clust++;
                    dim1 = k - nach;
                    int[][] dist1 = new int[dim1][];
                    for (int i = 0; i < dim1; i++)
                        dist1[i] = new int[dim1];
                    for (int i = nach; i < k; i++)
                        for (int j = nach; j < k; j++)
                            dist1[i - nach][j - nach] = dist[WorkList[i]][WorkList[j]];
                    int[] path = new int[dim1 + 1];
                    double cost1 = one_tsp(dim1, dist1, path);

                    textBox1.Text += "Коммивояжер " + num_clust.ToString() + Environment.NewLine;
                    for (int i = 0; i <= dim1; i++)
                    {
                        textBox1.Text += (WorkList[path[i] + nach] + 1).ToString();
                        if (i < dim1) textBox1.Text += " - ";
                        else textBox1.Text += Environment.NewLine;
                    }
                    textBox1.Text += "ЦФ" + num_clust.ToString() + " = " + cost1.ToString() + Environment.NewLine;
                    if (cost1 > cost) cost = cost1;
                    nach = k + 1;
                }
                k++;
            }
            if (kts - num_clust > 0) textBox1.Text += "Исключено пустых кластеров: " + (kts - num_clust).ToString() + Environment.NewLine;
            textBox1.Text += "ЦФ = " + cost.ToString() + Environment.NewLine;
            textBox1.Text += DateTime.Now.ToString() + Environment.NewLine;
        }
 
        private void decomp(int dim, int kts, int[][] dist, List<int> WorkList)
        {
            int[] lo = new int[dim];
            int dk = dim * kts;

            for (int i = 0; i < dim; i++)
                lo[i] = 1;

            Cplex cplex = new Cplex();
            NumVarType varType = NumVarType.Bool;
            INumVar[][] x = new INumVar[dim][];
            for (int i = 0; i < dim; i++)
                x[i] = cplex.NumVarArray(dk, 0, 1);

            for (int i = 0; i < dim; i++)
                for (int j = 0; j < dk; j++)
                    x[i][j] = cplex.BoolVar();

            //****************************************
            //Что тут происходит?
            for (int j = 0; j < dim; j++)
            {
                INumExpr xcolSum = cplex.NumExpr();
                for (int k = 0; k < kts; k++)
                    for (int i = 0; i < dim; i++)
                        xcolSum = cplex.Sum(xcolSum, x[i][k * dim + j]);
                cplex.AddEq(lo[j], xcolSum);
            }

            varType = NumVarType.Float;
            INumVar[] u = new INumVar[dk];
            for (int j = 0; j < dk; j++)
                u[j] = cplex.NumVar(0, 100000);

            for (int j = 1; j < dk; j++)
                cplex.AddGe(u[j], 0);

            for (int k = 0; k < kts; k++)
                for (int i = 1; i < dim; i++)
                    for (int j = 1; j < dim; j++)
                        if (i != j)
                            if (kts == 1) cplex.AddLe(cplex.Sum(cplex.Diff(u[k * dim + i], u[k * dim + j]), cplex.Prod(dim, x[i][k * dim + j])), dim - 1);
                            else cplex.AddLe(cplex.Sum(cplex.Diff(u[k * dim + i], u[k * dim + j]), cplex.Prod(dim, x[i][k * dim + j])), dim);

            for (int i = 0; i < dim; i++)
            {
                INumExpr xrowSum = cplex.NumExpr();
                for (int j = 0; j < dk; j++)
                    xrowSum = cplex.Sum(xrowSum, x[i][j]);
                cplex.AddEq(lo[i], xrowSum);
            }

            //Условия независимости кластеров
            if (kts > 1)
            {
                int[] a = new int[kts + 1];
                for (int k = 1; k < kts; k++)
                {
                    if (k > 1 && k < kts - 1) continue;
                    int p;
                    for (int i = 1; i <= k; i++)
                        a[i] = i;
                    p = k;
                    while (p >= 1)
                    {
                        for (int m = 0; m < dim; m++)
                        {
                            INumExpr xcolrowSum = cplex.NumExpr();
                            for (int j = 1; j <= kts; j++)
                            {
                                bool row = false;
                                for (int i = 1; i <= k; i++)
                                    if (a[i] == j) row = true;
                                if (row)
                                    for (int t = 0; t < dim; t++)
                                        xcolrowSum = cplex.Sum(xcolrowSum, x[m][(j - 1) * dim + t]);
                                else
                                    for (int t = 0; t < dim; t++)
                                        xcolrowSum = cplex.Sum(xcolrowSum, x[t][(j - 1) * dim + m]);
                            }
                            cplex.AddLe(xcolrowSum, lo[m]);
                        }
                        if (a[k] == kts) p--;
                        else p = k;
                        if (p >= 1)
                            for (int i = k; i >= p; i--)
                                a[i] = a[p] + i - p + 1;
                    }
                }
            }

            INumExpr costSum = cplex.NumExpr();
            INumExpr[] costSum1 = new INumExpr[kts];

            if (kts == 1)
            {
                for (int i = 0; i < dim; i++)
                    for (int j = 0; j < dim; j++)
                        costSum = cplex.Sum(costSum, cplex.Prod(x[i][j], dist[i][j]));
                cplex.AddMinimize(costSum);
            }
            else
            {
                for (int k = 0; k < kts; k++)
                {
                    costSum1[k] = cplex.NumExpr();
                    for (int i = 0; i < dim; i++)
                        for (int j = 0; j < dim; j++)
                            costSum1[k] = cplex.Sum(costSum1[k], cplex.Prod(x[i][k * dim + j], dist[i][j]));
                    //cplex.AddLe(costSum1[k], costSum);
                }
                costSum = cplex.Max(costSum1);
                cplex.AddMinimize(costSum);
            }

            try
            {
                if (cplex.Solve())
                {
                    textBox1.Text += "lambda = " + cplex.ObjValue + Environment.NewLine;
                    textBox1.Text += DateTime.Now.ToString() + Environment.NewLine;

                    WorkList.Clear();
                    int num_clust = 0;
                    for (int k = 0; k < kts; k++)
                    {
                        int dim1 = 0;
                        for (int i = 0; i < dim; i++)
                            for (int j = 0; j < dim; j++)
                                if (Convert.ToInt16(cplex.GetValue(x[i][k * dim + j])) == 1) dim1++;

                        if (dim1 > 0)
                        {
                            num_clust++;
                            for (int i = 0; i < dim; i++)
                                for (int j = 0; j < dim; j++)
                                    if (Convert.ToInt16(cplex.GetValue(x[i][k * dim + j])) == 1)
                                    {
                                        WorkList.Add(i);
                                        break;
                                    }

                            WorkList.Add(-1);
                        }
                    }
                    textBox1.Text += DateTime.Now.ToString() + Environment.NewLine;
                }
                else
                {
                    textBox1.Text += "Нет решения" + Environment.NewLine;
                    textBox1.Text += DateTime.Now.ToString() + Environment.NewLine;
                }
                cplex.End();
            }
            catch (ILOG.Concert.Exception ex)
            {
                textBox1.Text += "Concert Error: " + ex + Environment.NewLine;
                textBox1.Text += DateTime.Now.ToString() + Environment.NewLine;
            }
        }

        private void decomp1(int dim, int kts, int[][] dist, List<int> WorkList)
        {
            int[] p1 = new int[dim];
            int[] p2 = new int[dim];
            int p1_count = 1, p2_count = 1;
            List<int> AList = new List<int>();
            for (int i = 0; i < dim; i++)
                AList.Add(i);

            int max = dist[0][1];
            int max_i = 0;
            int max_j = 1;
            for (int i = 0; i < dim; i++)
                for (int j = 0; j < dim; j++)
                    if(i != j && dist[i][j] > max)
                    {
                        max = dist[i][j];
                        max_i = i;
                        max_j = j;
                    }
            p1[0] = max_i;
            p1[0] = max_j;
            while (p1_count + p2_count < dim)
            {
                int min1 = 100000;
                int min2 = 100000;
                int min_i1 = -1;
                int min_i2 = -1;
                for (int i = 0; i < dim; i++)
                {
                    if (AList.IndexOf(i) < 0) continue;
                    if(p1_count < 2 || p2_count > 1)
                    for (int j = 0; j < p1_count; j++)
                        if (dist[i][p1[j]] < min1)
                        {
                            min1 = dist[i][p1[j]];
                            min_i1 = i;
                        }
                    if(p2_count < 2 || p1_count > 1)
                    for (int j = 0; j < p2_count; j++)
                        if (dist[i][p2[j]] < min2)
                        {
                            min2 = dist[i][p2[j]];
                            min_i2 = i;
                        }
                }
                if (min1 < min2)
                {
                    p1[p1_count] = min_i1;
                    p1_count++;
                    AList.RemoveAt(AList.IndexOf(min_i1));
                }
                else
                {
                    p2[p2_count] = min_i2;
                    p2_count++;
                    AList.RemoveAt(AList.IndexOf(min_i2));
                }
            }
            WorkList.Clear();
            for (int j = 0; j < p1_count; j++)
                WorkList.Add(p1[j]);
            WorkList.Add(-1);
            for (int j = 0; j < p2_count; j++)
                WorkList.Add(p2[j]);
            WorkList.Add(-1);
        }

        private double one_tsp(int dim, int [][] matrix, int [] path)
        {
            int[] lo = new int[dim];

            for (int i = 0; i < dim; i++)
                lo[i] = 1;

            Cplex cplex = new Cplex();
            NumVarType varType = NumVarType.Bool;
            INumVar[][] x = new INumVar[dim][];
            for (int i = 0; i < dim; i++)
                x[i] = cplex.NumVarArray(dim, 0, 1);

            for (int i = 0; i < dim; i++)
                for (int j = 0; j < dim; j++)
                    x[i][j] = cplex.BoolVar();

            for (int j = 0; j < dim; j++)
            {
                INumExpr xcolSum = cplex.NumExpr();
                for (int i = 0; i < dim; i++)
                    xcolSum = cplex.Sum(xcolSum, x[i][j]);
                cplex.AddEq(lo[j], xcolSum);
            }

            varType = NumVarType.Float;
            INumVar[] u = new INumVar[dim];
            for (int j = 0; j < dim; j++)
                u[j] = cplex.NumVar(0, 100000);

            for (int j = 1; j < dim; j++)
                cplex.AddGe(u[j], 0);

            for (int i = 1; i < dim; i++)
                for (int j = 1; j < dim; j++)
                    if (i != j) cplex.AddLe(cplex.Sum(cplex.Diff(u[i], u[j]), cplex.Prod(dim, x[i][j])), dim - 1);

            for (int i = 0; i < dim; i++)
            {
                INumExpr xrowSum = cplex.NumExpr();
                for (int j = 0; j < dim; j++)
                    xrowSum = cplex.Sum(xrowSum, x[i][j]);
                cplex.AddEq(lo[i], xrowSum);
            }

            INumExpr costSum = cplex.NumExpr();
            for (int i = 0; i < dim; i++)
                for (int j = 0; j < dim; j++)
                    costSum = cplex.Sum(costSum, cplex.Prod(x[i][j], matrix[i][j]));
            cplex.AddMinimize(costSum);

            try
            {
                if (cplex.Solve())
                {
                    //MessageBox.Show("Solution status = " + cplex.GetStatus());
                    //MessageBox.Show("cost = " + cplex.ObjValue);
                    int ipath = 0;
                    int depo = -1;
                    for (int i = dim - 1; i >= 0; i--)
                        for (int j = 0; j < dim; j++)
                            if (Convert.ToInt16(cplex.GetValue(x[i][j])) == 1) depo = i;
                    path[ipath] = depo;
                    ipath++;
                    while (depo > -1)
                        for (int j = 0; j < dim; j++)
                            if (Convert.ToInt16(cplex.GetValue(x[path[ipath - 1]][j])) == 1)
                            {
                                path[ipath] = j;
                                ipath++;
                                if (j == depo) depo = -1;
                                break;
                            }
                    return (cplex.ObjValue);
                }
                cplex.End();
            }
            catch (ILOG.Concert.Exception ex)
            {
                System.Console.WriteLine("Concert Error: " + ex);
            }
            return -1;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                textBox2.Text = openFileDialog1.FileName;
        }

        private void SaveToFile(int dim, int [][] dist)
        {
            DateTime t = DateTime.Now;
            string FileName = "R" + dim.ToString() + "-";
            if (t.Day < 10) FileName += "0";
            FileName += t.Day.ToString();
            if (t.Month < 10) FileName += "0";
            FileName += t.Month.ToString() + (t.Year - 2000).ToString() + "-";
            if (t.Hour < 10) FileName += "0";
            FileName += t.Hour.ToString();
            if (t.Minute < 10) FileName += "0";
            FileName += t.Minute.ToString();
            if (t.Second < 10) FileName += "0";
            FileName += t.Second.ToString() + ".tsp";

            System.IO.StreamWriter writer = new System.IO.StreamWriter(FileName, false, System.Text.Encoding.Default);
            writer.Write("NAME: " + FileName + Environment.NewLine);
            writer.Write("TYPE: TSP" + Environment.NewLine);
            writer.Write("COMMENT: Random data" + Environment.NewLine);
            writer.Write("DIMENSION: " + dim.ToString() + Environment.NewLine);
            writer.Write("EDGE_WEIGHT_TYPE: EXPLICIT" + Environment.NewLine);
            writer.Write("EDGE_WEIGHT_FORMAT: FULL_MATRIX" + Environment.NewLine);
            writer.Write("DISPLAY_DATA_TYPE: NO_DISPLAY" + Environment.NewLine);
            writer.Write("EDGE_WEIGHT_SECTION" + Environment.NewLine);
            
            int max_znak = 0;
            for (int i = 0; i < dim; i++)
                for (int j = 0; j < dim; j++)
                {
                    int x = dist[i][j];
                    int kol_znak = 0;
                    while (x > 0)
                    {
                        x /= 10;
                        kol_znak++;
                    }
                    if (kol_znak > max_znak) max_znak = kol_znak;
                }
            
            for (int i = 0; i < dim; i++)
            {
                string s = "";
                for (int j = 0; j < dim; j++)
                {
                    int kol_znak = 0;
                    if (dist[i][j] < 10) kol_znak = 1;
                    else
                    {
                        int x = dist[i][j];
                        while (x > 0)
                        {
                            x /= 10;
                            kol_znak++;
                        }
                    }
                    for (int k = 0; k < max_znak - kol_znak; k++)
                        s += " ";
                    s += dist[i][j].ToString() + " ";
                }
                s = s + Environment.NewLine;
                writer.Write(s);
            }
            writer.Write("EOF");
            writer.Close();
        }
    }
}

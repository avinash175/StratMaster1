using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using CommonLib;
using System.Reflection;
using System.IO;
using StrategyCollection;
using RuleCollection;

namespace StrategyTesting
{
    public partial class Form1 : Form
    {
        private static StrategyData strategyData;
        private static BasicStrategy basicStrategy;
        private static PlotOption typeOfPlot;
        private static List<BasicRule> rules;
        private static OptimizationParameters optiParams;
        
        private int ht, wt;
        private int ht1, wt1;
        private int ht2, wt2;
        private int wt3, ht3;
        private int wt4, ht4;
        private int wt5, ht5;
        private static string selectedSec = "";
        private static int numBins;
        private static bool showToolTips;

        public Form1()
        {
            InitializeComponent();
            ht = this.Size.Height - splitContainer1.Size.Height;
            wt = this.Size.Width - splitContainer1.Size.Width;

            wt3 = splitContainer1.Panel1.Size.Width;
            ht3 = tabControl1.Size.Height - treeView1.Size.Height;

            ht1 = splitContainer1.Panel2.Size.Height - listBox1.Size.Height;
            wt1 = splitContainer1.Panel2.Size.Width - listBox1.Size.Width;

            ht2 = splitContainer1.Panel1.Size.Height - tabControl1.Size.Height;            
            wt2 = splitContainer1.Panel1.Size.Width - tabControl1.Size.Width;

            ht4 = splitContainer1.Panel2.Size.Height - tabControl2.Size.Height;
            wt4 = splitContainer1.Panel2.Size.Width - tabControl2.Size.Width;

            ht5 = tabControl2.Size.Height - dataGridView1.Size.Height;
            wt5 = tabControl2.Size.Width - dataGridView1.Size.Width;

            rules = new List<BasicRule>();

            dataGridView2.RowHeadersVisible = false;
            dataGridView2.ColumnHeadersVisible = false;
            dataGridView2.AllowUserToAddRows = false;
            
            dataGridView5.RowHeadersVisible = false;
            dataGridView5.ColumnHeadersVisible = false;
            dataGridView5.AllowUserToAddRows = false;
                        
            dataGridView5.ColumnCount = 2;
            DataGridViewCellStyle columnHeaderStyle = new DataGridViewCellStyle();
            dataGridView5.ColumnHeadersDefaultCellStyle = columnHeaderStyle;
            dataGridView5.Columns[0].ReadOnly = true;

            dataGridView5.Rows.Add(new object[] { "Min Value", 0 });
            dataGridView5.Rows.Add(new object[] { "Max Value", 10 });
            dataGridView5.Rows.Add(new object[] { "Step Size", 1 });
           
            dataGridView3.RowHeadersVisible = false;
            dataGridView3.ColumnHeadersVisible = false;
            dataGridView3.AllowUserToAddRows = false;
            
            //checkedListBox1.SetItemChecked(4, true);
            checkedListBox1.SetItemChecked(8, true);

            resizeDataGridnListBox();
            LoadStrategiesAndRules();
            StrategyChange();
            RuleChange();
            
            UpdateSettings();            
            RefreshValues();
        }

        public void RefreshValues()
        {
            if (radioButton1.Checked)
                typeOfPlot = PlotOption.SECURITY_PRICE;
            else if (radioButton2.Checked)
                typeOfPlot = PlotOption.SECURITY_TRADES;
            else if (radioButton3.Checked)
                typeOfPlot = PlotOption.MTM;
            else if (radioButton4.Checked)
                typeOfPlot = PlotOption.MOM;
            else if (radioButton5.Checked)
                typeOfPlot = PlotOption.YOY;
            else if (radioButton6.Checked)
                typeOfPlot = PlotOption.DD;
            else if (radioButton7.Checked)
                typeOfPlot = PlotOption.RETURN_DIST;
            else if (radioButton8.Checked)
                typeOfPlot = PlotOption.HOH;

            showToolTips = checkBox1.Checked;
            numBins = (int)numericUpDown1.Value;

        }

        public void LoadStrategiesAndRules()
        {

            Assembly[] arr = AppDomain.CurrentDomain.GetAssemblies();

            string[] types = Assembly.Load("StrategyCollection").GetTypes().Where(x => x.IsClass
                && x.Namespace == "StrategyCollection" && !x.IsAbstract && x.IsPublic)
                .Select(y => y.Name).OrderBy(y => y).ToArray();
            //string[] types = Assembly.GetExecutingAssembly().GetTypes().Where(x => x.IsClass
            //    && x.Namespace == "StrategyCollection" && !x.IsAbstract && x.IsPublic)
            //    .Select(y=>y.Name).ToArray();
            
            comboBox1.Items.Clear();
            comboBox1.Items.AddRange(types);
            comboBox1.SelectedIndex = 0;

            types = Assembly.Load("RuleCollection").GetTypes().Where(x => x.IsClass
                && x.Namespace == "RuleCollection" && !x.IsAbstract && x.IsPublic)
                .Select(y => y.Name).OrderBy(y => y).ToArray();

            comboBox6.Items.Clear();
            comboBox6.Items.AddRange(types);
            comboBox6.SelectedIndex = 0;
        }

        private void loadDataFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (LoadData())
                lb("Data Loaded: " + Path.GetFileName(openFileDialog1.FileName));
        }

        private void saveResultsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (SaveStats())
                lb("Statistics Saved");
        }

        private void runToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (RunStrategy())
                lb("Strategy Run Complete");
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (LoadData())
                lb("Data Loaded: " + Path.GetFileName(openFileDialog1.FileName));
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            if (SaveStats())
                lb("Statistics Saved");
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            if (RunStrategy())
                lb("Strategy Run Complete");
        }

        public bool LoadData()
        {
            openFileDialog1.FileName = "";            
            openFileDialog1.InitialDirectory = Application.StartupPath + "\\Data";
            DialogResult dr = openFileDialog1.ShowDialog();

            string filename;
            if (dr == System.Windows.Forms.DialogResult.OK)
            {
                filename = openFileDialog1.FileName;
            }
            else
            {
                return false;
            }
            try
            {
                TypeOfData fileFormat = (TypeOfData)Enum.Parse(typeof(TypeOfData), comboBox2.SelectedItem.ToString());
                TypeOfSeries seriesType = (TypeOfSeries)Enum.Parse(typeof(TypeOfSeries), comboBox3.SelectedItem.ToString());

                strategyData = new StrategyData(filename, fileFormat, seriesType);
                treeView1.Nodes[treeView1.Nodes.IndexOfKey("Node0")].Nodes.Clear();
                for (int i = 0; i < strategyData.InputData.Count; i++)
                {
                    treeView1.Nodes[treeView1.Nodes.IndexOfKey("Node0")]
                        .Nodes.Add(strategyData.InputData[i].Name, strategyData.InputData[i].Name);
                }
                treeView1.Nodes[treeView1.Nodes.IndexOfKey("Node0")].Expand();
                numericUpDown9.Value = (decimal)(strategyData.InputData[0].Dates.Length/10.0);
                numericUpDown10.Value = (numericUpDown9.Value/2);
                selectedSec = "ALL";
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        public bool SaveStats(bool IsDOD = false)
        {
            if (basicStrategy != null)
            {
                string path = Path.GetDirectoryName(Application.ExecutablePath);
                basicStrategy.SaveStatistics(path+"\\stats.csv",
                    path+"\\mtm.csv", strategyData,  IsDOD);
                return true;
            }
            else
            {
                MessageBox.Show("Load Data and then Run the Strategy");
                return false;
            }
        }

        public bool RunStrategy(Dictionary<string, object> values = null)
        {
            if (strategyData == null)
            {
                MessageBox.Show("Load Data and then Run the Strategy");
                return false;
            }

            RefreshValues();

            string className = (string)comboBox1.SelectedItem;
            System.Type type = Assembly.Load("StrategyCollection").GetType("StrategyCollection." + className);

            BindingFlags bfs = BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly;
            FieldInfo[] fi = type.GetFields(bfs);

            DataGridViewCellStyle columnHeaderStyle = new DataGridViewCellStyle();

            double alloc = (double)numericUpDown2.Value;

            Type type1 = typeof(CommonLib.TradingCost);
            double cost = Convert.ToDouble(type1.GetField(comboBox4.SelectedItem.ToString()).GetValue(null));
            type1 = typeof(CommonLib.TimeStep);
            double timeStep = Convert.ToDouble(type1.GetField(comboBox5.SelectedItem.ToString()).GetValue(null));

            List<object> args = new List<object>();
            args.Add(className);
            args.Add(alloc);
            args.Add(cost);
            args.Add(timeStep);           

            basicStrategy = (BasicStrategy)Activator.CreateInstance(type, args.ToArray());

            Dictionary<string, object> datagridValues = new Dictionary<string, object>();

            if (values != null)
                datagridValues = values;
            else
            {
                int cnt = dataGridView2.Rows.Count;

                for (int i = 0; i < cnt; i++)
                {
                    datagridValues.Add((string)dataGridView2.Rows[i].Cells[0].Value,
                        (object)dataGridView2.Rows[i].Cells[1].Value);
                }
            }

            if (values != null)
                datagridValues = values;

            for (int i = 0; i < fi.Length; i++)
            {
                object val = datagridValues[fi[i].Name];
                basicStrategy.GetType().InvokeMember(fi[i].Name,
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetField,
                    Type.DefaultBinder, basicStrategy, new object[] { val });
            }

            List<object[]> rows = null;
            try
            {
                StrategyData st = strategyData;

                if (selectedSec != ""
                   && selectedSec != "ALL")
                {
                    int idx = strategyData.SecName.IndexOf(selectedSec);
                    st = strategyData.GetSelectedElements(new int[] {idx});
                }                   

                basicStrategy.UseBidAsk = checkedListBox1.GetItemChecked(0);
                basicStrategy.UseSL = checkedListBox1.GetItemChecked(1);
                basicStrategy.UseTrailingSL = checkedListBox1.GetItemChecked(2);
                basicStrategy.UseTarget = checkedListBox1.GetItemChecked(3);
                basicStrategy.HoldOverNightPosition = checkedListBox1.GetItemChecked(4);
                basicStrategy.IsReverseStrategy = checkedListBox1.GetItemChecked(5);
                basicStrategy.UseAggStats = checkedListBox1.GetItemChecked(6);
                basicStrategy.UseNetPositionAsQty = checkedListBox1.GetItemChecked(7);
                basicStrategy.AllowSignalFlip = checkedListBox1.GetItemChecked(8);

                basicStrategy.StopLoss = (double)numericUpDown3.Value / 100.0;
                basicStrategy.Target = (double)numericUpDown4.Value / 100.0;
                basicStrategy.TrailingSL = (double)numericUpDown5.Value / 100.0;
                basicStrategy.SkipPeriod = (int)numericUpDown6.Value;
                basicStrategy.MaxHoldPeriod = (int)numericUpDown8.Value;

                basicStrategy.Rules = rules;

                basicStrategy.RunStrategy(st);
                rows = basicStrategy.ComputeStatistics(st);
            }
            catch (Exception ex)
            {
                if(values == null)
                {
                    lb(ex.Message);
                }
                return false;
            }

            if (values == null)
                DisplayResults(rows);
            
            return true;
        }

        public void DisplayResults(List<object[]> rows)
        {
            dataGridView1.DataSource = null;
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();

            dataGridView4.DataSource = null;
            dataGridView4.Rows.Clear();
            dataGridView4.Columns.Clear();

            dataGridView6.DataSource = null;
            dataGridView6.Rows.Clear();
            dataGridView6.Columns.Clear();

            dataGridView7.DataSource = null;
            dataGridView7.Rows.Clear();
            dataGridView7.Columns.Clear();

            DataGridViewCellStyle columnHeaderStyle = new DataGridViewCellStyle();
            dataGridView1.ColumnCount = rows[0].Length;
            dataGridView1.ColumnHeadersDefaultCellStyle = columnHeaderStyle;

            for (int i = 0; i < rows.Count; i++)
            {
                dataGridView1.Rows.Add(rows[i].Select(x => x.ToString()).ToArray());

                if (i == 0)
                {
                    dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.LightGray;
                    dataGridView1.Rows[i].DefaultCellStyle.Font = new Font(dataGridView1.Font, FontStyle.Italic);
                }
                else
                {
                    if (Convert.ToDouble(dataGridView1.Rows[i].Cells[1].Value) > 0)
                        dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.LightSteelBlue;
                    else if (Convert.ToDouble(dataGridView1.Rows[i].Cells[1].Value) < 0)
                        dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.LightSalmon;
                }
            }

            tabControl2.SelectedIndex = 0;
            
        }

        public void lb(string str)
        {
            listBox1.BringToFront();
            listBox1.Items.Add(DateTime.Now.ToString("HH:mm:ss.fff \t") + str);
            listBox1.SelectedIndex = listBox1.Items.Count - 1;
            listBox1.SelectedIndex = -1;
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            dataGridView1.BringToFront();
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (wt > 0 && ht > 0)
            {
                splitContainer1.Size = new Size(this.Size.Width - wt, this.Size.Height - ht);
            }
        }

        private void splitContainer1_Panel2_Resize(object sender, EventArgs e)
        {
            resizeDataGridnListBox();
        }

        private void resizeDataGridnListBox()
        {
            if (wt1 > 0 && ht1 > 0)
            {
                tabControl2.Size = new Size(splitContainer1.Panel2.Size.Width - wt4, splitContainer1.Panel2.Size.Height - ht4);
                dataGridView1.Size = new Size(tabControl2.Size.Width - wt5, tabControl2.Size.Height - ht5);
                dataGridView4.Size = new Size(tabControl2.Size.Width - wt5, tabControl2.Size.Height - ht5);
                dataGridView6.Size = new Size(tabControl2.Size.Width - wt5, tabControl2.Size.Height - ht5);
                dataGridView7.Size = new Size(tabControl2.Size.Width - wt5, tabControl2.Size.Height - ht5);
                listBox1.Size = new Size(splitContainer1.Panel2.Size.Width - wt1, listBox1.Size.Height);
            }
        }

        private void resizeTabControl()
        {
            if (wt2 > 0 && ht2 > 0)
            {
                tabControl1.Size = new Size(splitContainer1.Panel1.Size.Width - wt2, splitContainer1.Panel1.Size.Height - ht2);
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void tabControl1_Resize(object sender, EventArgs e)
        {
            treeView1.Size = new Size(treeView1.Size.Width, tabControl1.Size.Height - ht3);
        }

        private void splitContainer1_Panel1_Resize(object sender, EventArgs e)
        {
            resizeTabControl();
        }

        private void splitContainer1_Resize(object sender, EventArgs e)
        {
            //if(wt3>0)
            //splitContainer1.Panel1.Size = new Size(wt3, splitContainer1.Panel1.Size.Height);
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            selectedSec = e.Node.Text;
        }

        private void ShowPriceData()
        {
            if (selectedSec != "ALL"
                && selectedSec != "")
            {
                dataGridView1.DataSource = null;
                dataGridView1.Rows.Clear();
                dataGridView1.Columns.Clear();
                int idx = strategyData.SecName.IndexOf(selectedSec);
                dataGridView1.DataSource = strategyData.InputData[idx].ConvertToTimeStampArray();
            }
        }

        private void plotToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RefreshValues();
            PlotGraph();
        }

        private void PlotGraph()
        {
            try
            {
                if (strategyData == null)
                {
                    MessageBox.Show("Load Data and then press Plot");
                    return;
                }
                else
                {
                    if (strategyData.InputData.Count == 1)
                    {
                        selectedSec = strategyData.InputData[0].Name;
                    }
                    if (selectedSec == "")
                    {
                        MessageBox.Show("Select a security and then press Plot");
                        return;
                    }
                }
                
                Thread t = new Thread(new System.Threading.ThreadStart(ThreadProc));
                t.Start();
            }
            catch
            {

            }
        }

        public static void ThreadProc()
        {
            int idx = -1;
            if (selectedSec != ""
                   && selectedSec != "ALL")
            {
                idx = strategyData.SecName.IndexOf(selectedSec);
            }
            Application.Run(new Form2(strategyData, basicStrategy, 
                typeOfPlot, idx, showToolTips, numBins));
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            RefreshValues();
            PlotGraph();
        }

        private void StrategyChange()
        {
            string className = (string)comboBox1.SelectedItem;
            System.Type type = Assembly.Load("StrategyCollection").GetType("StrategyCollection." + className);

            BindingFlags bfs = BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly;
            FieldInfo[] fi = type.GetFields(bfs);

            List<object> args = new List<object>();
            args.Add(className);
            args.Add(1e7);
            args.Add(TradingCost.ZeroCost);
            args.Add(TimeStep.Hourly);

            object dummy = Activator.CreateInstance(type, args.ToArray());

            DataGridViewCellStyle columnHeaderStyle = new DataGridViewCellStyle();

            dataGridView2.Columns.Clear();
            dataGridView2.Rows.Clear();
            comboBox7.Items.Clear();

            dataGridView2.ColumnCount = 2;
            dataGridView2.ColumnHeadersDefaultCellStyle = columnHeaderStyle;
            dataGridView2.Columns[0].ReadOnly = true;

            for (int i = 0; i < fi.Length; i++)
            {
                object value = dummy.GetType().GetField(fi[i].Name).GetValue(dummy);
                object[] rowArr = new object[] { fi[i].Name, value };
                dataGridView2.Rows.Add(rowArr);
                comboBox7.Items.Add(fi[i].Name);                
            }

            if (comboBox7.Items.Count > 0)
            {
                comboBox7.SelectedIndex = 0;
            }

            optiParams = new OptimizationParameters(className);
        }

        private void RuleChange()
        {
            string className = (string)comboBox6.SelectedItem;
            System.Type type = Assembly.Load("RuleCollection").GetType("RuleCollection." + className);

            BindingFlags bfs = BindingFlags.Instance | BindingFlags.Public;
            FieldInfo[] fi = type.GetFields(bfs);

            List<object> args = new List<object>();
            args.Add(className);
            
            object dummy = Activator.CreateInstance(type, args.ToArray());

            DataGridViewCellStyle columnHeaderStyle = new DataGridViewCellStyle();

            dataGridView3.Columns.Clear();
            dataGridView3.Rows.Clear();

            dataGridView3.ColumnCount = 2;
            dataGridView3.ColumnHeadersDefaultCellStyle = columnHeaderStyle;
            dataGridView3.Columns[0].ReadOnly = true;

            for (int i = 0; i < fi.Length; i++)
            {
                object value = dummy.GetType().GetField(fi[i].Name).GetValue(dummy);
                object[] rowArr = new object[] { fi[i].Name, value };
                dataGridView3.Rows.Add(rowArr);
            }
        }

        private void comboBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            StrategyChange();
        }

        private void UpdateSettings()
        {            
            comboBox2.Items.AddRange(Enum.GetNames(typeof(TypeOfData)));
            comboBox2.SelectedIndex = 0;

            comboBox3.Items.AddRange(Enum.GetNames(typeof(TypeOfSeries)));
            comboBox3.SelectedIndex = 0;

            System.Type type = typeof(CommonLib.TradingCost);// System.Type.GetType("CommonLib.TradingCost");
            BindingFlags bfs = BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly;
            FieldInfo[] fi = type.GetFields(bfs);

            comboBox4.Items.AddRange(fi.Select(x=>x.Name).ToArray());
            comboBox4.SelectedIndex = 0;

            type = typeof(CommonLib.TimeStep);            
            fi = type.GetFields(bfs);

            comboBox5.Items.AddRange(fi.Select(x => x.Name).ToArray());
            comboBox5.SelectedIndex = 0;
        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string className = (string)comboBox6.SelectedItem;
            System.Type type = Assembly.Load("RuleCollection").GetType("RuleCollection." + className);

            BindingFlags bfs = BindingFlags.Instance | BindingFlags.Public;
            FieldInfo[] fi = type.GetFields(bfs);

            DataGridViewCellStyle columnHeaderStyle = new DataGridViewCellStyle();

            List<object> args = new List<object>();
            args.Add(className);

            BasicRule br = (BasicRule)Activator.CreateInstance(type, args.ToArray());

            Dictionary<string, object> datagridValues = new Dictionary<string, object>();

            int cnt = dataGridView3.Rows.Count;

            for (int i = 0; i < cnt; i++)
            {
                datagridValues.Add((string)dataGridView3.Rows[i].Cells[0].Value,
                    (object)dataGridView3.Rows[i].Cells[1].Value);
            }

            for (int i = 0; i < fi.Length; i++)
            {
                object val = datagridValues[fi[i].Name];
                br.GetType().InvokeMember(fi[i].Name,
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetField,
                    Type.DefaultBinder, br, new object[] { val });
            }

            rules.Add(br);
            treeView2.Nodes[treeView2.Nodes.IndexOfKey("Node0")]
                        .Nodes.Add(className, className);
            treeView2.Nodes[treeView2.Nodes.IndexOfKey("Node0")].Expand();
        }

        private void comboBox6_SelectedIndexChanged(object sender, EventArgs e)
        {
            RuleChange();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            rules.Clear();
            treeView2.Nodes[treeView2.Nodes.IndexOfKey("Node0")].Nodes.Clear();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                if (treeView2.SelectedNode.Parent
                    == treeView2.Nodes[treeView2.Nodes.IndexOfKey("Node0")])
                {
                    rules.RemoveAt(treeView2.Nodes[treeView2.Nodes.IndexOfKey("Node0")].Nodes.IndexOf(treeView2.SelectedNode));
                    treeView2.SelectedNode.Remove();
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void SaveToLog(string fileName)
        {
            List<string> logs = new List<string>();
            logs.Add("\n" + DateTime.Now.ToString("dd-MMM-yyyy HH:mm:ss"));

            logs.Add("Strategy Parameters");

            System.Type type = basicStrategy.GetType();
            BindingFlags bfs = BindingFlags.Instance | BindingFlags.Public;
            FieldInfo[] fi = type.GetFields(bfs);

            for (int i = 0; i < fi.Length; i++)
            {
                object value = basicStrategy.GetType().GetField(fi[i].Name).GetValue(basicStrategy);
                logs.Add(fi[i].Name + "," + value.ToString());
            }

            logs.Add("Rule Parameters");

            for (int i = 0; i < basicStrategy.Rules.Count; i++)
            {
                type = basicStrategy.Rules[i].GetType();
                fi = type.GetFields(bfs);
                logs.Add("Rule Name," + type.Name);
                for (int j = 0; j < fi.Length; j++)
                {
                    object value = basicStrategy.Rules[i].GetType().GetField(fi[j].Name).GetValue(basicStrategy.Rules[i]);
                    logs.Add(fi[j].Name + "," + value.ToString());
                }
            }

            for (int i = 0; i < basicStrategy.rows.Count; i++)
            {
                logs.Add(UF.ToCSVString(basicStrategy.rows[i]));
            }

            FileWrite.Write(logs, fileName, true);
            lb("Logs saved to " + fileName);
        }

        private void button4_Click(object sender, EventArgs e)
        {

        }

        private void button8_Click(object sender, EventArgs e)
        {
            lb("Optimization Started");
            try
            {
                string fileName = "AllSampleResults.csv";
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }

                Dictionary<string, object> datagridValues = new Dictionary<string, object>();
                List<double> mtm = new List<double>();
                List<double> mtm2tv = new List<double>();
                List<double> sharpe = new List<double>();
                List<double> numTrades = new List<double>();
                List<double> maxDD = new List<double>();
                List<double> perPosMonths = new List<double>();
                bool flagBestRegion = false;

                int cnt = dataGridView2.Rows.Count;
                for (int i = 0; i < cnt; i++)
                {
                    datagridValues.Add((string)dataGridView2.Rows[i].Cells[0].Value,
                        (object)dataGridView2.Rows[i].Cells[1].Value);
                }

                optiParams.GenerateAllCombinations();

                progressBar1.Minimum = 0;
                progressBar1.Maximum = optiParams.AllCombinations.Count - 1;

                List<string> logs = new List<string>();
                for (int i = 0; i < optiParams.AllCombinations.Count; i++)
                {
                    string strNames = "";
                    string strValues = "";

                    for (int j = 0; j < optiParams.AllCombinations[i].Count; j++)
                    {
                        datagridValues[optiParams.AllCombinations[i][j].ParamName] =
                            (object)optiParams.AllCombinations[i][j].Value;
                        strNames = optiParams.AllCombinations[i][j].ParamName + "," + strNames;
                        strValues = optiParams.AllCombinations[i][j].Value.ToString() + "," + strValues;
                    }

                    RunStrategy(datagridValues);

                    if (basicStrategy.rows != null)
                    {
                        if (i == 0)
                        {
                            logs.Add(strNames + UF.ToCSVString(basicStrategy.rows[0]));
                        }
                        if ((selectedSec == "" || selectedSec == "ALL") &&
                            basicStrategy.Stats.TotalMTM.Count > 1)
                        {
                            for (int j = 1; j < basicStrategy.rows.Count; j++)
                            {
                                logs.Add(strValues + UF.ToCSVString(basicStrategy.rows[j]));
                            }
                        }
                        else
                        {
                            flagBestRegion = true;
                            int idx = 1;
                            //if (selectedSec != "")
                            //{
                            //    idx = strategyData.SecName.IndexOf(selectedSec) + 1;
                            //}
                            logs.Add(strValues + UF.ToCSVString(basicStrategy.rows[idx]));
                            mtm.Add(basicStrategy.Stats.TotalMTM[idx - 1]);
                            mtm2tv.Add(basicStrategy.Stats.MTM2TV[idx - 1]);
                            sharpe.Add(basicStrategy.Stats.Return2Risk[idx - 1]);
                            numTrades.Add(basicStrategy.Stats.NumTrades[idx - 1]);
                            maxDD.Add(basicStrategy.Stats.MaxDrawDown[idx - 1]);
                            perPosMonths.Add(basicStrategy.Stats.PerPosMonths[idx - 1]);
                        }
                    }
                    progressBar1.Value = i;
                    Application.DoEvents();
                }

                if (flagBestRegion)
                {
                    double mtmT = UF.Percentile(mtm.ToArray(), 0.75);
                    double mtm2tvT = UF.Percentile(mtm2tv.ToArray(), 0.50);
                    double sharpeT = UF.Percentile(sharpe.ToArray(), 0.50);
                    double numTradesT = UF.Percentile(numTrades.ToArray(), 0.5);
                    double maxDDT = UF.Percentile(maxDD.ToArray(), 0.50);
                    double perPosMonthsT = UF.Percentile(perPosMonths.ToArray(), 0.50);

                    int[] idxs = mtm.Select((x, j) => new { Index = j, Val = x })
                        .Where(x => x.Val > mtmT).Select(x => x.Index).ToArray();
                    idxs = idxs.Intersect(mtm2tv.Select((x, j) => new { Index = j, Val = x })
                        .Where(x => x.Val > mtm2tvT).Select(x => x.Index).ToArray()).ToArray();
                    idxs = idxs.Intersect(sharpe.Select((x, j) => new { Index = j, Val = x })
                        .Where(x => x.Val > sharpeT).Select(x => x.Index).ToArray()).ToArray();
                    idxs = idxs.Intersect(numTrades.Select((x, j) => new { Index = j, Val = x })
                        .Where(x => x.Val > numTradesT).Select(x => x.Index).ToArray()).ToArray();
                    idxs = idxs.Intersect(maxDD.Select((x, j) => new { Index = j, Val = x })
                        .Where(x => x.Val < maxDDT).Select(x => x.Index).ToArray()).ToArray();
                    idxs = idxs.Intersect(perPosMonths.Select((x, j) => new { Index = j, Val = x })
                        .Where(x => x.Val > perPosMonthsT).Select(x => x.Index).ToArray()).ToArray();

                    if (idxs.Length > 0)
                    {
                        List<List<ParamValue>> bestVals = optiParams.AllCombinations.
                            Where((x, i) => idxs.Contains(i)).ToList();
                        List<int> idxsList = idxs.Select(x => x + 1).ToList();
                        idxsList.Insert(0, 0);
                        //bestVals = optiParams.FindStableRegion(bestVals);
                        FileWrite.Write(logs.Where((x, i) => idxsList.Contains(i)).ToList(), "FilteredResults.csv", false);
                    }
                    else
                    {
                        MessageBox.Show("Best Region not found");
                    }
                }

                FileWrite.Write(logs, fileName, true);
                SaveToLog(fileName);
                lb("Optimization Done");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            OptimizationParamAttributes attribute = new OptimizationParamAttributes();
             
            attribute.ParamName = (string)comboBox7.SelectedItem;
            attribute.Min = Convert.ToDouble(dataGridView5.Rows[0].Cells[1].Value);
            attribute.Max = Convert.ToDouble(dataGridView5.Rows[1].Cells[1].Value);
            attribute.StepSize = Convert.ToDouble(dataGridView5.Rows[2].Cells[1].Value);

            if(attribute.StepSize == 0.0 ||
                attribute.Min > attribute.Max)
            {
                return;
            }

            if (!optiParams.Parameters.Select(x => x.ParamName).Contains(attribute.ParamName))
            {
                optiParams.Parameters.Add(attribute);
                treeView3.Nodes[treeView3.Nodes.IndexOfKey("Node0")]
                        .Nodes.Add(attribute.ParamName, attribute.ParamName);
                treeView3.Nodes[treeView3.Nodes.IndexOfKey("Node0")].Expand();   
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                if (treeView3.SelectedNode.Parent
                    == treeView3.Nodes[treeView3.Nodes.IndexOfKey("Node0")])
                {
                    optiParams.Parameters.RemoveAt(treeView3.Nodes[treeView3.Nodes.IndexOfKey("Node0")].Nodes.IndexOf(treeView3.SelectedNode));
                    treeView3.SelectedNode.Remove();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            optiParams.Parameters.Clear();
            treeView3.Nodes[treeView3.Nodes.IndexOfKey("Node0")].Nodes.Clear();
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
            {
                numericUpDown9.Enabled = true;
                numericUpDown10.Enabled = true;
            }
            else
            {
                numericUpDown9.Enabled = false;
                numericUpDown10.Enabled = false;
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.L || e.KeyData == Keys.F2)
            {
                if (LoadData())
                    lb("Data Loaded: " + openFileDialog1.FileName);
            }
            else if (e.KeyData == Keys.R || e.KeyData == Keys.F3)
            {
                if (RunStrategy())
                    lb("Strategy Run Complete");
            }
            else if (e.KeyData == Keys.P || e.KeyData == Keys.F4)
            {
                RefreshValues();
                PlotGraph();
            }
            else if (e.KeyData == Keys.F5)
            {
                RefreshValues();
                typeOfPlot = PlotOption.SECURITY_TRADES;
                PlotGraph();
            }
            else if (e.KeyData == Keys.F6)
            {
                RefreshValues();
                typeOfPlot = PlotOption.MTM;
                PlotGraph();
            }
            else if (e.KeyData == Keys.F7)
            {
                RefreshValues();
                typeOfPlot = PlotOption.MOM;
                PlotGraph();
            }
            else if (e.KeyData == Keys.F8)
            {
                RefreshValues();
                typeOfPlot = PlotOption.YOY;
                PlotGraph();
            }
        }

        private void toolStripButton8_Click(object sender, EventArgs e)
        {
            if (SaveStats(true))
                lb("Statistics Saved to stats.csv");
        }

        private void toolStripButton9_Click(object sender, EventArgs e)
        {
            try
            {
                if (strategyData == null)
                {
                    MessageBox.Show("Load Data and Run Strategy");
                    return;
                }
                else if (basicStrategy.Stats == null)
                {
                    MessageBox.Show("Run Strategy and Try Again");
                    return;
                }
                else
                {
                    if (strategyData.InputData.Count == 1)
                    {
                        selectedSec = strategyData.InputData[0].Name;
                    }
                    if (selectedSec == "")
                    {
                        MessageBox.Show("Select a security and then Check the Trades");
                        return;
                    }

                    ShowTrades(selectedSec);
                    UpdateYOY(selectedSec);
                    UpdateMOM(selectedSec);                    
                }                
            }
            catch
            {

            }
        }

        private void storeInLogFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (strategyData == null || basicStrategy == null)
            {
                MessageBox.Show("Load Data, Run Strategy and try again");
                return;
            }
            else
            {
                SaveToLog("log.csv");
            }
        }

        private void deleteLogFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (File.Exists("log.csv"))
                {
                    File.Delete("log.csv");
                    lb("Log File Deleted");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void saveMTMDODToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (SaveStats(true))
                lb("Statistics Saved to stats.csv");
        }

        private void showDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowPriceData();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox1 ab = new AboutBox1();
            ab.ShowDialog();
        }

        private void dataGridView4_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            foreach (DataGridViewRow row in dataGridView4.Rows)
            {
                if (Convert.ToDouble(row.Cells[7].Value) > 0)
                {
                    row.DefaultCellStyle.BackColor = Color.LightSteelBlue;
                }
                else if (Convert.ToDouble(row.Cells[7].Value) < 0)
                {
                    row.DefaultCellStyle.BackColor = Color.LightSalmon;
                }
            }
        }

        private void UpdateYOY(string selSec)
        {
            List<object[]> rows = basicStrategy.ComputeStatisticsYOY(strategyData, selSec);

            dataGridView6.DataSource = null;
            dataGridView6.Rows.Clear();
            dataGridView6.Columns.Clear();
            DataGridViewCellStyle columnHeaderStyle = new DataGridViewCellStyle();
            dataGridView6.ColumnCount = rows[0].Length;
            dataGridView6.ColumnHeadersDefaultCellStyle = columnHeaderStyle;

            for (int i = 0; i < rows.Count; i++)
            {
                dataGridView6.Rows.Add(rows[i].Select(x => x.ToString()).ToArray());
                if (i == 0)
                {
                    dataGridView6.Rows[i].DefaultCellStyle.BackColor = Color.LightGray;
                    dataGridView6.Rows[i].DefaultCellStyle.Font = new Font(dataGridView6.Font, FontStyle.Italic);
                }
                else
                {
                    if (Convert.ToDouble(dataGridView6.Rows[i].Cells[1].Value) > 0)
                        dataGridView6.Rows[i].DefaultCellStyle.BackColor = Color.LightSteelBlue;
                    else if (Convert.ToDouble(dataGridView6.Rows[i].Cells[1].Value) < 0)
                        dataGridView6.Rows[i].DefaultCellStyle.BackColor = Color.LightSalmon;                        
                }
            }
        }

        private void UpdateMOM(string selSec)
        {
            List<object[]> rows = basicStrategy.ComputeStatisticsMOM(strategyData, selSec);

            dataGridView7.DataSource = null;
            dataGridView7.Rows.Clear();
            dataGridView7.Columns.Clear();
            DataGridViewCellStyle columnHeaderStyle = new DataGridViewCellStyle();
            dataGridView7.ColumnCount = rows[0].Length;
            dataGridView7.ColumnHeadersDefaultCellStyle = columnHeaderStyle;

            for (int i = 0; i < rows.Count; i++)
            {
                dataGridView7.Rows.Add(rows[i].Select(x => x.ToString()).ToArray());
                if (i == 0)
                {
                    dataGridView7.Rows[i].DefaultCellStyle.BackColor = Color.LightGray;
                    dataGridView7.Rows[i].DefaultCellStyle.Font = new Font(dataGridView7.Font, FontStyle.Italic);
                }
                else
                {
                    if (Convert.ToDouble(dataGridView7.Rows[i].Cells[1].Value) > 0)
                        dataGridView7.Rows[i].DefaultCellStyle.BackColor = Color.LightSteelBlue;
                    else if (Convert.ToDouble(dataGridView7.Rows[i].Cells[1].Value) < 0)
                        dataGridView7.Rows[i].DefaultCellStyle.BackColor = Color.LightSalmon;
                }
            }
            
        }

        private void ShowTrades(string selSec)
        {
            int idx = strategyData.SecName.FindIndex(x => x == selSec);
            dataGridView4.DataSource = basicStrategy.Stats.Trades[idx];
        }
                
        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            string selDataSec = dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString();

            if (strategyData.SecName.Contains(selDataSec))
            {
                UpdateYOY(selDataSec);
                UpdateMOM(selDataSec);
                ShowTrades(selDataSec);
                tabControl2.SelectedIndex = 1;
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CommonLib;
using System.Windows.Forms.DataVisualization.Charting;

namespace StrategyTesting
{
    public partial class Form2 : Form
    {
        private int wt, ht;
        private StrategyData plotData;
        private BasicStrategy plotStrategy;
        private PlotOption typeOfPlot;
        private int idx;
        private bool showToolTips;
        private int numBins;

        public Form2(StrategyData st, BasicStrategy bs, PlotOption po, int _idx,
            bool _showToolTips = false, int _numBins = 40)
        {
            InitializeComponent();
            wt = this.Size.Width - chart1.Size.Width;
            ht = this.Size.Height - chart1.Size.Height;
            plotData = st;
            plotStrategy = bs;
            typeOfPlot = po;
            idx = _idx;
            showToolTips = _showToolTips;
            numBins = _numBins;

            chart1.ChartAreas[0].CursorX.IsUserEnabled = true;
            chart1.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
            chart1.ChartAreas[0].AxisX.ScaleView.Zoomable = true;

            chart1.ChartAreas[0].AxisY.IsStartedFromZero = false;

            chart1.Legends[0].Docking = Docking.Top;
            chart1.Legends[0].LegendStyle = LegendStyle.Row;
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            chart1.ChartAreas[0].AxisX.IsMarginVisible = false;
            chart1.ChartAreas[0].AxisY.IsMarginVisible = false;
            chart1.ChartAreas[0].AxisY.IsStartedFromZero = false;

            chart1.ChartAreas[0].AxisX.MajorGrid.LineColor = Color.Gold;
            chart1.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.Gold;
            chart1.ChartAreas[0].AxisY2.MajorGrid.LineColor = Color.Gold;

            chart1.ChartAreas[0].AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dash;
            chart1.ChartAreas[0].AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dash;
            chart1.ChartAreas[0].AxisY2.MajorGrid.LineDashStyle = ChartDashStyle.Dash;
            Plot();
        }

        private void Form2_Resize(object sender, EventArgs e)
        {
            if (wt > 0 && ht > 0)
            {
                chart1.Size = new Size(this.Size.Width - wt, this.Size.Height - ht);
            }
        }

        private void Plot()
        {
            if (plotData != null)
            {
                chart1.Series.Clear();
                if (typeOfPlot == PlotOption.SECURITY_PRICE)
                {
                    if (idx < 0)
                        return;

                    chart1.ChartAreas[0].AxisY.LabelStyle.Format = "0";
                    chart1.ChartAreas[0].AxisX.LabelStyle.Format = "dd-MMM-yy HH:mm";
                    string chartName = plotData.SecName[idx];
                    chart1.Series.Add(chartName);

                    if ((plotData.SeriesType == TypeOfSeries.OHLC ||
                        plotData.SeriesType == TypeOfSeries.OHLCV) && showToolTips)
                    {
                        chart1.Series[chartName].ChartType = SeriesChartType.Stock;
                        for (int i = 0; i < plotData.InputData[idx].Dates.Length; i++)
                        {
                            chart1.Series[chartName].Points.AddXY(plotData.InputData[idx].Dates[i],
                                plotData.InputData[idx].OHLC.high[i], plotData.InputData[idx].OHLC.low[i],
                                plotData.InputData[idx].OHLC.open[i], plotData.InputData[idx].OHLC.close[i]);
                        }
                    }
                    else
                    {
                        chart1.Series[chartName].Points.DataBindXY(plotData.InputData[idx].Dates,
                            plotData.InputData[idx].Prices);
                        chart1.Series[chartName].ChartType = SeriesChartType.FastLine;
                    }

                    chart1.Series[chartName].IsXValueIndexed = true;
                    chart1.Series[chartName].BorderWidth = 3;

                    foreach (Series series in chart1.Series)
                    {
                        series.IsXValueIndexed = true;
                    }
                }
                else if (typeOfPlot == PlotOption.SECURITY_TRADES)
                {
                    if (idx < 0)
                        return;

                    chart1.ChartAreas[0].AxisY.LabelStyle.Format = "0";
                    chart1.ChartAreas[0].AxisX.IntervalType = DateTimeIntervalType.NotSet;
                    string chartName = "";
                    
                    chartName = plotData.SecName[idx];
                    chart1.Series.Add("MTM");
                    chart1.Series["MTM"].Points.DataBindXY(plotStrategy.Stats.MTM[idx].Dates,
                        UF.CummSum(plotStrategy.Stats.MTM[idx].Prices));

                    chart1.Series["MTM"].ChartType = SeriesChartType.Area;
                    chart1.Series["MTM"].BorderWidth = 3;
                   
                    chart1.Series.Add(chartName);
                    if ((plotData.SeriesType == TypeOfSeries.OHLC ||
                        plotData.SeriesType == TypeOfSeries.OHLCV) && showToolTips)
                    {
                        chart1.Series[chartName].ChartType = SeriesChartType.Stock;
                        for (int i = 0; i < plotData.InputData[idx].Dates.Length; i++)
                        {
                            chart1.Series[chartName].Points.AddXY(plotData.InputData[idx].Dates[i],
                                plotData.InputData[idx].OHLC.high[i], plotData.InputData[idx].OHLC.low[i],
                                plotData.InputData[idx].OHLC.open[i], plotData.InputData[idx].OHLC.close[i]);
                        }
                    }
                    else
                    {
                        chart1.Series[chartName].Points.DataBindXY(plotData.InputData[idx].Dates,
                            plotData.InputData[idx].Prices);
                        chart1.Series[chartName].ChartType = SeriesChartType.FastLine;
                    }
                    chart1.Series[chartName].BorderWidth = 3;
                    chart1.Series[chartName].YAxisType = AxisType.Secondary;                    
                    chart1.ChartAreas[0].AxisY2.IsStartedFromZero = false;

                    TimeStamp[] longEntry = plotStrategy.Stats.Trades[idx].Where(x => x.LongShort == LongShortType.LONG)
                        .Select(x => new TimeStamp(x.EntryDate, x.EntryPrice)).ToArray();
                    TimeStamp[] longExit = plotStrategy.Stats.Trades[idx].Where(x => x.LongShort == LongShortType.LONG)
                        .Select(x => new TimeStamp(x.ExitDate, x.ExitPrice)).ToArray();
                    TimeStamp[] ShortEntry = plotStrategy.Stats.Trades[idx].Where(x => x.LongShort == LongShortType.SHORT)
                        .Select(x => new TimeStamp(x.EntryDate, x.EntryPrice)).ToArray();
                    TimeStamp[] ShortExit = plotStrategy.Stats.Trades[idx].Where(x => x.LongShort == LongShortType.SHORT)
                        .Select(x => new TimeStamp(x.ExitDate, x.ExitPrice)).ToArray();

                    int markerSize = 10;

                    DateTime[] tempdates = longEntry.Select(x=>x.Date).ToArray();

                    double[] leP = plotData.InputData[idx].Dates.Select((x, i) => tempdates.Contains(x) ? 
                        plotData.InputData[idx].Prices[i] : Double.NaN).ToArray();

                    chart1.Series.Add("LongEntry");
                    chart1.Series["LongEntry"].Points.DataBindXY(plotData.InputData[idx].Dates, leP);
                    chart1.Series["LongEntry"].ChartType = SeriesChartType.Point;
                    chart1.Series["LongEntry"].MarkerSize = markerSize;
                    chart1.Series["LongEntry"].MarkerStyle = MarkerStyle.Triangle;
                    chart1.Series["LongEntry"].MarkerColor = Color.Green;
                    chart1.Series["LongEntry"].YAxisType = AxisType.Secondary;

                    tempdates = longExit.Select(x => x.Date).ToArray();

                    double[] leE = plotData.InputData[idx].Dates.Select((x, i) => tempdates.Contains(x) ?
                        plotData.InputData[idx].Prices[i] : Double.NaN).ToArray();

                    chart1.Series.Add("LongExit");
                    chart1.Series["LongExit"].Points.DataBindXY(plotData.InputData[idx].Dates, leE);
                    chart1.Series["LongExit"].ChartType = SeriesChartType.Point;
                    chart1.Series["LongExit"].MarkerSize = markerSize;
                    chart1.Series["LongExit"].MarkerStyle = MarkerStyle.Cross;
                    chart1.Series["LongExit"].MarkerColor = Color.Green;
                    chart1.Series["LongExit"].YAxisType = AxisType.Secondary;

                    tempdates = ShortEntry.Select(x => x.Date).ToArray();

                    double[] stP = plotData.InputData[idx].Dates.Select((x, i) => tempdates.Contains(x) ?
                        plotData.InputData[idx].Prices[i] : Double.NaN).ToArray();

                    chart1.Series.Add("ShortEntry");
                    chart1.Series["ShortEntry"].Points.DataBindXY(plotData.InputData[idx].Dates, stP);
                    chart1.Series["ShortEntry"].ChartType = SeriesChartType.Point;
                    chart1.Series["ShortEntry"].MarkerSize = markerSize;
                    chart1.Series["ShortEntry"].MarkerStyle = MarkerStyle.Triangle;
                    chart1.Series["ShortEntry"].MarkerColor = Color.Red;                    
                    chart1.Series["ShortEntry"].YAxisType = AxisType.Secondary;

                    tempdates = ShortExit.Select(x => x.Date).ToArray();

                    double[] stE = plotData.InputData[idx].Dates.Select((x, i) => tempdates.Contains(x) ?
                        plotData.InputData[idx].Prices[i] : Double.NaN).ToArray();

                    chart1.Series.Add("ShortExit");
                    chart1.Series["ShortExit"].Points.DataBindXY(plotData.InputData[idx].Dates, stE);
                    chart1.Series["ShortExit"].ChartType = SeriesChartType.Point;
                    chart1.Series["ShortExit"].MarkerSize = markerSize;
                    chart1.Series["ShortExit"].MarkerStyle = MarkerStyle.Cross;
                    chart1.Series["ShortExit"].MarkerColor = Color.Red;  
                    chart1.Series["ShortExit"].YAxisType = AxisType.Secondary;

                    foreach (Series series in chart1.Series)
                    {
                        series.IsXValueIndexed = true;
                    }

                    //if (showToolTips)
                    //{
                    //    chart1.Series["LongEntry"].ToolTip = "Long \nEntry Date = #VALX \nEntry Price = #VALY";
                    //    chart1.Series["LongExit"].ToolTip = "Long \nExit Date = #VALX \nExit Price = #VALY";
                    //    chart1.Series["ShortEntry"].ToolTip = "Short \nEntry Date = #VALX \nEntry Price = #VALY";
                    //    chart1.Series["ShortExit"].ToolTip = "Short \nExit Date = #VALX \nExit Price = #VALY"; ;
                    //}
                }
                else if (typeOfPlot == PlotOption.MOM)
                {
                    if (idx < 0 && plotStrategy.AggStats==null)
                        return;
                                        
                    string chartName = "";
                    
                    if (idx >= 0)
                    {
                        chartName = plotData.SecName[idx] + " Return";
                        chart1.Series.Add(chartName);
                        chart1.Series[chartName].Points.DataBindXY(plotStrategy.Stats.MOM[idx].Select(x => x.Date).ToArray(),
                            plotStrategy.Stats.MOM[idx].Select(x => x.Price).ToArray());
                    }
                    chart1.Series[chartName].ChartType = SeriesChartType.Column;
                    chart1.ChartAreas[0].AxisY.LabelStyle.Format = "0.000%";
                    chart1.ChartAreas[0].AxisX.LabelStyle.Format = "MM-yyyy";
                    //chart1.Series[chartName].BorderWidth = 3;
                    chart1.ChartAreas[0].AxisX.IsMarginVisible = true;
                    chart1.ChartAreas[0].AxisY.IsMarginVisible = true;

                    if (idx >= 0)
                    {

                        chart1.ChartAreas.Add("C2");
                        chartName = plotData.SecName[idx] + " MTM/TV";
                        chart1.Series.Add(chartName);
                        chart1.Series[chartName].Points.DataBindXY(plotStrategy.Stats.MOMm2v[idx].Select(x => x.Date).ToArray(),
                            plotStrategy.Stats.MOMm2v[idx].Select(x => x.Price).ToArray());
                        chart1.Series[chartName].ChartType = SeriesChartType.Column;
                        chart1.Series[chartName].ChartArea = chart1.ChartAreas[1].Name;

                        chart1.ChartAreas[1].AxisX.IsMarginVisible = true;
                        chart1.ChartAreas[1].AxisY.IsMarginVisible = true;

                        //chart1.ChartAreas[1].AlignmentOrientation = AreaAlignmentOrientations.Vertical;
                        chart1.ChartAreas[1].AlignWithChartArea = chart1.ChartAreas[0].Name;

                        chart1.ChartAreas[1].AxisY.LabelStyle.Format = "0.000%";

                        chart1.ChartAreas[1].AxisX.MajorGrid.LineColor = Color.Gold;
                        chart1.ChartAreas[1].AxisY.MajorGrid.LineColor = Color.Gold;

                        chart1.ChartAreas[1].AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dash;
                        chart1.ChartAreas[1].AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dash;

                        chart1.Series[plotData.SecName[idx] + " Return"].ToolTip = "Date = #VALX \nReturn = #VALY";
                        chart1.Series[plotData.SecName[idx] + " MTM/TV"].ToolTip = "Date = #VALX \nMTM/TV = #VALY";
                    }

                }
                else if (typeOfPlot == PlotOption.YOY)
                {
                    if (idx < 0 && plotStrategy.AggStats == null)
                        return;

                    string chartName = "";
                    
                    if (idx >= 0)
                    {
                        chartName = plotData.SecName[idx] + " Return";
                        chart1.Series.Add(chartName);
                        chart1.Series[chartName].Points.DataBindXY(plotStrategy.Stats.YOY[idx].Select(x => x.Date).ToArray(),
                            plotStrategy.Stats.YOY[idx].Select(x => x.Price).ToArray());
                    }
                    
                    chart1.Series[chartName].ChartType = SeriesChartType.Column;
                    chart1.Series[chartName].LabelFormat = "0.00%";
                    chart1.Series[chartName].IsValueShownAsLabel = true;
                    chart1.ChartAreas[0].AxisY.LabelStyle.Format = "0.00%";
                    chart1.ChartAreas[0].AxisX.LabelStyle.Format = "yyyy";
                    chart1.Series[chartName].BorderWidth = 3;

                    chart1.ChartAreas[0].AxisX.IsMarginVisible = true;
                    chart1.ChartAreas[0].AxisY.IsMarginVisible = true;
                    chart1.ChartAreas[0].AxisY.IsStartedFromZero = true;

                    if (idx >= 0)
                    {
                        chart1.ChartAreas.Add("C2");
                        chartName = plotData.SecName[idx] + " MTM/TV";
                        chart1.Series.Add(chartName);
                        chart1.Series[chartName].Points.DataBindXY(plotStrategy.Stats.YOYm2v[idx].Select(x => x.Date).ToArray(),
                            plotStrategy.Stats.YOYm2v[idx].Select(x => x.Price).ToArray());
                        chart1.Series[chartName].ChartType = SeriesChartType.Column;
                        chart1.Series[chartName].ChartArea = chart1.ChartAreas[1].Name;
                        chart1.Series[chartName].LabelFormat = "0.000%";
                        chart1.Series[chartName].IsValueShownAsLabel = true;

                        chart1.ChartAreas[1].AxisX.IsMarginVisible = true;
                        chart1.ChartAreas[1].AxisY.IsMarginVisible = true;

                        chart1.ChartAreas[1].AlignWithChartArea = chart1.ChartAreas[0].Name;
                        chart1.ChartAreas[1].AxisY.LabelStyle.Format = "0.000%";

                        chart1.ChartAreas[1].AxisX.MajorGrid.LineColor = Color.Gold;
                        chart1.ChartAreas[1].AxisY.MajorGrid.LineColor = Color.Gold;

                        chart1.ChartAreas[1].AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dash;
                        chart1.ChartAreas[1].AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dash;

                        chart1.ChartAreas[1].AxisY.IsStartedFromZero = true;
                    }
                }
                else if (typeOfPlot == PlotOption.HOH)
                {
                    if (idx < 0)
                        return;
                    string chartName = plotData.SecName[idx] + " Return";

                    chart1.Series.Add(chartName);
                    chart1.Series[chartName].Points.DataBindXY(plotStrategy.Stats.HOH[idx].Select(x => x.Date).ToArray(),
                        plotStrategy.Stats.HOH[idx].Select(x => x.Price).ToArray());
                    chart1.Series[chartName].ChartType = SeriesChartType.Column;
                    chart1.ChartAreas[0].AxisY.LabelStyle.Format = "0.000%";
                    chart1.ChartAreas[0].AxisX.LabelStyle.Format = "HH:mm";
                    
                    chart1.ChartAreas[0].AxisX.IsMarginVisible = true;
                    chart1.ChartAreas[0].AxisY.IsMarginVisible = true;
                    chart1.ChartAreas[0].AxisY.IsStartedFromZero = true;

                    chart1.Series[plotData.SecName[idx] + " Return"].ToolTip = "Date = #VALX \nReturn = #VALY";                    

                }
                else if (typeOfPlot == PlotOption.DD)
                {
                    if (idx < 0)
                        return;

                    chart1.ChartAreas[0].AxisY.LabelStyle.Format = "0.00%";
                    string chartName = plotData.SecName[idx];
                    chart1.Series.Add(chartName);
                    chart1.Series[chartName].Points.DataBindXY(plotStrategy.Stats.DrawDown[idx].Dates,
                        plotStrategy.Stats.DrawDown[idx].Prices.Select(x=>x/100.0).ToArray());
                    chart1.Series[chartName].ChartType = SeriesChartType.Line;
                    chart1.Series[chartName].BorderWidth = 3;

                    foreach (Series series in chart1.Series)
                    {
                        series.IsXValueIndexed = true;
                    }
                }
                else if (typeOfPlot == PlotOption.MTM)
                {
                    if (idx < 0 && plotStrategy.AggStats==null)
                        return;

                    chart1.ChartAreas[0].AxisY.LabelStyle.Format = "0.00";
                    string chartName="";
                    if (idx >= 0)
                    {
                        chartName = plotData.SecName[idx];
                        chart1.Series.Add(chartName);
                        chart1.Series[chartName].Points.DataBindXY(plotStrategy.Stats.MTM[idx].Dates,
                            UF.CummSum(plotStrategy.Stats.MTM[idx].Prices));
                    }
                    else
                    {
                        chartName = "ALL";
                        chart1.Series.Add(chartName);
                        chart1.Series[chartName].Points.DataBindXY(plotStrategy.AggStats.MTM[0].Dates,
                            UF.CummSum(plotStrategy.AggStats.MTM[0].Prices));
                    }                                       
                   
                    chart1.Series[chartName].ChartType = SeriesChartType.Line;
                    chart1.Series[chartName].BorderWidth = 3;

                    foreach (Series series in chart1.Series)
                    {
                        series.IsXValueIndexed = true;
                    }
                }
                else if (typeOfPlot == PlotOption.RETURN_DIST)
                {
                    if (idx < 0)
                        return;

                    chart1.ChartAreas[0].AxisY.LabelStyle.Format = "0";
                    chart1.ChartAreas[0].AxisX.LabelStyle.Format = "0.00%";
                    string chartName = plotData.SecName[idx];
                    chart1.Series.Add(chartName);
                    double[] xAxis;
                    double[] yAxis = UF.Histogram(plotStrategy.Stats.Trades[idx].Select(x => x.Return).ToArray(), numBins, out xAxis);

                    chart1.Series[chartName].Points.DataBindXY(xAxis, yAxis);

                    foreach (DataPoint p in chart1.Series[chartName].Points)
                        p.Color = p.XValue < 0 ? Color.Red : Color.Green;

                    chart1.Series[chartName].ChartType = SeriesChartType.Column;
                    chart1.Series[chartName].BorderWidth = 3;
                }                
            }
        }

        private void chart1_Click(object sender, EventArgs e)
        {

        }        

        private void chart1_AxisViewChanged(object sender, ViewEventArgs e)
        {
            AxisChange(e.Axis.AxisName);
        }        

        private void Form2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.X || e.KeyCode == Keys.Right)
            {
                chart1.ChartAreas[0].AxisX.ScaleView.Scroll(ScrollType.SmallIncrement);
            }
            else if (e.KeyCode == Keys.Z || e.KeyCode == Keys.Left)
            {
                chart1.ChartAreas[0].AxisX.ScaleView.Scroll(ScrollType.SmallDecrement);
            }
            else if (e.KeyCode == Keys.PageDown)
            {
                chart1.ChartAreas[0].AxisX.ScaleView.Scroll(ScrollType.LargeIncrement);
            }
            else if (e.KeyCode == Keys.PageUp)
            {
                chart1.ChartAreas[0].AxisX.ScaleView.Scroll(ScrollType.LargeDecrement);
            }
            else if (e.KeyCode == Keys.Home)
            {
                chart1.ChartAreas[0].AxisX.ScaleView.Scroll(ScrollType.First);
            }
            else if (e.KeyCode == Keys.End)
            {
                chart1.ChartAreas[0].AxisX.ScaleView.Scroll(ScrollType.Last);
            }
            else if (e.KeyCode == Keys.Escape)
            {
               this.Close();
               return;        
            }

            AxisChange(AxisName.X);
        }

        private void AxisChange(AxisName an)
        {
            try
            {
                if (an == AxisName.X && typeOfPlot != PlotOption.SECURITY_TRADES)
                {
                    int start = (int)chart1.ChartAreas[0].AxisX.ScaleView.ViewMinimum;
                    int end = (int)chart1.ChartAreas[0].AxisX.ScaleView.ViewMaximum;

                    double[] temp = chart1.Series[0].Points.Where((x, i) => i >= start && i <= end).Select(x => x.YValues[0]).ToArray();
                    double ymin = temp.Min();
                    double ymax = temp.Max();

                    chart1.ChartAreas[0].AxisY.ScaleView.Position = ymin;
                    chart1.ChartAreas[0].AxisY.ScaleView.Size = ymax - ymin;
                }
                else if (an == AxisName.X && typeOfPlot == PlotOption.SECURITY_TRADES)
                {
                    int start = (int)chart1.ChartAreas[0].AxisX.ScaleView.ViewMinimum;
                    int end = (int)chart1.ChartAreas[0].AxisX.ScaleView.ViewMaximum;

                    double[] temp = chart1.Series[0].Points.Where((x, i) => i >= start && i <= end).Select(x => x.YValues[0]).ToArray();
                    double ymin = temp.Min();
                    double ymax = temp.Max();

                    chart1.ChartAreas[0].AxisY.ScaleView.Position = ymin;
                    chart1.ChartAreas[0].AxisY.ScaleView.Size = ymax - ymin;

                    temp = chart1.Series[1].Points.Where((x, i) => i >= start && i <= end).Select(x => x.YValues[0]).ToArray();
                    ymin = temp.Min();
                    ymax = temp.Max();

                    chart1.ChartAreas[0].AxisY2.ScaleView.Position = ymin;
                    chart1.ChartAreas[0].AxisY2.ScaleView.Size = ymax - ymin;
                }
            }
            catch
            {
            }
        }
    }
}

using App2.SolidWorksPackage.NodeWork;
using ConsoleApp1.SolidWorksPackage.NodeWork;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ConsoleApp1.util
{
    public partial class FormChart : Form
    {
        public FormChart()
        {
            InitializeComponent();
        }
        public FormChart(List<Tuple<ElementArea, Node, double>> distances, IEnumerable<ElementArea> areas) : this()
        {
            foreach (var item in areas)
            {

                var currentAreaDistances = from d in distances
                                           where d.Item1 == item
                                           select d;

                var chart = new System.Windows.Forms.DataVisualization.Charting.Chart();
                chart.Series.Add(new System.Windows.Forms.DataVisualization.Charting.Series());
                chart.ChartAreas.Add(new System.Windows.Forms.DataVisualization.Charting.ChartArea());
                chart.Series[0].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Point;
                chart.Size = new Size(550, 550);

               
                foreach (var dist in currentAreaDistances)
                {
                    chart.Series[0].Points.AddXY(dist.Item2.number, dist.Item3);

                }

                var label = new Label();
                label.Text = $"График области с количеством элементов {item.elements.Count}";

                var flow = new FlowLayoutPanel();
                flow.Controls.Add(label);
                flow.Controls.Add(chart);
                flow.Size = new System.Drawing.Size(600, 600);

                flowLayoutPanel1.Controls.Add(flow);
            }
        }

        private void FormChart_Load(object sender, EventArgs e)
        {

        }
    }
}

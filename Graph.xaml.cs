using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LiveCharts;
using LiveCharts.Wpf;
using Npgsql;
using System.Data;

namespace askue
{
    /// <summary>
    /// Логика взаимодействия для Graph.xaml
    /// </summary> 

    public partial class Graph : Page
    {
        NpgsqlConnection conn;
        NpgsqlDataAdapter adapter;
        DataSet dataset;
        agency agent;

        public Graph(agency a, NpgsqlConnection con, double s)
        {
            InitializeComponent();
            this.agent = a;
            this.conn = con;
            chart.Height = s - 120;

            datepick1.Text = datepick1.DisplayDate.ToString(); // rewrite
            datepick2.Text = datepick2.DisplayDate.AddDays(3).ToString(); // rewrite4

            adapter = new NpgsqlDataAdapter("select date_trunc('day',( select min(time) from value_elec where id_counter = " + agent.counterid + "))", conn);
            datepick1.DisplayDateStart = (DateTime)adapter.SelectCommand.ExecuteScalar();
            datepick2.DisplayDateStart = datepick1.DisplayDateStart;
            adapter = new NpgsqlDataAdapter("select date_trunc('day',( select max(time) from value_elec where id_counter = " + agent.counterid + "))", conn);
            datepick1.DisplayDateEnd = (DateTime)adapter.SelectCommand.ExecuteScalar();
            datepick2.DisplayDateEnd = datepick1.DisplayDateEnd;

            chart.Visibility = Visibility.Hidden;
        }

        public void refresh(string select_string)
        {
            List<bool> l = new List<bool>();
            foreach (CheckBox cb in comboBox1.Items)
            {
                if (cb.IsChecked == true)
                    l.Add(true);
                else
                    l.Add(false);
            }

            List<LineSeries> ls = new List<LineSeries>();
            Axis ax = new Axis() { Separator = new LiveCharts.Wpf.Separator() };
            ax.Labels = new List<string>();

            adapter = new NpgsqlDataAdapter(select_string, conn);
            dataset = new DataSet();
            adapter.Fill(dataset);

            #region ls.add
            ls.Add(new LineSeries
            {
                DataLabels = false,
                Values = new ChartValues<double>(),
                LabelPoint = Point => Point.Y.ToString(),
                LineSmoothness = 1,
                PointGeometrySize = 4,
                Title = "Показания прибора учета, вт/ч",
                ScalesYAt = 0
            });
            ls.Add(new LineSeries
            {
                DataLabels = false,
                Values = new ChartValues<double>(),
                LabelPoint = Point => Point.Y.ToString(),
                LineSmoothness = 1, //0: straight lines, 1: really smooth lines
                PointGeometrySize = 4,
                Title = "Полная мощность, ВА",
            });
            ls.Add(new LineSeries
            {
                DataLabels = false,
                Values = new ChartValues<double>(),
                LabelPoint = Point => Point.Y.ToString(),
                LineSmoothness = 1,
                PointGeometrySize = 4,
                Title = "Активная мощность, Вт"
            });
            ls.Add(new LineSeries
            {
                DataLabels = false,
                Values = new ChartValues<double>(),
                LabelPoint = Point => Point.Y.ToString(),
                LineSmoothness = 1,
                PointGeometrySize = 4,
                Title = "Реактивная мощность, вар"
            });
            ls.Add(new LineSeries
            {
                DataLabels = false,
                Values = new ChartValues<double>(),
                LabelPoint = Point => Point.Y.ToString(),
                LineSmoothness = 1,
                PointGeometrySize = 4,
                Title = "Напряжение, В"
            });
            ls.Add(new LineSeries
            {
                DataLabels = false,
                Values = new ChartValues<double>(),
                LabelPoint = Point => Point.Y.ToString(),
                LineSmoothness = 1,
                PointGeometrySize = 4,
                Title = "Сила тока, А"
            });
            ls.Add(new LineSeries
            {
                DataLabels = false,
                Values = new ChartValues<double>(),
                LabelPoint = Point => Point.Y.ToString(),
                LineSmoothness = 1,
                PointGeometrySize = 4,
                Title = "cosf"
            });
            ls.Add(new LineSeries
            {
                DataLabels = false,
                Values = new ChartValues<double>(),
                LabelPoint = Point => Point.Y.ToString(),
                LineSmoothness = 1,
                PointGeometrySize = 4,
                Title = "Частота, Гц"
            });
            #endregion

            for (int i = 0; i < dataset.Tables[0].Columns[0].Table.Rows.Count; i++)
            {
                if (l[0])
                    ls[0].Values.Add(Convert.ToDouble(dataset.Tables[0].Columns[0].Table.Rows[i].ItemArray[0]));
                if (l[1])
                    ls[1].Values.Add(Convert.ToDouble(dataset.Tables[0].Columns[0].Table.Rows[i].ItemArray[1]));
                if (l[2])
                    ls[2].Values.Add(Convert.ToDouble(dataset.Tables[0].Columns[0].Table.Rows[i].ItemArray[2]));
                if (l[3])
                    ls[3].Values.Add(Convert.ToDouble(dataset.Tables[0].Columns[0].Table.Rows[i].ItemArray[3]));
                if (l[4])
                    ls[4].Values.Add(Convert.ToDouble(dataset.Tables[0].Columns[0].Table.Rows[i].ItemArray[4]));
                if (l[5])
                    ls[5].Values.Add(Convert.ToDouble(dataset.Tables[0].Columns[0].Table.Rows[i].ItemArray[5]));
                if (l[6])
                    ls[6].Values.Add(Convert.ToDouble(dataset.Tables[0].Columns[0].Table.Rows[i].ItemArray[6]));
                if (l[7])
                    ls[7].Values.Add(Convert.ToDouble(dataset.Tables[0].Columns[0].Table.Rows[i].ItemArray[7]));
                ax.Labels.Add(Convert.ToString(dataset.Tables[0].Columns[0].Table.Rows[i].ItemArray[8]));
            }

            chart.Series.Clear();
            chart.AxisX.Clear();
            chart.AxisY.Clear();
            chart.Series.AddRange(ls);
            chart.AxisX.Add(ax);
            chart.AxisY.Add(new Axis()
            {
                LabelFormatter = value => value.ToString(),
                Separator = new LiveCharts.Wpf.Separator(),
                MinValue = 0
            });
        }


        private void filterButton_Click(object sender, RoutedEventArgs e)
        {
            bool t = false;
            foreach (CheckBox cb in comboBox1.Items)
                if (cb.IsChecked == true)
                    t = true;
            if (t)
            {
                if (datepick1.SelectedDate <= datepick2.SelectedDate)
                {
                    chart.Visibility = Visibility.Visible;

                    if (MonthRButton.IsChecked == true)
                        refresh(String.Format("select * from getAvgMonth({2}, '{0}', '{1}')", datepick1.SelectedDate.Value, datepick2.SelectedDate.Value, agent.counterid));

                    if (DayRButton.IsChecked == true)
                        refresh(String.Format("select * from getAvgDay({2}, '{0}', '{1}')", datepick1.SelectedDate.Value, datepick2.SelectedDate.Value, agent.counterid));

                    if (HourRButton.IsChecked == true)
                        refresh(String.Format("select v.value, v.full, active, reactive, voltage, amperage, angle, frequence, time from value_elec v " +
                    "where time > '{0}' AND time < '{1}' AND id_counter = {2}", datepick1.SelectedDate.Value, datepick2.SelectedDate.Value, agent.counterid));
                }
                else
                    MessageBox.Show("Начальная дата должна быть меньше конечной");

            }
            else
                MessageBox.Show("Выберите фильтр");

        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            comboBox1.SelectedIndex = -1;
        }
    }
}

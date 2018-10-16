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
using Npgsql;
using System.Data;

namespace askue
{
    /// <summary>
    /// Логика взаимодействия для Adress.xaml
    /// </summary>
    public partial class Adress : Page
    {
        NpgsqlConnection conn;
        NpgsqlDataAdapter adapter;
        DataSet dataset;
        int table;
        Frame far;
        agency agent;
        DateTime t1;
        double size;

        public Adress()
        {
            InitializeComponent();
        }

        public Adress(int select, NpgsqlConnection con, Frame f, agency a, double s)
        {
            InitializeComponent();
            DB.IsReadOnly = true;
            table = select;
            conn = con;
            if (select == 0)
                DB.ContextMenu.Visibility = Visibility.Hidden;
            else
                DB.ContextMenu.Visibility = Visibility.Visible;

            far = f;
            this.agent = a;
            size = s;
        }

        public void setting_DB()
        {
            DB.CanUserAddRows = false;
            DB.SelectionMode = DataGridSelectionMode.Extended;
            DB.SelectionUnit = DataGridSelectionUnit.FullRow;
            DB.CanUserDeleteRows = false;
            DB.CanUserReorderColumns = false;
            DB.CanUserSortColumns = true;
            if (table == 2)
                DB.CanUserSortColumns = false;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (conn != null)
            {
                setting_DB();
                switch (table)
                {
                    case 0:
                        adapter = new NpgsqlDataAdapter("select service as \"Обслуживающая организация\", agency.name as \"Название\"," +
    " street as \"Улица\", house as \"Дом\" from agency", conn);
                        break;
                    case 1:
                        t1 = DateTime.Now;
                        t1 = t1.AddMinutes(-(t1.Minute));
                        t1 = t1.AddSeconds(-(t1.Second));

                        adapter = new NpgsqlDataAdapter(String.Format("select street as \"Улица\", house as \"Дом\", a.name as \"Наименование\", c.name as \"Уточнение\", " + 
                            "name_counter as \"Счетчик\", c.type as \"Тип счетчика\", date_verification as \"Дата поверки\", v.value as \"Текущие показания, ВТч\" " +
                            "from counter c, value_elec v, agency a where a.id = c.id_agency AND c.id = v.id_counter AND v.time > '{0}' AND v.time < '{1}'", t1.ToString(), t1.AddHours(1).ToString()), conn);
                        break;
                    case 2:
                        adapter = new NpgsqlDataAdapter("select v.time, v.value, v.full as \"Полная\nмощность, ВА\", active as \"Активная\nмощность, Вт\", " + 
                            "reactive as \"Реактивная\nмощность, вар\", voltage as \"Напряжение, В\", amperage as \"Сила тока, А\", angle AS \"cosf\", frequence AS \"Частота, Гц\" "+
                            "from value_elec v where id_counter = "+agent.counterid, conn);
                        break;
                    default:
                        adapter = new NpgsqlDataAdapter("select service as \"Обслуживающая организация\", agency.name as \"Название\"," +
                " street as \"Улица\", house as \"Дом\" from agency", conn);
                        break;
                }

                dataset = new DataSet();
                adapter.Fill(dataset);
                DB.ItemsSource = dataset.Tables[0].AsDataView();
                if (table == 2)
                {
                    DB.Columns[0].Header = "Дата / Время";
                    DB.Columns[1].Header = "Текущее значение, ВТ/ч";
                }
            }
        }

        public int getSelect()
        {
            return table;
        }

        private void DB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataRowView row;
            switch (table)
            {
                case 0:
                    if (DB.SelectedItems.Count > 0)
                    {
                        row = (DataRowView)DB.SelectedItems[0];
                        adapter = new NpgsqlDataAdapter(String.Format("select id from agency where agency.name = '{0}' AND street = '{1}' AND house = '{2}'",
                            row.Row.ItemArray[1], row.Row.ItemArray[2], row.Row.ItemArray[3]), conn);
                        agent.agencyid = (int)adapter.SelectCommand.ExecuteScalar();
                        t1 = DateTime.Now;
                        t1 = t1.AddMinutes(-(t1.Minute));
                        t1 = t1.AddSeconds(-(t1.Second));

                        adapter = new NpgsqlDataAdapter(String.Format("select street as \"Улица\", house as \"Дом\", a.name as \"Наименование\", c.name as \"Уточнение\", " +
                            "name_counter as \"Счетчик\", c.type as \"Тип счетчика\", date_verification as \"Дата поверки\", v.value as \"Текущие показания, ВТч\" " +
                            "from counter c, value_elec v, agency a where a.id = c.id_agency AND c.id = v.id_counter AND a.id = {2} AND v.time > '{0}' AND v.time < '{1}'", 
                            t1.ToString(), t1.AddHours(1).ToString(), agent.agencyid), conn);
                        dataset = new DataSet();
                        adapter.Fill(dataset);
                        DB.ItemsSource = dataset.Tables[0].DefaultView;
                        table = 1;
                        setting_DB();
                        DB.ContextMenu.Visibility = Visibility.Visible;
                    }
                    break;
                case 1:
                    if (DB.SelectedItems.Count > 0)
                    {
                        row = (DataRowView)DB.SelectedItems[0];
                        if (row.Row.ItemArray.Length > 5)
                        {
                            adapter = new NpgsqlDataAdapter(String.Format("select counter.id from counter, agency where street = '{0}' " +
                                "AND house = '{1}' AND agency.name = '{2}' AND counter.name = '{3}' AND name_counter = '{4}' AND " +
                                "type = '{5}' AND date_verification = '{6}'",
                                row.Row.ItemArray[0], row.Row.ItemArray[1], row.Row.ItemArray[2], row.Row.ItemArray[3], row.Row.ItemArray[4], row.Row.ItemArray[5], row.Row.ItemArray[6]), conn);
                            agent.counterid = (int)adapter.SelectCommand.ExecuteScalar();
                        }
                    }
                    break;
                case 2:
                    break;
                default:
                    adapter = new NpgsqlDataAdapter("select service as \"Обслуживающая организация\", agency.name as \"Название\"," +
            " street as \"Улица\", house as \"Дом\" from agency", conn);
                    break;
            }
        }

        private void cartHeader_Click(object sender, RoutedEventArgs e)
        {
            t1 = DateTime.Now;
            t1 = t1.AddMinutes(-(t1.Minute));
            t1 = t1.AddSeconds(-(t1.Second));

            adapter = new NpgsqlDataAdapter(String.Format("select name_counter as \"Счетчик\", counter.type as \"Тип счетчика\", date_verification as \"Дата поверки\", "+
                "value_elec.value as \"Текущие показания, кВТч\" from counter, value_elec where counter.id = {0} AND counter.id = value_elec.id_counter AND " + 
                "value_elec.time > '{1}' AND value_elec.time < '{2}'",agent.counterid, t1.ToString(), t1.AddHours(1).ToString()), conn);
            dataset = new DataSet();
            adapter.Fill(dataset);
            DB.ItemsSource = dataset.Tables[0].DefaultView;
        }

        private void graphHeader_Click(object sender, RoutedEventArgs e)
        {
            Graph gp = new Graph(agent, conn, size);
            far.Content = gp;
        }
    }
}

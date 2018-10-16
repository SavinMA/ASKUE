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
using LiveCharts;
using LiveCharts.Wpf;
using System.IO;

namespace askue
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public int id_counter;
        public int id_address;
        public int select;
        public NpgsqlConnection conn;
        public agency agent;

        public MainWindow()
        {
            InitializeComponent();
            id_counter = -1;
        }

        private void ButtonLogOut_Click(object sender, RoutedEventArgs e)
        {
            conn.Close();
            Application.Current.Shutdown();
        }

        private void ButtonCloseMenu_Click(object sender, RoutedEventArgs e)
        {
            ButtonCloseMenu.Visibility = Visibility.Collapsed;
            ButtonOpenMenu.Visibility = Visibility.Visible;
        }

        private void ButtonOpenMenu_Click(object sender, RoutedEventArgs e)
        {
            ButtonOpenMenu.Visibility = Visibility.Collapsed;
            ButtonCloseMenu.Visibility = Visibility.Visible;
        }

        private void mainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            conn = new NpgsqlConnection(File.ReadAllText("connection.ini"));
            conn.Open();
            agent = new agency();
        }

        private void ButtonHidden_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.WindowState = WindowState.Minimized;
        }

        private void ItemObject_MouseUp(object sender, MouseButtonEventArgs e)
        {
            select = 0;
            Adress address = new Adress(0, conn, MainFrame, agent, this.Height);
            MainFrame.Content = address;
            select = address.getSelect();

            topTextBlock.Text = "Объекты учета";
            //TextBlockBottom.Text = "Загружена таблица с адресами";
        }

        private void ItemPoint_MouseUp(object sender, MouseButtonEventArgs e)
        {
            select = 1;
            Adress address = new Adress(select, conn, MainFrame, agent, this.Height);
            MainFrame.Content = address;
            select = address.getSelect();

            topTextBlock.Text = "Точки учета";
            //TextBlockBottom.Text = "Загружена таблица с адресами";
        }

        private void GraphItem_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (agent.counterid != -1)
            {
                topTextBlock.Text = "График";
                Graph graph = new Graph(agent, conn, this.Height);
                MainFrame.Content = graph;
            }
            else
                MessageBox.Show("Чтобы просмотреть график выделите нужную точку учета или используйте ПКМ");
        }

        private void ItemTable_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (agent.counterid != -1)
            {
                topTextBlock.Text = "Таблица";
                select = 2;
                Adress address = new Adress(select, conn, MainFrame, agent, this.Height);
                MainFrame.Content = address;
                select = address.getSelect();
            }
            else
                MessageBox.Show("Чтобы просмотреть таблицу выделите нужную точку учета или используйте ПКМ");
        }
    }
}

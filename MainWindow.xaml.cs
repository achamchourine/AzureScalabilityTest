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

namespace AzureScalabilityTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public class Configuration
    {
        public int NumberOfThreads { get; set; }
        public string ConnectionString { get; set; }

        public Configuration()
        {
            NumberOfThreads = 0;
            ConnectionString = @"Server=tcp:<server>.database.windows.net,1433;Initial Catalog=<db>;Persist Security Info=False;User ID=<user>;Password=<password>;MultipleActiveResultSets=False;Connection Timeout=30;";
        }
    }

    public partial class MainWindow : Window
    {
        public Configuration Config;
        public ThreadController controller;

        public MainWindow()
        {
            InitializeComponent();
    
            Config = new Configuration();

            DataContext = Config;

            controller = new ThreadController();
        }

        private void btnAddThread_Click(object sender, RoutedEventArgs e)
        {
            controller.Add10Tasks(Config.ConnectionString);

            DataContext = null;
            Config.NumberOfThreads = controller.NumberOfTasks;
            DataContext = Config;

        }

        private void btnDelThread_Click(object sender, RoutedEventArgs e)
        {
            controller.Cancel10Tasks();

            System.Threading.Thread.Sleep(500);

            DataContext = null;
            Config.NumberOfThreads = controller.NumberOfTasks;
            DataContext = Config;

        }

        private void btnStopAll_Click(object sender, RoutedEventArgs e)
        {
            controller.CancelAllTasks();

            System.Threading.Thread.Sleep(500);

            DataContext = null;
            Config.NumberOfThreads = controller.NumberOfTasks;
            DataContext = Config;
        }

    }
}

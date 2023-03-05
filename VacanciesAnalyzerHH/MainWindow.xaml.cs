using System;
using System.Windows;

namespace VacanciesAnalyzerHH
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainViewModel = new MainViewModel();
            DataContext = MainViewModel;
        }

        public MainViewModel MainViewModel { get; set; }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MainViewModel?.Search();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ConvertSalaries(object sender, RoutedEventArgs e)
        {
            MainViewModel?.ConvertSalaries();
        }
    }
}

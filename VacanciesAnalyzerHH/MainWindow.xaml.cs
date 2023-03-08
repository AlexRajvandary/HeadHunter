using System;
using System.Windows;
using System.Windows.Data;
using VacanciesAnalyzerHH.Models;

namespace VacanciesAnalyzerHH
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private CollectionViewSource collectionViewSource;

        public MainWindow()
        {
            InitializeComponent();
            collectionViewSource = (CollectionViewSource)TryFindResource("VacanciesCollectionViewSource");
            MainViewModel = new MainViewModel();
            DataContext = MainViewModel;
            DataFilter = new DataFilter();
            DataFilter.PropertyChanged += FilterUpdated;
        }

        public DataFilter DataFilter { get; set; }

        public MainViewModel MainViewModel { get; set; }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await MainViewModel?.Search();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ConvertSalaries(object sender, RoutedEventArgs e)
        {
            MainViewModel?.ConvertSalaries();
        }

        private void FilterUpdated(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            collectionViewSource.View.Filter = (object o) => DataFilter.Filter(o as Vacancy);
        }
    }
}

using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace TestListBinding
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public ObservableCollection<MainModel> Models { get; }

        public ObservableCollection<MainModel> SelectedItems { get; }
        
        public static DependencyProperty SelectedModelProperty =
            DependencyProperty.Register(nameof(SelectedModel), typeof(MainModel), typeof(MainWindow));
        public MainModel SelectedModel { get => (MainModel)GetValue(SelectedModelProperty); set => SetValue(SelectedModelProperty, value); }

        public SelectionChangedCommand SelectionChangedCommand { get; }

        public MainWindow()
        {
            Models = new ObservableCollection<MainModel>();
            SelectedItems = new ObservableCollection<MainModel>();
            SelectionChangedCommand = new SelectionChangedCommand(SelectedItems);

            var rnd = new Random();
            for(int i = 0; i < 50; i++)
            {
                Models.Add(new MainModel 
                { 
                    Name = "Item " + i,
                    Value1 = 1,
                    Value2 = i,
                    Value3 = rnd.Next(20),
                    Value4 = rnd.Next(20),
                    Value5 = rnd.Next(20),
                    Value6 = rnd.Next(20),
                    Value7 = rnd.Next(20),
                    Value8 = rnd.Next(20),
                    Value9 = rnd.Next(20),
                });
            }

            InitializeComponent();
        }
    }
}

using System.Windows;
using Application.WPF.ViewModel;

namespace Application.WPF.View
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            ViewModelManager.GetEmployeesView();
        }
    }
}
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
using TestTask.ViewModel;

namespace TestTask
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = StudentViewModel.SharedViewModel();
            this.Loaded += MainWindow_Loaded;
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var errorsAdding = GrdStudent.GetValue(Validation.ErrorsProperty);
          //  var errorsUpdating = UpdStudent.GetValue(Validation.ErrorsProperty);
        }

        private void Validation_Error(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added)
                StudentViewModel.Errors += 1;
            if (e.Action == ValidationErrorEventAction.Removed) StudentViewModel.Errors -= 1;
        }

        private void Validation_Error_Updating(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added) StudentViewModel.ErrorsUpd += 1;
            if (e.Action == ValidationErrorEventAction.Removed) StudentViewModel.ErrorsUpd -= 1;
        }
    }
}

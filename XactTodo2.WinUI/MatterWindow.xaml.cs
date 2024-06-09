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
using System.Windows.Shapes;
using XactTodo.WinUI.Models;
using XactTodo.WinUI.Utils;

namespace XactTodo.WinUI
{
    /// <summary>
    /// MatterWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MatterWindow : Window
    {

        public MatterWindow()
        {
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async void btnOkay_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var vm = this.DataContext as MatterViewModel;
                if (vm != null)
                    await vm.SaveCommand.ExecuteAsync(sender);
                this.DialogResult = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "异常", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public static Matter? AddMatter()
        {
            var window = new MatterWindow
            {
                Owner = Application.Current.MainWindow
            };
            if (window.ShowDialog()??false)
            {
                return (window.DataContext as MatterViewModel)?.Matter;
            }
            else
                return null;
        }

        internal static void ShowMatter(Matter matter)
        {
            var window = new MatterWindow
            {
                Owner = Application.Current.MainWindow,
                DataContext = new MatterViewModel(matter),
            };
            window.ShowDialog();
        }

    }

}

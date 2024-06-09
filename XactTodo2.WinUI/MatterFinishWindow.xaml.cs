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

namespace XactTodo.WinUI
{
    /// <summary>
    /// MatterFinishWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MatterFinishWindow : Window
    {
        public MatterFinishWindow()
        {
            InitializeComponent();
        }

        public static MatterFinishInfo? ConfirmFinish(MatterOutline matter)
        {
            var matterFinishInfo = new MatterFinishInfo
            {
                Subject = matter.Subject,
                FinishTime = DateTime.Now
            };
            var window = new MatterFinishWindow
            {
                Owner = Application.Current.MainWindow,
                DataContext = new MatterFinishViewModel(matterFinishInfo),
            };
            if (window.ShowDialog() ?? false)
            {
                return (window.DataContext as MatterFinishViewModel)?.MatterFinishInfo;
            }
            else
                return null;
        }

        private void btnOkay_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var vm = this.DataContext as MatterFinishViewModel;
                if (vm != null)
                {
                    vm.MatterFinishInfo.FinishTime = vm.MatterFinishInfo.FinishTime.AddHours((int)numHour.Value - vm.MatterFinishInfo.FinishTime.Hour)
                        .AddMinutes((int)numMinute.Value - vm.MatterFinishInfo.FinishTime.Minute);
                }
                this.DialogResult = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "异常", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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
    /// SettingWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SettingWindow : Window
    {
        public SettingWindow()
        {
            InitializeComponent();
        }

        internal static void Show()
        {
            var window = new SettingWindow
            {
                Owner = Application.Current.MainWindow,
            };
            window.ShowDialog();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = new SettingViewModel();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async void btnValidate_Click(object sender, RoutedEventArgs e)
        {
            const string url_login = "api/Login";
            var client = Utils.HttpClientFactory.CreateClient();
            var jsonLogin = JsonConvert.SerializeObject(new { UserName=txtUserName.Text, Password=txtPassword.Password});
            var response = await client.PostAsync(url_login, new StringContent(jsonLogin, Encoding.UTF8, "application/json"));
            response.EnsureSuccessStatusCode(); // 有错误码时报出异常
            var content = await response.Content.ReadAsStringAsync();
            var loginResult = JsonConvert.DeserializeObject<LoginResult>(content);
            if (loginResult.ResultType != 1)
            {
                MessageBox.Show("登录验证失败！" + loginResult.ErrorMessage, "错误", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                (App.Current as App).LoginResult = loginResult;
                MessageBox.Show($"登录验证成功，当前用户已切换为\"{loginResult.UserName}\"！", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}

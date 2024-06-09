using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using XactTodo.WinUI.Models;
using XactTodo.WinUI.Utils;

namespace XactTodo.WinUI
{
    public partial class MainViewModel
    {
        private readonly HttpClient client;
        private bool loading;

        public MainViewModel()
        {
            this.Matters = new ObservableCollection<MatterOutline>();
            try
            {
                client = Utils.HttpClientFactory.CreateClient();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "警告", MessageBoxButton.OK, MessageBoxImage.Information);
            }

        }

        public ObservableCollection<MatterOutline> Matters { get; }

        private bool IsMainWindowLoaded()
        {
            return Application.Current.MainWindow != null && Application.Current.MainWindow.IsLoaded;
        }

        //[RelayCommand(CanExecute = nameof(IsMainWindowLoaded))]
        [RelayCommand]
        private void ShowSettings()
        {
            SettingWindow.Show();
        }

        [RelayCommand]
        private void AddMatter()
        {
            var matter = MatterWindow.AddMatter();
        }

        [RelayCommand]
        private void ShowMatter(object sender)
        {
            if (sender == null)
                return;
            var id = (((ListBoxItem)sender).Content as MatterOutline)?.Id;
            var url = $"api/Matter/{id}";
            var response = client.GetAsync(url).Result;
            var s = response.Content.ReadAsStringAsync().Result;
            response.EnsureSuccessStatusCode(); // 有错误码时报出异常
            var matter = JsonConvert.DeserializeObject<Matter>(s);
            if (matter == null)
            {
                MessageBox.Show("反序列化Matter对象失败！" + s);
            }
            else
            {
                MatterWindow.ShowMatter(matter);
            }
        }

        private bool Login()
        {
            if(string.IsNullOrEmpty(AppSettings.Instance.UserName) || string.IsNullOrEmpty(AppSettings.Instance.Password))
            {
                MessageBox.Show("请在用户设置对话框中设置用户名及密码！");
                return false;
            }
            const string url_login = "api/Login";
            var client = Utils.HttpClientFactory.CreateClient();
            var jsonLogin = JsonConvert.SerializeObject(new { AppSettings.Instance.UserName, AppSettings.Instance.Password });
            var response = client.PostAsync(url_login, new StringContent(jsonLogin, Encoding.UTF8, "application/json")).Result;
            response.EnsureSuccessStatusCode(); // 有错误码时报出异常
            var content = response.Content.ReadAsStringAsync().Result;
            var loginResult = JsonConvert.DeserializeObject<LoginResult>(content);
            if (loginResult.ResultType != 1)
            {
                MessageBox.Show($"登录失败：{loginResult.ErrorMessage}\n请在用户设置对话框中设置正确的用户名及密码！", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                ShowSettings();
                return false;
            }
            (App.Current as App).LoginResult = loginResult;
            return true;
        }

        [RelayCommand]
        private async void ReloadMatters()
        {
            if (loading || !IsMainWindowLoaded()) return;
            try
            {
                if((App.Current as App).LoginResult==null && !Login())
                {
                    return;
                }
                loading = true;
                const string url_unfinished = "api/Matter/unfinished";
                var response = await client.GetAsync(url_unfinished);
                //response.EnsureSuccessStatusCode(); // 有错误码时报出异常
                if (!response.IsSuccessStatusCode) //调用下面的方法，显示更详细的错误信息
                {
                    response.ShowHttpRequestException();
                    return;
                }
                var s = await response.Content.ReadAsStringAsync();
                var matters = JsonConvert.DeserializeObject<IEnumerable<MatterOutline>>(s);
                //var jo = (JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(rlt.Content.ReadAsStringAsync().Result);
                //var matters = JsonConvert.DeserializeObject<IEnumerable<Matter>>(rlt.Content.ReadAsStringAsync().Result);
                this.Matters.Clear();
                foreach(var matter in matters)
                {
                    this.Matters.Add(matter);
                }
            }
            catch (JsonException jEx)
            {
                // 这个异常指明了一个解序列化请求体的问题。
                MessageBox.Show(jEx.Message);
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show(ex.AllMessages(), "异常", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                loading = false;
            }
        }

        [RelayCommand]
        private void MatterFinished(object sender)
        {
            var lstMatters = (Application.Current.MainWindow as MainWindow).lstMatters;
            if (!(((ListBoxItem)lstMatters.ContainerFromElement((Control)sender)).Content is MatterOutline matter))
                return;
            var matterFinishInfo = MatterFinishWindow.ConfirmFinish(matter);
            if (matterFinishInfo == null)
                return;
            var url_unfinished = $"api/Matter/{matter.Id}/Finish";
            var json = JsonConvert.SerializeObject(new
            {
                matterFinishInfo.FinishTime,
                matterFinishInfo.Comment,
            });
            var response = client.PostAsync(url_unfinished, new StringContent(json, Encoding.UTF8, "application/json")).Result;
            response.EnsureSuccessStatusCode();
            this.Matters.Remove(matter);
        }

    }
}
using Newtonsoft.Json;
using System.Configuration;
using System.Data;
using System.Windows;
using XactTodo.WinUI.Models;
using XactTodo.WinUI.Utils;

namespace XactTodo.WinUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private LoginResult loginResult;

        public LoginResult LoginResult
        {
            get { return loginResult; }
            set
            {
                loginResult = value;
                HttpClientFactory.Authorization = loginResult?.Token?.AccessToken;
                var response = HttpClientFactory.CreateClient().GetAsync("/api/user/currentuser").Result;
                var s = response.Content.ReadAsStringAsync().Result;
                var user = JsonConvert.DeserializeObject<UserOutline>(s);
                CurrentUser = user;
            }
        }

        /// <summary>
        /// 当前登录用户
        /// </summary>
        public UserOutline CurrentUser { get; private set; }

    }

}

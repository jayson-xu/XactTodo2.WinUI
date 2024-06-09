using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using XactTodo.WinUI.Exceptions;

namespace XactTodo.WinUI.Utils
{
    public static class HttpClientFactory
    {
        private const string KEY_AUTHORIZATION = "authorization";
        private static string authorization;
        private static HttpClient client;
        private static AppSettings AppSettings => AppSettings.Instance;

        static HttpClientFactory()
        {
        }

        public static HttpClient CreateClient()
        {
            if (client == null)
            {
                //初始化HttpClient实例变量
                client = new HttpClient();
                if (string.IsNullOrEmpty(AppSettings.RootUrl_Api))
                {
                    throw new MissingSettingException("缺少应用程序配置项[API服务器地址]，请在系统设置中设定该配置项。");
                }
                var baseAddress = AppSettings.RootUrl_Api ?? "";
                if (!baseAddress.EndsWith("/"))
                    baseAddress += "/";
                client.BaseAddress = new Uri(baseAddress);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                if (!string.IsNullOrWhiteSpace(authorization))
                    client.DefaultRequestHeaders.Add(KEY_AUTHORIZATION, authorization);
            }
            return client;
        }

        public static string Authorization
        {
            get => authorization;
            set
            {
                if (authorization == value) return;
                authorization = value;
                if (client != null)
                {
                    if (client.DefaultRequestHeaders.Contains(KEY_AUTHORIZATION))
                        client.DefaultRequestHeaders.Remove(KEY_AUTHORIZATION);
                    if (!string.IsNullOrWhiteSpace(authorization))
                        client.DefaultRequestHeaders.Add(KEY_AUTHORIZATION, authorization);
                }
            }
        }
    }
}

using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Windows;

namespace XactTodo.WinUI.Utils
{
    /// <summary>
    /// 异常处理辅助类
    /// </summary>
    public static class ExceptionHelper
    {
        /// <summary>
        /// 遍历Exception.InnerException,获取全部异常消息。
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public static string AllMessages(this Exception exception)
        {
            StringBuilder sb = new StringBuilder();
            var ex = exception;
            while (ex != null)
            {
                string message = ex.Message;
                sb.Append(message).Append(Environment.NewLine);
                ex = ex.InnerException;
            }
            return sb.ToString();
        }

        public static void ShowHttpRequestException(this HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
                return;
            var content = response.Content.ReadAsStringAsync().Result;
            if (!string.IsNullOrEmpty(content))
            {
                if (content.StartsWith("{") && content.EndsWith("}"))
                {
                    try
                    {
                        var errorObj = JsonConvert.DeserializeObject<JsonErrorResponse>(content);
                        content = errorObj.Message;
                    }
                    catch { }
                }
            }
            MessageBox.Show("后台返回：\n" + content, "网络请求异常", MessageBoxButton.OK, MessageBoxImage.Error);
        }

    }
}

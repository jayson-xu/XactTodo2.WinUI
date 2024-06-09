using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XactTodo.WinUI.Models
{
    /// <summary>
    /// 登录令牌
    /// </summary>
    public class Token
    {
        /// <summary>
        /// 访问令牌
        /// </summary>
        //[Newtonsoft.Json.JsonProperty("access_token")]
        public string AccessToken { get; set; }

        /// <summary>
        /// 令牌类型(默认为Bearer)
        /// </summary>
        //[Newtonsoft.Json.JsonProperty("token_type")]
        public string TokenType { get; set; } = "bearer";

        /// <summary>
        /// 令牌有效时长(秒)
        /// </summary>
        //[Newtonsoft.Json.JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }

        /// <summary>
        /// 令牌颁发时间
        /// </summary>
        //[Newtonsoft.Json.JsonProperty("issue_time")]
        public DateTime IssueTime { get; set; }

        /// <summary>
        /// 刷新令牌
        /// </summary>
        //[Newtonsoft.Json.JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }

        /// <summary>
        /// 令牌失效时间
        /// </summary>
        //[Newtonsoft.Json.JsonProperty("expire_time")]
        public DateTime ExpireTime
        {
            get { return IssueTime.AddSeconds(ExpiresIn); }
        }

    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XactTodo.WinUI.Models
{
    /// <summary>
    /// 登录结果
    /// </summary>
    public class LoginResult
    {
        /// <summary>
        /// 结果类型<see cref="LoginResultType"/> 
        /// </summary>
        public int ResultType { get; set; }

        /// <summary>
        /// 登录或验证失败时，可通过此属性向前台反馈更明确的错误信息
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 用户账号
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        public string Nickname { get; set; }

        /// <summary>
        /// 令牌
        /// </summary>
        public Token Token { get; set; }

    }
}

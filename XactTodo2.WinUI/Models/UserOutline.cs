using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XactTodo.WinUI.Models
{
    public class UserOutline
    {
            public int Id { get; set; }

            /// <summary>
            /// 账号
            /// </summary>
            public string UserName { get; set; }

            public string DisplayName { get; set; }

            public string Email { get; set; }

            public bool EmailConfirmed { get; set; }

            public new int? CreatorUserId { get; set; }
        }
    }

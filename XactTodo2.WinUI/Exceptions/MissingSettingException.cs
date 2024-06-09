using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XactTodo.WinUI.Exceptions
{
    public class MissingSettingException : Exception
    {
        public MissingSettingException()
        {
        }

        public MissingSettingException(string message) : base(message)
        {
        }
    }
}

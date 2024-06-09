using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XactTodo.WinUI
{
    public class MatterFinishViewModel
    {
        public MatterFinishInfo MatterFinishInfo { get; }

        public MatterFinishViewModel()
        {
            this.MatterFinishInfo = new MatterFinishInfo();
        }

        public MatterFinishViewModel(MatterFinishInfo matterFinishInfo)
        {
            this.MatterFinishInfo = matterFinishInfo;
        }

    }

    public class MatterFinishInfo
    {
        public string Subject { get; set; }

        public DateTime FinishTime { get; set; }

        public string Comment { get; set; }

        public int HourOfFinishTime
        {
            get
            {
                return FinishTime.Hour;
            }
            set
            {
                FinishTime = FinishTime.AddHours(value - FinishTime.Hour);
            }
        }

        public int MinuteOfFinishTime
        {
            get
            {
                return FinishTime.Minute;
            }
            set
            {
                FinishTime = FinishTime.AddMinutes(value - FinishTime.Minute);
            }
        }
    }

}

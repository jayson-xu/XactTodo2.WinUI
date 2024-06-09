using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XactTodo.WinUI.Models
{
    public class Matter : INotifyPropertyChanged
    {
        public Matter() : this("")
        {
        }

        public Matter(string subject="", string content="")
        {
            Subject = subject;
            Content = content;
            CreationTime = DateTime.Now;
        }

        public int Id { get; set; }

        /// <summary>
        /// 主题
        /// </summary>
        public string Subject
        {
            get => subject;
            set
            {
                subject = value;
                NotifyPropertyChanged(nameof(Subject));
            }
        }
        private string subject;

        /// <summary>
        /// 内容
        /// </summary>
        public string Content
        {
            get => content;
            set
            {
                content = value;
                NotifyPropertyChanged(nameof(Content));
            }
        }
        private string content;

        public string Tags { get => tags;
            set
            {
                tags = value;
                NotifyPropertyChanged(nameof(Tags));
            }
        }
        private string tags;

        /// <summary>
        /// 负责人Id
        /// </summary>
        public int? ExecutantId
        {
            get => executantId;
            set
            {
                executantId = value;
                NotifyPropertyChanged(nameof(ExecutantId));
            }
        }
        private int? executantId;

        /// <summary>
        /// 事项来源
        /// </summary>
        public string CameFrom
        {
            get => cameFrom;
            set
            {
                cameFrom = value;
                NotifyPropertyChanged(nameof(CameFrom));
            }
        }
        private string cameFrom;

        /// <summary>
        /// 密码
        /// </summary>
        /// <remarks>如果设定了此密码，则在查看或编辑事项详情时必须先核对密码，事项创建人可重置此密码</remarks>
        public string Password
        {
            get => password;
            set
            {
                password = value;
                NotifyPropertyChanged(nameof(Password));
            }
        }
        private string password;

        /// <summary>
        /// 关联事项
        /// </summary>
        public string RelatedMatter
        {
            get => relatedMatter;
            set
            {
                relatedMatter = value;
                NotifyPropertyChanged(nameof(RelatedMatter));
            }
        }
        private string relatedMatter;

        /// <summary>
        /// 关联事项Id
        /// </summary>
        public int? RelatedMatterId
        {
            get => relatedMatterId;
            set
            {
                relatedMatterId = value;
                NotifyPropertyChanged(nameof(RelatedMatterId));
            }
        }
        private int? relatedMatterId;

        /// <summary>
        /// 重要性
        /// </summary>
        public int Importance
        {
            get => importance;
            set
            {
                importance = value;
                NotifyPropertyChanged(nameof(Importance));
            }
        }
        private int importance;

        /// <summary>
        /// 预计需时(数量)
        /// </summary>
        public decimal EstimatedTimeRequired_Num
        {
            get => estimatedTimeRequired_Num;
            set
            {
                estimatedTimeRequired_Num = value;
                NotifyPropertyChanged(nameof(EstimatedTimeRequired_Num));
            }
        }
        private decimal estimatedTimeRequired_Num;

        /// <summary>
        /// 预计需时(单位)
        /// </summary>
        public int EstimatedTimeRequired_Unit
        {
            get => estimatedTimeRequired_Unit;
            set
            {
                estimatedTimeRequired_Unit = value;
                NotifyPropertyChanged(nameof(EstimatedTimeRequired_Unit));
            }
        }
        private int estimatedTimeRequired_Unit;

        /// <summary>
        /// 最后期限
        /// </summary>
        public DateTime? Deadline
        {
            get => deadline;
            set
            {
                deadline = value;
                NotifyPropertyChanged(nameof(Deadline));
            }
        }
        private DateTime? deadline;

        /// <summary>
        /// 已完成
        /// </summary>
        public bool Finished
        {
            get => finished;
            set
            {
                finished = value;
                NotifyPropertyChanged(nameof(Finished));
            }
        }
        private bool finished;

        /// <summary>
        /// 完成时间
        /// </summary>
        public DateTime? FinishTime
        {
            get => finishTime;
            set
            {
                finishTime = value;
                NotifyPropertyChanged(nameof(FinishTime));
            }
        }
        private DateTime? finishTime;

        /// <summary>
        /// 周期性事项
        /// </summary>
        public bool Periodic
        {
            get => periodic;
            set
            {
                periodic = value;
                NotifyPropertyChanged(nameof(Periodic));
            }
        }
        private bool periodic;

        /// <summary>
        /// 间隔周期(数值)
        /// </summary>
        public decimal IntervalPeriod_Num
        {
            get => intervalPeriod_Num;
            set
            {
                intervalPeriod_Num = value;
                NotifyPropertyChanged(nameof(IntervalPeriod_Num));
            }
        }
        private decimal intervalPeriod_Num;

        /// <summary>
        /// 间隔周期(单位)
        /// </summary>
        public int IntervalPeriod_Unit
        {
            get => intervalPeriod_Unit;
            set
            {
                intervalPeriod_Unit = value;
                NotifyPropertyChanged(nameof(IntervalPeriod_Unit));
            }
        }
        private int intervalPeriod_Unit;

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark
        {
            get => remark;
            set
            {
                remark = value;
                NotifyPropertyChanged(nameof(Remark));
            }
        }
        private string remark;

        /// <summary>
        /// 所属小组，此属性值为null时表示归属个人
        /// </summary>
        public int? TeamId
        {
            get => teamId;
            set
            {
                teamId = value;
                NotifyPropertyChanged(nameof(TeamId));
            }
        }
        private int? teamId;

        public string LatestEvolvement
        {
            get => latestEvolvement;
            set
            {
                latestEvolvement = value;
                NotifyPropertyChanged(nameof(LatestEvolvement));
            }
        }
        private string latestEvolvement;

        /// <summary>
        /// 创建人姓名
        /// </summary>
        public string CreatorName { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreationTime { get; set; }


        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    public class MatterOutline : INotifyPropertyChanged
    {
        public MatterOutline()
        {
        }

        public int Id { get; set; }

        /// <summary>
        /// 主题
        /// </summary>
        public string Subject
        {
            get => subject;
            set
            {
                subject = value;
                NotifyPropertyChanged(nameof(Subject));
            }
        }
        private string subject;

        /// <summary>
        /// 重要性
        /// </summary>
        public int Importance
        {
            get => importance; set
            {
                importance = value;
                NotifyPropertyChanged(nameof(Importance));
            }
        }
        public int importance;

        /// <summary>
        /// 最后期限
        /// </summary>
        public DateTime? Deadline
        {
            get => deadline; set
            {
                deadline = value;
                NotifyPropertyChanged(nameof(Deadline));
            }
        }
        public DateTime? deadline;

        /// <summary>
        /// 小组/项目
        /// </summary>
        public string Team
        {
            get => team;
            set
            {
                team = value;
                NotifyPropertyChanged(nameof(Team));
            }
        }
        private string team;

        /// <summary>
        /// 创建人姓名
        /// </summary>
        public string CreatorName { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using XactTodo.WinUI.Models;
using XactTodo.WinUI.Utils;

namespace XactTodo.WinUI
{
    public partial class MatterViewModel : INotifyPropertyChanged
    {
        private readonly HttpClient client;

        public Matter Matter { get; }

        public event PropertyChangedEventHandler PropertyChanged;
        public ObservableCollection<MemberOutline> TeamMembers { get; set; }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private static List<ValueItem> importances;
        public IReadOnlyList<ValueItem> Importances
        {
            get
            {
                if (importances == null)
                {
                    importances = new List<ValueItem>();
                    var client = Utils.HttpClientFactory.CreateClient();
                    const string url = "api/Matter/importances";
                    var response = AsyncHelper.RunSync(() => client.GetAsync(url));
                    response.EnsureSuccessStatusCode(); // 有错误码时报出异常
                    var s = response.Content.ReadAsStringAsync().Result;
                    var kvs = JsonConvert.DeserializeObject<IEnumerable<KeyValuePair<int, string>>>(s);
                    foreach(var kv in kvs)
                    {
                        importances.Add(new ValueItem(kv.Key, kv.Value));
                    }
                }
                return importances.AsReadOnly();
            }
        }

        private static List<ValueItem> joinedTeams;
        public IReadOnlyList<ValueItem> JoinedTeams
        {
            get
            {
                if (joinedTeams == null)
                {
                    joinedTeams = new List<ValueItem>();
                    joinedTeams.Add(new ValueItem(null, ""));
                    var client = Utils.HttpClientFactory.CreateClient();
                    const string url = "api/Team/GetJoinedTeams";
                    var response = AsyncHelper.RunSync(() => client.GetAsync(url));
                    response.EnsureSuccessStatusCode(); // 有错误码时报出异常
                    var s = response.Content.ReadAsStringAsync().Result;
                    var teams = JsonConvert.DeserializeObject<IEnumerable<TeamOutline>>(s);
                    foreach(var team in teams)
                    {
                        joinedTeams.Add(new ValueItem(team.Id, team.Name));
                    }
                }
                return joinedTeams.AsReadOnly();
            }
        }

        private static List<ValueItem> timeUnits;
        public IReadOnlyList<ValueItem> TimeUnits
        {
            get
            {
                if (timeUnits == null)
                {
                    timeUnits = new List<ValueItem>();
                    var client = Utils.HttpClientFactory.CreateClient();
                    const string url = "api/Matter/timeunits";
                    var response = AsyncHelper.RunSync(() => client.GetAsync(url));
                    response.EnsureSuccessStatusCode(); // 有错误码时报出异常
                    var s = response.Content.ReadAsStringAsync().Result;
                    var kvs = JsonConvert.DeserializeObject<IEnumerable<KeyValuePair<int, string>>>(s);
                    foreach(var kv in kvs)
                    {
                        timeUnits.Add(new ValueItem(kv.Key, kv.Value));
                    }
                }
                return timeUnits.AsReadOnly();
            }
        }

        public MatterViewModel()
        {
            client = Utils.HttpClientFactory.CreateClient();
            //Importances = AsyncHelper.RunSync(GetImportances());
            this.Matter = new Matter();
            BindPropertyChanged();
        }

        public MatterViewModel(Matter matter)
        {
            this.Matter = matter.Clone();
            BindPropertyChanged();
            if(matter.TeamId.HasValue)
                OnPropertyChanged(nameof(CurrentTeam));
        }

        private void BindPropertyChanged()
        {
            // 当所属小组ComboBox的选项变化时，更新负责人ComboBox的项
            PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(CurrentTeam))
                {
                    LoadMembers(this.Matter.TeamId);
                }
            };
        }

        private void LoadMembers(int? teamId)
        {
            IEnumerable<MemberOutline> members = new MemberOutline[0];
            // 根据当前选择的组，更新负责人选择框数据源
            TeamMembers = new ObservableCollection<MemberOutline>();
            if (teamId.HasValue)
            {
                var client = Utils.HttpClientFactory.CreateClient();
                string url = $"api/Team/{teamId}/Members";
                var response = client.GetAsync(url).Result;
                response.EnsureSuccessStatusCode(); // 有错误码时报出异常
                var s = response.Content.ReadAsStringAsync().Result;
                members = JsonConvert.DeserializeObject<IEnumerable<MemberOutline>>(s);
                foreach (var member in members)
                {
                    TeamMembers.Add(member);
                }
            }
            // 如果组内成员不含当前登录用户，则将当前登录用户加入到列表
            if (!members.Any(p => p.UserId == (App.Current as App).CurrentUser.Id))
            {
                var u = (App.Current as App).CurrentUser;
                TeamMembers.Insert(0, new MemberOutline { Id = -1, UserId = u.Id, UserName = u.UserName, DisplayName = u.DisplayName, Email = u.Email });
            }
            // 通知界面更新
            OnPropertyChanged(nameof(TeamMembers));
        }

        public ValueItem CurrentImportance
        {
            get { return Importances.Single(p => p.Id == this.Matter.Importance); }
            set { this.Matter.Importance = value.Id.Value; }
        }

        public ValueItem CurrentTeam
        {
            get { return JoinedTeams.SingleOrDefault(p => p.Id == this.Matter.TeamId); }
            set
            {
                this.Matter.TeamId = value.Id;
                OnPropertyChanged(nameof(CurrentTeam));
            }
        }

        public ValueItem EstimatedTimeRequired_Unit
        {
            get { return TimeUnits.Single(p => p.Id == this.Matter.EstimatedTimeRequired_Unit); }
            set { this.Matter.EstimatedTimeRequired_Unit = value.Id.Value; }
        }

        public ValueItem IntervalPeriod_Unit
        {
            get { return TimeUnits.Single(p => p.Id == this.Matter.IntervalPeriod_Unit); }
            set { this.Matter.IntervalPeriod_Unit = value.Id.Value; }
        }

        [RelayCommand]
        internal async Task Save()
        {
            if (Matter.Id == 0) //新增事项
            {
                const string url = "api/Matter";
                var jsonMatter = JsonConvert.SerializeObject(this.Matter);
                var response = await client.PostAsync(url, new StringContent(jsonMatter, Encoding.UTF8, "application/json"));
                response.EnsureSuccessStatusCode();
                var id = await response.Content.ReadAsStringAsync();
                Matter.Id = int.Parse(id);
            }
            else //修改事项
            {
                var client = Utils.HttpClientFactory.CreateClient();
                var url = $"api/Matter/{Matter.Id}";
                var jsonMatter = JsonConvert.SerializeObject(this.Matter);
                var response = await client.PutAsync(url, new StringContent(jsonMatter, Encoding.UTF8, "application/json"));
                response.EnsureSuccessStatusCode();
            }
        }

        [RelayCommand]
        internal void SaveAs()
        {
            var subject = this.Matter.Subject;
            MessageBox.Show(subject);
        }

        public class ValueItem
        {
            public ValueItem(int? id, string text)
            {
                Id = id;
                Text = text;
            }

            public int? Id { get; private set; }

            public string Text { get; private set; }
        }

    }
}

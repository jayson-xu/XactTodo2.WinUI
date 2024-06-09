using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace XactTodo.WinUI.Utils
{
    public class AppSettingManager
    {
        private readonly string settingFile;

        private AppSettingManager()
        {
            //Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) //当前用户使用的应用程序特定数据存储路径
            this.settingFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AppSettings.xml");
        }

        public static AppSettingManager _instance;
        public static AppSettingManager Instance => _instance ?? (_instance = new AppSettingManager());

        public void Save(string key, string value)
        {
            Save(new KeyValuePair<string, string>(key, value));
        }

        public void Save(params KeyValuePair<string, string>[] items)
        {
            XmlDocument doc = new XmlDocument();
            if (File.Exists(settingFile))
                doc.Load(settingFile);
            if (doc.DocumentElement == null)
                doc.AppendChild(doc.CreateElement("appSettings"));
            foreach (var kv in items)
            {
                //通过XPath查找拥有名为key且其值为{kv.Key}的属性的add节点，通过translate将属性值转换为大写比较以实现忽略大小写效果
                var node = doc.DocumentElement.SelectSingleNode($"add[translate(@key,'abcdefghijklmnopqrstuvwxyz','ABCDEFGHIJKLMNOPQRSTUVWXYZ')='{kv.Key.ToUpper()}']");
                if (node == null)
                {
                    node = doc.DocumentElement.AppendChild(doc.CreateElement("add"));
                    node.Attributes.Append(doc.CreateAttribute("key")).Value = kv.Key;
                }
                var valueAttr = node.Attributes["value"];
                if (valueAttr == null)
                {
                    valueAttr = node.Attributes.Append(doc.CreateAttribute("value"));
                }
                node.Attributes["value"].Value = kv.Value;
            }
            using (var xw = new XmlTextWriter(settingFile, Encoding.UTF8))
            {
                xw.Formatting = Formatting.Indented;
                doc.Save(xw);
            }
        }

        public IEnumerable<KeyValuePair<string, string>> Read()
        {
            var kvs = new List<KeyValuePair<string, string>>();
            if (File.Exists(settingFile))
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(settingFile);
                //XmlNode nodeLoginSetting = doc.DocumentElement["LastLogin"];
                var nodes = doc.DocumentElement.SelectNodes("add[@key]");
                foreach (XmlNode node in nodes)
                {
                    var kv = new KeyValuePair<string, string>(node.Attributes["key"].Value, node.Attributes["value"]?.Value);
                    kvs.Add(kv);
                }
            }
            return kvs.AsEnumerable();
        }

    }
}

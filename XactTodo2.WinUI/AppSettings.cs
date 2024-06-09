using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using XactTodo.WinUI.Utils;

namespace XactTodo.WinUI
{
    public class AppSettings
    {
        private byte[] entropy = new byte[20]; // 生成一个附加的熵（用于加密存放密码）
        private string rootUrl_Api;
        private string userName;
        private string password;
        private bool autoStart;

        private AppSettings()
        {
            InitEntropy();
            var items = AppSettingManager.Instance.Read();
            var kv = items.Where(p => p.Key.Equals(nameof(rootUrl_Api), StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            //if (kv != null)
            {
                rootUrl_Api = kv.Value;
            }
            //用户账号
            kv = items.Where(p => p.Key.Equals(nameof(userName), StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            userName = kv.Value;
            //密码
            kv = items.Where(p => p.Key.Equals(nameof(password), StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            if (!string.IsNullOrEmpty(kv.Value))
            {
                var pw = Convert.FromBase64String(kv.Value);
                password = Encoding.UTF8.GetString(ProtectedData.Unprotect(pw, entropy, DataProtectionScope.CurrentUser));
            }
            //自启动
            kv = items.Where(p => p.Key.Equals(nameof(autoStart), StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            bool.TryParse(kv.Value, out autoStart);
        }

        /// <summary>
        /// 初始化用于加密的随机序列
        /// </summary>
        private void InitEntropy()
        {
            var dirAppData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), this.GetType().Namespace);
            string entropyFile = Path.Combine(dirAppData, "ncc");
            if (File.Exists(entropyFile))
            {
                using (var fs = File.OpenRead(entropyFile))
                {
                    for (int i = 0; i < entropy.Length; i++)
                    {
                        entropy[i] = (byte)fs.ReadByte();
                    }
                }
            }
            else
            {
                using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
                {
                    rng.GetBytes(entropy);
                }
                if (!Directory.Exists(dirAppData))
                    Directory.CreateDirectory(dirAppData);
                File.WriteAllBytes(entropyFile, entropy);
            }
        }

        private static AppSettings _instance;
        public static AppSettings Instance => _instance ?? (_instance = new AppSettings());

        public string RootUrl_Api
        {
            get => rootUrl_Api;
            set
            {
                rootUrl_Api = value;
                AppSettingManager.Instance.Save(nameof(RootUrl_Api), rootUrl_Api);
            }
        }

        public string UserName
        {
            get => userName;
            set
            {
                userName = value;
                AppSettingManager.Instance.Save(nameof(UserName), userName);
            }
        }

        public string Password
        {
            get => password;
            set
            {
                password = value;
                string protectedPw = string.Empty;
                if (!string.IsNullOrEmpty(password))
                {
                    byte[] plaintext = Encoding.UTF8.GetBytes(password);
                    protectedPw = Convert.ToBase64String(ProtectedData.Protect(plaintext, entropy, DataProtectionScope.CurrentUser));
                }
                AppSettingManager.Instance.Save(nameof(Password), protectedPw);
            }
        }

        public bool AutoStart
        {
            get => autoStart;
            set
            {
                autoStart = value;
                var module = Process.GetCurrentProcess().MainModule;
                SetAutoStart(autoStart, module.ModuleName, module.FileName);
            }
        }

        private string ToTitleCase(string s)
        {
            return string.IsNullOrEmpty(s) ? s : (s.Substring(0, 1).ToUpper() + s.Substring(1));
        }

        private static bool SetAutoStart(bool autoStart, string fileName, string fullPath)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentNullException(nameof(fileName));
            if (string.IsNullOrWhiteSpace(fullPath))
                throw new ArgumentNullException(nameof(fullPath));
            try
            {
                var local = Registry.LocalMachine;
                var key = local.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
                if (key == null)
                {
                    local.CreateSubKey("SOFTWARE//Microsoft//Windows//CurrentVersion//Run");
                }
                if (autoStart)//若开机自启动则添加键值对
                {
                    key.SetValue(fileName, fullPath);
                    key.Close();
                }
                else//否则删除键值对
                {
                    string[] names = key.GetValueNames();
                    foreach (string name in names)
                    {
                        if (fileName.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                        {
                            key.DeleteValue(fileName);
                            key.Close();
                        }
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        private bool IsExistKey(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));
            try
            {
                var local = Registry.LocalMachine;
                var runs = local.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
                if (runs == null)
                {
                    var key2 = local.CreateSubKey("SOFTWARE");
                    var key3 = key2.CreateSubKey("Microsoft");
                    var key4 = key3.CreateSubKey("Windows");
                    var key5 = key4.CreateSubKey("CurrentVersion");
                    var key6 = key5.CreateSubKey("Run");
                    runs = key6;
                }
                string[] runsName = runs.GetValueNames();
                foreach (string name in runsName)
                {
                    if (key.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                    {
                        return true;
                    }
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}

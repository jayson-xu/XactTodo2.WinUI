using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace XactTodo.WinUI.Components
{
    /// <summary>
    /// TagEditor.xaml 的交互逻辑
    /// </summary>
    public partial class TagEditor : UserControl
    {
        public TagEditor()
        {
            InitializeComponent();
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var items = lstTags.Items;
                var newTag = new TextBlock() { Text = (sender as TextBox).Text };
                (sender as TextBox).Text = "";
                lstTags.Items.Insert(lstTags.Items.Count - 1, newTag);
            }
        }
    }
}

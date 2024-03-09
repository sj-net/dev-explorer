using System.Windows;
using System.Windows.Controls;

namespace FileExplorer.Controls
{
    public class DevExplorerTextBox : TextBox
    {
        public static readonly DependencyProperty WaterMarkProperty = DependencyProperty.Register(nameof(WaterMark), typeof(string), typeof(DevExplorerTextBox), new PropertyMetadata("Enter value"));

        public string WaterMark
        {
            get { return (string)GetValue(WaterMarkProperty); }
            set { SetValue(WaterMarkProperty, value); }
        }
    }
}

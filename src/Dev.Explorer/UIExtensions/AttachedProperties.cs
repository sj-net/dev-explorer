namespace FileExplorer.UIExtensions
{
    using System.Windows.Controls;
    using System.Windows;

    public class AttachedProperties
    {
        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.RegisterAttached("Icon", typeof(ControlTemplate), typeof(AttachedProperties), new PropertyMetadata(null));

        public static ControlTemplate GetIcon(DependencyObject obj)
        {
            return (ControlTemplate)obj.GetValue(IconProperty);
        }

        public static void SetIcon(DependencyObject obj, ControlTemplate value)
        {
            obj.SetValue(IconProperty, value);
        }
    }
}

namespace FileExplorer.Controls
{
    using CommunityToolkit.Mvvm.Messaging;

    using FileExplorer.ViewModels;

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;

    public class DevExplorerTextBlock : ContentControl
    {
        public static readonly DependencyProperty CanCopyProperty =
         DependencyProperty.Register(nameof(CanCopy), typeof(bool), typeof(DevExplorerTextBlock), new PropertyMetadata(false));

        public static readonly DependencyProperty TextDecorationsProperty =
            DependencyProperty.Register(nameof(TextDecorations), typeof(TextDecorationCollection), typeof(DevExplorerTextBlock), new PropertyMetadata(default));

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(nameof(Text), typeof(string), typeof(DevExplorerTextBlock), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty TextTrimmingProperty =
            DependencyProperty.Register(nameof(TextTrimming), typeof(TextTrimming), typeof(DevExplorerTextBlock), new PropertyMetadata(TextTrimming.None));

        public static readonly DependencyProperty TextWrappingProperty =
            DependencyProperty.Register(nameof(TextWrapping), typeof(TextWrapping), typeof(DevExplorerTextBlock), new PropertyMetadata(TextWrapping.NoWrap));

        private Button? copyButton;
        private TextBlock? textBlock;

        public TextTrimming TextTrimming
        {
            get => (TextTrimming)GetValue(TextTrimmingProperty);
            set => SetValue(TextTrimmingProperty, value);
        }

        public TextWrapping TextWrapping
        {
            get => (TextWrapping)GetValue(TextWrappingProperty);
            set => SetValue(TextWrappingProperty, value);
        }
        public bool CanCopy
        {
            get { return (bool)GetValue(CanCopyProperty); }
            set { SetValue(CanCopyProperty, value); }
        }
        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }
        public TextDecorationCollection TextDecorations
        {
            get { return (TextDecorationCollection)GetValue(TextDecorationsProperty); }
            set { SetValue(TextDecorationsProperty, value); }
        }
        public override void OnApplyTemplate()
        {
            copyButton = Template.FindName("PART_TextBlock_CopyButton", this) as Button;

            textBlock = Template.FindName("PART_TextBlock", this) as TextBlock;

            if (copyButton != null && CanCopy)
            {
                copyButton.Click += CopyButton_Click;
            }

            if (textBlock != null && TextDecorations != null)
            {
                textBlock.TextDecorations = TextDecorations;
            }
        }
        private void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(Text);
            StrongReferenceMessenger.Default.Send(new ToasterViewModel(ToasterType.Success, $"Copied {Text}"));
            copyButton?.SetCurrentValue(VisibilityProperty, Visibility.Collapsed);
        }
    }
}

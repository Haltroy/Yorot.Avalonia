using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System;

namespace Yorot_Avalonia.Views
{
    public partial class MessageBox : Window
    {
        public MessageBox()
        {
            InitializeComponent();
        }

        public MessageBox(string title, string caption, MessageBoxButton[] messageBoxButtons, bool closeAfterclick = true)
        {
            CloseAfterClick = closeAfterclick;
            MessageBoxButtons = messageBoxButtons;
            InitializeComponent();
            if (TextContent != null)
            {
                TextContent.Text = caption;
            }
            if (TitleBar != null)
            {
                TitleBar.Text = title;
            }
        }

        public MessageBox(string title, string caption, string defaultPrompt, MessageBoxButton[] messageBoxButtons, bool closeAfterclick = true)
        {
            CloseAfterClick = closeAfterclick;
            MessageBoxButtons = messageBoxButtons;
            InitializeComponent();
            if (TextContent != null)
            {
                TextContent.Text = caption;
            }
            if (TitleBar != null)
            {
                TitleBar.Text = title;
            }
            if (defaultPrompt != null && DontAskAgain != null)
            {
                DontAskAgain.IsVisible = false;
                DontAskAgain.IsEnabled = false;
                PromptBox.IsVisible = true;
                PromptBox.IsEnabled = true;
                PromptBox.Text = defaultPrompt;
            }
        }

        public MessageBox(string title, string caption, MessageBoxButton[] messageBoxButtons, Avalonia.Media.IImage image, bool closeAfterclick = true)
        {
            CloseAfterClick = closeAfterclick;
            MessageBoxButtons = messageBoxButtons;
            InitializeComponent();
            if (TextContent != null)
            {
                TextContent.Text = caption;
            }
            if (TitleBar != null)
            {
                TitleBar.Text = title;
            }
            if (Image != null)
            {
                Image.Source = image;
            }
        }

        public MessageBox(string title, string caption, string defaultPrompt, MessageBoxButton[] messageBoxButtons, Avalonia.Media.IImage image, bool closeAfterclick = true)
        {
            CloseAfterClick = closeAfterclick;
            MessageBoxButtons = messageBoxButtons;
            InitializeComponent();
            if (TextContent != null)
            {
                TextContent.Text = caption;
            }
            if (TitleBar != null)
            {
                TitleBar.Text = title;
            }
            if (Image != null)
            {
                Image.Source = image;
            }
            if (PromptBox != null && DontAskAgain != null)
            {
                DontAskAgain.IsVisible = false;
                DontAskAgain.IsEnabled = false;
                PromptBox.IsVisible = true;
                PromptBox.IsEnabled = true;
            }
        }

        private MessageBoxButton[] MessageBoxButtons = new MessageBoxButton[] { new MessageBoxButton.Ok(), new MessageBoxButton.Cancel() };

        private DockPanel? Buttons;
        private CheckBox? DontAskAgain;
        private Image? Image;
        private TextBlock? TextContent;
        private TextBlock? TitleBar;
        private TextBox? PromptBox;

        private bool CloseAfterClick = true;

        public string Prompt => PromptBox != null ? PromptBox.Text : "";

        public MessageBoxButton DialogResult = new MessageBoxButton.Ok();

        public bool DontShowThis => DontAskAgain != null ? (DontAskAgain.IsChecked.HasValue ? DontAskAgain.IsChecked.Value : false) : false;

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            var maingrid = this.FindControl<Grid>("MainGrid");
            TitleBar = maingrid.FindControl<TextBlock>("TitleBar");
            DontAskAgain = maingrid.FindControl<CheckBox>("DontAskAgain");
            DontAskAgain.Content = new TextBlock() { Text = YorotGlobal.Main.CurrentLanguage.GetItemText("DialogBox.DontAskAgain") };
            PromptBox = maingrid.FindControl<TextBox>("PromptBox");
            Buttons = maingrid.FindControl<DockPanel>("Buttons");
            var contentgrid = maingrid.FindControl<Grid>("ContentGrid");
            Image = contentgrid.FindControl<Image>("ImageContent");
            TextContent = contentgrid.FindControl<TextBlock>("TextContent");

            var examplebutton = Buttons.FindControl<Button>("examplebutton");

            for (int i = 0; i < MessageBoxButtons.Length; i++)
            {
                var btn = MessageBoxButtons[i];
                Button button = new()
                {
                    Margin = new Thickness(5, 5, 5, 5),
                    Content = btn.Text
                };
                button.Bind(BackgroundProperty, examplebutton.GetBindingObservable(BackgroundProperty));
                button.Bind(ForegroundProperty, examplebutton.GetBindingObservable(ForegroundProperty));
                button.Click += new EventHandler<Avalonia.Interactivity.RoutedEventArgs>((sender, e) =>
                {
                    DialogResult = btn;
                    if (CloseAfterClick)
                    {
                        Close();
                    }
                });
                Buttons.Children.Add(button);
            }
        }
    }

    public class MessageBoxButton
    {
        public virtual string Text => "";

        public class Ok : MessageBoxButton
        {
            public override string Text => YorotGlobal.Main.CurrentLanguage.GetItemText("DialogBox.OK");
        }

        public class Yes : MessageBoxButton
        {
            public override string Text => YorotGlobal.Main.CurrentLanguage.GetItemText("DialogBox.Yes");
        }

        public class No : MessageBoxButton
        {
            public override string Text => YorotGlobal.Main.CurrentLanguage.GetItemText("DialogBox.No");
        }

        public class Cancel : MessageBoxButton
        {
            public override string Text => YorotGlobal.Main.CurrentLanguage.GetItemText("DialogBox.Cancel");
        }

        public class Apply : MessageBoxButton
        {
            public override string Text => YorotGlobal.Main.CurrentLanguage.GetItemText("DialogBox.Apply");
        }

        public class Ignore : MessageBoxButton
        {
            public override string Text => YorotGlobal.Main.CurrentLanguage.GetItemText("DialogBox.Ignore");
        }

        public class Retry : MessageBoxButton
        {
            public override string Text => YorotGlobal.Main.CurrentLanguage.GetItemText("DialogBox.Retry");
        }

        public class Abort : MessageBoxButton
        {
            public override string Text => YorotGlobal.Main.CurrentLanguage.GetItemText("DialogBox.Abort");
        }
    }
}
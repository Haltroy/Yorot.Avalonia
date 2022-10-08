using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System;

namespace Yorot_Avalonia.Views
{
    public partial class test : UserControl
    {
        public test()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
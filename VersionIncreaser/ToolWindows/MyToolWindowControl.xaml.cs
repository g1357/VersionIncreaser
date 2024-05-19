using VersionIncreaser.ToolWindows;
using System.Windows;
using System.Windows.Controls;

namespace VersionIncreaser
{
    public partial class MyToolWindowControl : UserControl
    {
        public readonly MyToolWindowViewModel viewModel;

        public MyToolWindowControl()
        {
            InitializeComponent();


            DataContext = viewModel = new MyToolWindowViewModel();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            VS.MessageBox.Show("FirstTooWinCom", "Button clicked");
        }
    }
}
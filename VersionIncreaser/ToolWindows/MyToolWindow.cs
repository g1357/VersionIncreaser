using Community.VisualStudio.Toolkit;
using Microsoft.VisualStudio.Imaging;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace VersionIncreaser
{
    public class MyToolWindow : BaseToolWindow<MyToolWindow>
    {
        public override string GetTitle(int toolWindowId) => "My Tool Window";

        public override Type PaneType => typeof(Pane);

        FrameworkElement frameworkElement;

        public override async Task<FrameworkElement> CreateAsync(int toolWindowId, CancellationToken cancellationToken)
        {
            frameworkElement = await Task.FromResult<FrameworkElement>(new MyToolWindowControl());
            return frameworkElement;
        }

        [Guid("13b80bfb-08d5-4386-848e-8f86935e28f0")]
        internal class Pane : ToolkitToolWindowPane
        {
            public Pane()
            {
                BitmapImageMoniker = KnownMonikers.ToolWindow;
            }
        }

        public new static async Task<ToolWindowPane?> ShowAsync(int id = 0, bool create = true)
        {
            var toolWindowPane = await BaseToolWindow<MyToolWindow>.ShowAsync(id, create);

            ((MyToolWindowControl)toolWindowPane.Content).viewModel.RefreshData();

            return toolWindowPane;
        }

    }
}
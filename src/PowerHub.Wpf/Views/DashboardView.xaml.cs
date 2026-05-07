using System.Windows.Controls;
using PowerHub.UI.ViewModels;

namespace PowerHub.UI.Views
{
    public partial class DashboardView : Page
    {
        public DashboardView(DashboardViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}

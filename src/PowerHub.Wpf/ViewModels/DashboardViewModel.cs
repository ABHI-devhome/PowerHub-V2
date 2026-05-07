using System.Windows.Input;
using Wpf.Ui.Input;
using PowerHub.UI.Services;

namespace PowerHub.UI.ViewModels
{
    public class DashboardViewModel : ObservableObject
    {
        private readonly ITweakService _tweakService;

        public ICommand ToggleStealthModeCommand { get; }

        public DashboardViewModel(ITweakService tweakService)
        {
            _tweakService = tweakService;
            ToggleStealthModeCommand = new RelayCommand(ExecuteToggleStealthMode);
        }

        private void ExecuteToggleStealthMode()
        {
            _tweakService.ApplyStealthMode();
        }
    }
}

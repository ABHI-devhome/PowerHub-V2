using System;
using System.Windows;
using global::Wpf.Ui.Controls;
using PowerHub.UI.Services;
using PowerHub.UI.Views;
using PowerHub.UI.ViewModels;

namespace PowerHub.UI
{
    public partial class MainWindow : FluentWindow
    {
        private readonly AppServices _services;

        public MainWindow(AppServices services)
        {
            _services = services;
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            RootNavigation.SelectionChanged += RootNavigation_SelectionChanged;
            NavigateTo("Dashboard");
        }

        private void RootNavigation_SelectionChanged(NavigationView sender, RoutedEventArgs args)
        {
            if (sender.SelectedItem is NavigationViewItem item)
            {
                NavigateTo(item.Content?.ToString());
            }
        }

        private void NavigateTo(string? pageName)
        {
            if (pageName == "Dashboard")
            {
                var vm = new DashboardViewModel(_services.Tweaks);
                RootFrame.Content = new DashboardView(vm);
            }
            else
            {
                // Placeholder for other pages
                RootFrame.Content = null;
            }
        }
    }
}

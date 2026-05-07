using System;
using System.Windows;
using System.Windows.Threading;
using PowerHub.UI.Services;

namespace PowerHub.UI
{
    public partial class App : Application
    {
        public static AppServices Services { get; private set; } = null!;

        public App()
        {
            DispatcherUnhandledException += OnDispatcherUnhandledException;
        }

        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show(
                $"A critical error occurred after startup:\n\n{e.Exception.Message}\n\nStack Trace:\n{e.Exception.StackTrace}",
                "PowerHub Critical Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            e.Handled = true;
            Shutdown(1);
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {
                base.OnStartup(e);

                var admin = new AdminService();
                //if (!admin.IsRunningAsAdministrator())
                //{
                //    MessageBox.Show(
                //        "PowerHub must be launched with administrator privileges. Windows should prompt for this automatically before the app opens.",
                //        "PowerHub",
                //        MessageBoxButton.OK,
                //        MessageBoxImage.Warning);
                //    Shutdown(1);
                //    return;
                //}

                var log = new ActivityLogService();
                var dialogs = new DialogService();
                var power = new PowerService(log);
                var display = new DisplayService(log);
                var tweaks = new TweakService(log);
                var settings = new SettingsService();
                var telemetry = new TelemetryService();
                Services = new AppServices(log, dialogs, power, display, tweaks, admin, telemetry, settings);

                log.Add(ActivityKind.Info, "PowerHub started.");

                var main = new MainWindow(Services);
                MainWindow = main;
                main.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"PowerHub failed to start correctly.\n\nError: {ex.Message}\n\nInner Error: {ex.InnerException?.Message}\n\nStack Trace:\n{ex.StackTrace}",
                    "PowerHub Startup Failure",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                Shutdown(1);
            }
        }
    }
}

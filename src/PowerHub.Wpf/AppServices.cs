using PowerHub.UI.Services;

namespace PowerHub.UI
{
    public sealed class AppServices
    {
        public AppServices(
            IActivityLogService activityLog,
            IDialogService dialogs,
            IPowerService power,
            IDisplayService display,
            ITweakService tweaks,
            IAdminService admin,
            ITelemetryService telemetry,
            ISettingsService settings)
        {
            ActivityLog = activityLog;
            Dialogs = dialogs;
            Power = power;
            Display = display;
            Tweaks = tweaks;
            Admin = admin;
            Telemetry = telemetry;
            Settings = settings;
        }

        public IActivityLogService ActivityLog { get; }
        public IDialogService Dialogs { get; }
        public IPowerService Power { get; }
        public IDisplayService Display { get; }
        public ITweakService Tweaks { get; }
        public IAdminService Admin { get; }
        public ITelemetryService Telemetry { get; }
        public ISettingsService Settings { get; }
    }
}

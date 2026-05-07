namespace PowerHub.UI.Services
{
    public sealed class AppSettings
    {
        public int DashboardRefreshSeconds { get; set; } = 2;
        public string TemperatureUnit { get; set; } = "C";
    }

    public interface ISettingsService
    {
        AppSettings Current { get; }
        AppSettings Load();
        AppSettings Save(AppSettings settings);
    }
}

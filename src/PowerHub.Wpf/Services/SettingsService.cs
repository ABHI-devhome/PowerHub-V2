using System;
using System.IO;
using System.Text.Json;

namespace PowerHub.UI.Services
{
    public sealed class SettingsService : ISettingsService
    {
        private readonly string _settingsPath;
        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions { WriteIndented = true };

        public SettingsService()
        {
            string root = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "PowerHub");
            Directory.CreateDirectory(root);
            _settingsPath = Path.Combine(root, "settings.json");
            Current = Load();
        }

        public AppSettings Current { get; private set; }

        public AppSettings Load()
        {
            try
            {
                if (!File.Exists(_settingsPath))
                    return new AppSettings();

                var settings = JsonSerializer.Deserialize<AppSettings>(File.ReadAllText(_settingsPath), _jsonOptions);
                return Normalize(settings ?? new AppSettings());
            }
            catch
            {
                return new AppSettings();
            }
        }

        public AppSettings Save(AppSettings settings)
        {
            Current = Normalize(settings);
            File.WriteAllText(_settingsPath, JsonSerializer.Serialize(Current, _jsonOptions));
            return Current;
        }

        private static AppSettings Normalize(AppSettings settings)
        {
            if (settings.DashboardRefreshSeconds < 2) settings.DashboardRefreshSeconds = 2;
            if (settings.DashboardRefreshSeconds > 10) settings.DashboardRefreshSeconds = 10;
            if (settings.TemperatureUnit != "F") settings.TemperatureUnit = "C";
            return settings;
        }
    }
}

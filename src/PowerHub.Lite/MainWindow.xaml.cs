using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using PowerHub.Core;
using System.Windows.Media.Imaging;

namespace PowerHub.Lite
{
    public partial class MainWindow : Window
    {
        private string _standardModeGuid = "00000000-0000-0000-0000-000000000000";
        private string _batterySaverModeGuid = PowerManager.BatterySaverOverlayGuid;

        public MainWindow()
        {
            InitializeComponent();

            try
            {
                this.Icon = new BitmapImage(new Uri("pack://application:,,,/PowerHub.ico"));
            }
            catch
            {
                // Fallback in case the PowerHub.ico file is not found or not set as a Resource correctly
            }

            RefreshInfo();
            LoadPlans();
            LoadRefreshRates();
            LoadPowerModes();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Update refresh rate display after window loads
            UpdateRefreshRateDisplay();
        }

        private void RefreshInfo()
        {
            try
            {
                string planName = PowerManager.GetActivePlanName();
                ActivePlanText.Text = string.IsNullOrEmpty(planName) ? "Unknown" : planName;

                string modeGuid = PowerManager.GetActivePowerModeGuid();
                ActiveModeText.Text = PowerManager.GetPowerModeDisplayName(modeGuid);

                int? refreshRate = PowerManager.GetCurrentRefreshRate();
                RefreshRateText.Text = refreshRate.HasValue ? $"{refreshRate} Hz" : "Unknown";

                BatterySaverText.Text = PowerManager.IsEcoModeEnabled() ? "Enabled" : "Disabled";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error refreshing info: " + ex.Message);
            }
        }

        private void LoadPlans()
        {
            var plans = new List<LitePowerPlan>
            {
                new LitePowerPlan { DisplayName = "Power saver", Guid = "a1841308-3541-4fab-bc81-f71556f20b4a", Description = "Reduces system performance and conserves energy." },
                new LitePowerPlan { DisplayName = "Balanced", Guid = "381b4222-f694-41f0-9685-ff5bb260df2e", Description = "Balances performance and energy." },
                new LitePowerPlan { DisplayName = "High performance", Guid = "8c5e7fda-e8bf-4a96-9a85-a6e23a8c635c", Description = "Maximizes CPU performance." },
                new LitePowerPlan { DisplayName = "Ultimate Performance", Guid = "e9a42b02-d5df-448d-aa00-03f14749eb61", Description = "Highest performance; removes micro-latencies." },
                new LitePowerPlan { DisplayName = "Battery saver", Guid = PowerManager.BatterySaverOverlayGuid, Description = "Windows energy saver overlay." }
            };
            PlansListBox.ItemsSource = plans;
        }

        private void LoadRefreshRates()
        {
            try
            {
                var rates = PowerManager.GetAvailableRefreshRates();
                RefreshRateCombo.ItemsSource = rates;
                
                int? currentRate = PowerManager.GetCurrentRefreshRate();
                if (currentRate.HasValue && rates.Contains(currentRate.Value))
                {
                    RefreshRateCombo.SelectedItem = currentRate.Value;
                }
                else if (rates.Count > 0)
                {
                    RefreshRateCombo.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                RefreshStatusText.Text = "Error loading rates: " + ex.Message;
            }
        }

        private void LoadPowerModes()
        {
            try
            {
                // Set default selections
                PluggedInModeCombo.SelectedIndex = 0; // Best Performance
                OnBatteryModeCombo.SelectedIndex = 1; // Balanced

                // Add selection change event handlers
                PluggedInModeCombo.SelectionChanged += PluggedInModeCombo_SelectionChanged;
                OnBatteryModeCombo.SelectionChanged += OnBatteryModeCombo_SelectionChanged;

                // Initialize descriptions
                UpdatePluggedInDescription();
                UpdateOnBatteryDescription();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading power modes: " + ex.Message);
            }
        }

        private void PluggedInModeCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdatePluggedInDescription();
        }

        private void OnBatteryModeCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateOnBatteryDescription();
        }

        private void UpdatePluggedInDescription()
        {
            if (PluggedInModeCombo.SelectedItem is ComboBoxItem item)
            {
                PluggedInModeDesc.Text = item.Content.ToString() switch
                {
                    "Best Performance" => "Maximize performance for demanding tasks",
                    "Balanced" => "Balance performance and energy consumption",
                    "Best Battery Saver" => "Minimize power consumption to extend battery life",
                    _ => ""
                };
            }
        }

        private void UpdateOnBatteryDescription()
        {
            if (OnBatteryModeCombo.SelectedItem is ComboBoxItem item)
            {
                OnBatteryModeDesc.Text = item.Content.ToString() switch
                {
                    "Best Performance" => "Maximize performance for demanding tasks",
                    "Balanced" => "Balance performance and energy consumption",
                    "Best Battery Saver" => "Minimize power consumption to extend battery life",
                    _ => ""
                };
            }
        }

        private void UpdateRefreshRateDisplay()
        {
            try
            {
                int? current = PowerManager.GetCurrentRefreshRate();
                CurrentRefreshText.Text = $"Current: {(current.HasValue ? current.Value + " Hz" : "Unknown")}";
            }
            catch { }
        }

        private void RefreshInfo_Click(object sender, RoutedEventArgs e) => RefreshInfo();

        private void ActivatePlan_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.CommandParameter is LitePowerPlan plan)
            {
                try
                {
                    PowerManager.ApplyPowerPlan(plan.Guid, plan.DisplayName);
                    System.Threading.Thread.Sleep(500);
                    RefreshInfo();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to apply plan: " + ex.Message);
                }
            }
        }

        private void ActivateMode_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
            {
                try
                {
                    string guid = btn.Name == "StandardModeBtn" ? _standardModeGuid : _batterySaverModeGuid;
                    PowerManager.ApplyPowerMode(guid);
                    System.Threading.Thread.Sleep(500);
                    RefreshInfo();
                    MessageBox.Show("Power mode applied successfully!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to apply mode: " + ex.Message);
                }
            }
        }

        private void ApplyRefreshRate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (RefreshRateCombo.SelectedItem is int rate)
                {
                    PowerManager.ApplyRefreshRate(rate);
                    RefreshStatusText.Foreground = System.Windows.Media.Brushes.Green;
                    RefreshStatusText.Text = $"Applied {rate} Hz successfully!";
                    System.Threading.Thread.Sleep(2000);
                    UpdateRefreshRateDisplay();
                }
            }
            catch (Exception ex)
            {
                RefreshStatusText.Foreground = System.Windows.Media.Brushes.Red;
                RefreshStatusText.Text = "Error: " + ex.Message;
            }
        }

        private void UnlockPlans_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var plans = (IEnumerable<LitePowerPlan>)PlansListBox.ItemsSource;
                PowerManager.UnlockAllPlans(plans.Select(p => new KeyValuePair<string, string>(p.DisplayName, p.Guid)));
                MessageBox.Show("Standard plans unlocked. If they were missing, check the list now.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to unlock: " + ex.Message);
            }
        }

        private void ImportPlan_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog { Filter = "Power Scheme (*.pow)|*.pow" };
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    PowerManager.ImportPowerPlan(dialog.FileName);
                    MessageBox.Show("Plan imported successfully.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to import: " + ex.Message);
                }
            }
        }

        private void RunBoost_Click(object sender, RoutedEventArgs e)
        {
            BoostStatusText.Text = "Running boost...";
            try
            {
                SystemTweaks.OneClickBoost();
                BoostStatusText.Text = "One-click boost applied successfully!";
            }
            catch (Exception ex)
            {
                BoostStatusText.Text = "Failed: " + ex.Message;
            }
        }

        private void ApplyPluggedInMode_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (PluggedInModeCombo.SelectedItem is ComboBoxItem item)
                {
                    string? selectedMode = item.Content?.ToString();
                    if (!string.IsNullOrEmpty(selectedMode))
                    {
                        // Apply the selected power mode for plugged in state
                        string modeGuid = PowerManager.GetPowerModeGuidByName(selectedMode);
                        PowerManager.ApplyPowerMode(modeGuid);
                        System.Threading.Thread.Sleep(500);
                        RefreshInfo();
                        MessageBox.Show($"Plugged in power mode set to: {selectedMode}");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to apply plugged in mode: " + ex.Message);
            }
        }

        private void ApplyOnBatteryMode_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (OnBatteryModeCombo.SelectedItem is ComboBoxItem item)
                {
                    string? selectedMode = item.Content?.ToString();
                    if (!string.IsNullOrEmpty(selectedMode))
                    {
                        // Apply the selected power mode for battery state
                        string modeGuid = PowerManager.GetPowerModeGuidByName(selectedMode);
                        PowerManager.ApplyPowerMode(modeGuid);
                        System.Threading.Thread.Sleep(500);
                        RefreshInfo();
                        MessageBox.Show($"On battery power mode set to: {selectedMode}");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to apply on battery mode: " + ex.Message);
            }
        }
    }

    public class LitePowerPlan
    {
        public string DisplayName { get; set; } = "";
        public string Guid { get; set; } = "";
        public string Description { get; set; } = "";
    }
}

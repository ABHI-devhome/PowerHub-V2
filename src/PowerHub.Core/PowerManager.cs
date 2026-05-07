using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace PowerHub.Core
{
    public static class PowerManager
    {
        public const string BatterySaverOverlayGuid = "961cc777-2547-4f9d-8174-7d86181b8a7a";
        private const string OverlaySubgroupGuidAc = "3b04c4cb-1a9a-493a-8143-ff9c4a94225c";
        private const string OverlaySubgroupGuidDc = "3e00f420-8d3c-4c8d-b004-7ad15a670fc7";
        private const int WM_SETTINGCHANGE = 0x1A;
        private const int WM_POWERBROADCAST = 0x218;
        private const int PBT_APMPOWERSTATUSCHANGE = 0xA;
        private const int SMTO_ABORTIFHUNG = 0x0002;
        private const uint KEYEVENTF_KEYUP = 0x0002;
        private const byte VK_LWIN = 0x5B;
        private const byte VK_A = 0x41;
        private static readonly IntPtr HWND_BROADCAST = new IntPtr(0xffff);

        [DllImport("powrprof.dll", EntryPoint = "PowerSetActiveOverlayScheme")]
        private static extern uint PowerSetActiveOverlayScheme(ref Guid overlaySchemeGuid);

        [DllImport("powrprof.dll", EntryPoint = "PowerGetEffectiveOverlayScheme")]
        private static extern uint PowerGetEffectiveOverlayScheme(out Guid overlaySchemeGuid);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SendMessageTimeout(IntPtr hWnd, int msg, IntPtr wParam, string? lParam, int fuFlags, int uTimeout, out IntPtr lpdwResult);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SendMessageTimeout(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, int fuFlags, int uTimeout, out IntPtr lpdwResult);

        public static string GetActivePowerModeGuid()
        {
            try
            {
                Guid scheme;
                if (PowerGetEffectiveOverlayScheme(out scheme) == 0)
                    return scheme.ToString().ToLowerInvariant();
            }
            catch { }

            return SystemTweaks.RunCommandOutput("powershell", "-NoProfile -Command \"try { (Get-ItemProperty 'HKLM:\\SYSTEM\\CurrentControlSet\\Control\\Power\\User\\PowerSchemes').ActiveOverlayAcPowerScheme } catch { '' }\"").Trim().ToLowerInvariant();
        }

        public static string GetActivePlanName()
        {
            string output = SystemTweaks.RunCommandOutput("powercfg", "/getactivescheme");
            Match match = Regex.Match(output, @"\((.+?)\)\s*$", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            return match.Success ? match.Groups[1].Value.Trim() : string.Empty;
        }

        public static string GetActivePlanGuid()
        {
            string output = SystemTweaks.RunCommandOutput("powercfg", "/getactivescheme");
            Match match = Regex.Match(output, @"Power Scheme GUID:\s+([a-f0-9\-]+)", RegexOptions.IgnoreCase);
            return match.Success ? match.Groups[1].Value.Trim().ToLowerInvariant() : string.Empty;
        }

        public static string NormalizePlanDisplayName(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return string.Empty;
            string normalized = Regex.Replace(text, @"^\W+", string.Empty).Trim();
            normalized = Regex.Replace(normalized, @"\s+\(OEM/Custom\)$", string.Empty, RegexOptions.IgnoreCase).Trim();
            return normalized;
        }

        public static bool IsEcoModeEnabled()
        {
            string output = SystemTweaks.RunCommandOutput("powershell", "-NoProfile -Command \"try { (Get-ItemPropertyValue 'HKLM:\\SYSTEM\\CurrentControlSet\\Control\\Power' -Name EcoModeState) } catch { '' }\"");
            return int.TryParse(output.Trim(), out int state) && state == 1;
        }

        private static void SetEcoModeState(bool enabled)
        {
            string value = enabled ? "1" : "2";
            SystemTweaks.RunCommandSilent("powershell", "-NoProfile -Command \"New-ItemProperty -Path 'HKLM:\\SYSTEM\\CurrentControlSet\\Control\\Power' -Name EcoModeState -PropertyType DWord -Value " + value + " -Force | Out-Null\"");
        }

        private static void ApplyOverlayScheme(string overlayGuid)
        {
            SystemTweaks.RunCommandSilent("powercfg", "/setacvalueindex scheme_current overlay " + OverlaySubgroupGuidAc + " " + overlayGuid);
            SystemTweaks.RunCommandSilent("powercfg", "/setdcvalueindex scheme_current overlay " + OverlaySubgroupGuidDc + " " + overlayGuid);
            SystemTweaks.RunCommandSilent("powercfg", "/setactiveoverlay scheme_current");

            try
            {
                Guid scheme = new Guid(overlayGuid);
                PowerSetActiveOverlayScheme(ref scheme);
            }
            catch { }

            SystemTweaks.RunCommandSilent("powershell", "-NoProfile -Command Set-ItemProperty -Path \"HKLM:\\SYSTEM\\CurrentControlSet\\Control\\Power\\User\\PowerSchemes\" -Name ActiveOverlayAcPowerScheme -Value \"" + overlayGuid + "\"");
            SystemTweaks.RunCommandSilent("powershell", "-NoProfile -Command Set-ItemProperty -Path \"HKLM:\\SYSTEM\\CurrentControlSet\\Control\\Power\\User\\PowerSchemes\" -Name ActiveOverlayDcPowerScheme -Value \"" + overlayGuid + "\"");
        }

        private static void RefreshEnergySaverShell()
        {
            SendMessageTimeout(HWND_BROADCAST, WM_SETTINGCHANGE, IntPtr.Zero, "Power", SMTO_ABORTIFHUNG, 300, out _);
            SendMessageTimeout(HWND_BROADCAST, WM_SETTINGCHANGE, IntPtr.Zero, "Policy", SMTO_ABORTIFHUNG, 300, out _);
            SendMessageTimeout(HWND_BROADCAST, WM_POWERBROADCAST, new IntPtr(PBT_APMPOWERSTATUSCHANGE), IntPtr.Zero, SMTO_ABORTIFHUNG, 300, out _);
            SystemTweaks.RunCommandSilent("taskkill", "/F /IM ShellExperienceHost.exe");
        }

        private static void OpenQuickSettings()
        {
            keybd_event(VK_LWIN, 0, 0, UIntPtr.Zero);
            keybd_event(VK_A, 0, 0, UIntPtr.Zero);
            keybd_event(VK_A, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
            keybd_event(VK_LWIN, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
        }

        private static bool TrySyncQuickSettingsEnergySaver(bool enabled) => false;

        public static void ApplyPowerMode(string guidStr)
        {
            SystemTweaks.RunCommandSilent("powercfg", "/setactive 381b4222-f694-41f0-9685-ff5bb260df2e");
            SetEcoModeState(guidStr.Equals(BatterySaverOverlayGuid, StringComparison.OrdinalIgnoreCase));
            ApplyOverlayScheme(guidStr);
            RefreshEnergySaverShell();
        }

        private static Dictionary<string, string> GetExistingPlans()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            Regex regex = new Regex(@"Power Scheme GUID:\s+([a-f0-9\-]+)\s+\((.+?)\)");
            foreach (Match match in regex.Matches(SystemTweaks.RunCommandOutput("powercfg", "/L")))
            {
                if (!dict.ContainsKey(match.Groups[2].Value))
                    dict[match.Groups[2].Value] = match.Groups[1].Value;
            }
            return dict;
        }

        private static void DisableSleepMode(string guid)
        {
            SystemTweaks.RunCommandSilent("powercfg", "/setacvalueindex " + guid + " 238c9fa8-0aad-41ed-83f4-97be242c8f20 29f6c1db-86da-48c5-9fdb-f2b67b1f44da 0");
            SystemTweaks.RunCommandSilent("powercfg", "/setdcvalueindex " + guid + " 238c9fa8-0aad-41ed-83f4-97be242c8f20 29f6c1db-86da-48c5-9fdb-f2b67b1f44da 0");
            SystemTweaks.RunCommandSilent("powercfg", "/setactive " + guid);
        }

        private static void PreserveDisplaySettings(string guid)
        {
            // Disable display idle timeout (never turn off display)
            // GUID: 7516b95f-f776-4464-8c53-06167f40cc99 (Display)
            // Subgroup: 3c0bc021-c8a8-4e07-a973-6b14cbcb2b7e (Display timeout on AC power)
            // Setting: 3c0bc021-c8a8-4e07-a973-6b14cbcb2b7e (0 = never)
            SystemTweaks.RunCommandSilent("powercfg", "/setacvalueindex " + guid + " 7516b95f-f776-4464-8c53-06167f40cc99 3c0bc021-c8a8-4e07-a973-6b14cbcb2b7e 0");
            SystemTweaks.RunCommandSilent("powercfg", "/setdcvalueindex " + guid + " 7516b95f-f776-4464-8c53-06167f40cc99 3c0bc021-c8a8-4e07-a973-6b14cbcb2b7e 0");
            SystemTweaks.RunCommandSilent("powercfg", "/setactive " + guid);
        }

        public static void ApplyPowerPlan(string targetGuid, string targetName)
        {
            if (targetGuid.Equals(BatterySaverOverlayGuid, StringComparison.OrdinalIgnoreCase) || targetName.Equals("Battery saver", StringComparison.OrdinalIgnoreCase))
            {
                SetEcoModeState(true);
                SystemTweaks.RunCommandSilent("powercfg", "/setdcvalueindex SCHEME_CURRENT SUB_ENERGYSAVER ESBATTTHRESHOLD 100");
                SystemTweaks.RunCommandSilent("powercfg", "/setacvalueindex SCHEME_CURRENT SUB_ENERGYSAVER ESBATTTHRESHOLD 100");
                SystemTweaks.RunCommandSilent("powercfg", "/setactive SCHEME_CURRENT");
                SystemTweaks.RunCommandSilent("powercfg", "/setactive 381b4222-f694-41f0-9685-ff5bb260df2e");
                ApplyOverlayScheme(targetGuid);
                RefreshEnergySaverShell();
                TrySyncQuickSettingsEnergySaver(true);
                return;
            }

            SetEcoModeState(false);
            SystemTweaks.RunCommandSilent("powercfg", "/setdcvalueindex SCHEME_CURRENT SUB_ENERGYSAVER ESBATTTHRESHOLD 20");
            SystemTweaks.RunCommandSilent("powercfg", "/setacvalueindex SCHEME_CURRENT SUB_ENERGYSAVER ESBATTTHRESHOLD 0");
            SystemTweaks.RunCommandSilent("powercfg", "/setactive SCHEME_CURRENT");
            RefreshEnergySaverShell();

            Dictionary<string, string> existing = GetExistingPlans();
            if (existing.TryGetValue(targetName, out string? realGuid))
                targetGuid = realGuid;

            SystemTweaks.RunCommandSilent("powercfg", "/setactive " + targetGuid);
            DisableSleepMode(targetGuid);
            
            // Preserve display settings for high performance modes to prevent display from turning off
            if (targetName.Contains("High performance", StringComparison.OrdinalIgnoreCase) || 
                targetName.Contains("Ultimate Performance", StringComparison.OrdinalIgnoreCase))
            {
                PreserveDisplaySettings(targetGuid);
            }
        }

        public static void ImportPowerPlan(string filename)
        {
            if (!string.IsNullOrWhiteSpace(filename))
                SystemTweaks.RunCommandSilent("powercfg", "-import \"" + filename + "\"");
        }

        public static void UnlockPlan(string name, string guid)
        {
            string output = SystemTweaks.RunCommandOutput("powercfg", "/L");
            if (output.IndexOf(guid, StringComparison.OrdinalIgnoreCase) < 0)
                SystemTweaks.RunCommandSilent("powercfg", "-duplicatescheme " + guid);
        }

        public static void UnlockAllPlans(IEnumerable<KeyValuePair<string, string>> plans)
        {
            foreach (var plan in plans)
                UnlockPlan(plan.Key, plan.Value);
        }

        public static string GetPowerModeDisplayName(string modeGuid)
        {
            string guid = modeGuid.ToLowerInvariant();
            if (guid.Equals(BatterySaverOverlayGuid)) return "Battery Saver";
            if (guid.Equals("961cc777-2547-4f9d-8174-7d86181b8a7a")) return "Battery Saver";
            return "Standard Mode";
        }

        public static string GetPowerModeGuidByName(string modeName)
        {
            if (string.IsNullOrEmpty(modeName))
                return "381b4222-f694-41f0-9685-ff5bb260df2e"; // Default to Balanced
            
            return modeName switch
            {
                "Best Performance" => "e9a42b02-d5df-448d-aa00-03f14749eb61", // Ultimate Performance
                "Balanced" => "381b4222-f694-41f0-9685-ff5bb260df2e",
                "Best Battery Saver" => BatterySaverOverlayGuid,
                _ => "381b4222-f694-41f0-9685-ff5bb260df2e" // Default to Balanced
            };
        }

        public static List<(string Name, string Guid)> GetAvailablePowerModes()
        {
            var modes = new List<(string, string)>
            {
                ("Standard", "00000000-0000-0000-0000-000000000000"),
                ("Battery Saver", BatterySaverOverlayGuid)
            };
            return modes;
        }

        public static void ApplyRefreshRate(int frequency)
        {
            DisplayManager.ApplyRefreshRate(frequency);
        }

        public static int? GetCurrentRefreshRate()
        {
            return DisplayManager.GetCurrentRefreshRate();
        }

        public static List<int> GetAvailableRefreshRates()
        {
            return DisplayManager.GetAvailableRefreshRates();
        }
    }
}

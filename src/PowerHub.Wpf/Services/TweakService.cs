using System;
using PowerHub.Core;

namespace PowerHub.UI.Services
{
    public sealed class TweakService : ITweakService
    {
        private readonly IActivityLogService _log;

        public TweakService(IActivityLogService log)
        {
            _log = log;
        }

        public OperationResult OneClickBoost()
        {
            try
            {
                SystemTweaks.OneClickBoost();
                _log.Add(ActivityKind.Warning, "One-click boost applied (includes SysMain disable).");
                return OperationResult.Ok("One-click boost completed.");
            }
            catch (Exception ex)
            {
                _log.Add(ActivityKind.Error, "One-click boost failed: " + ex.Message);
                return OperationResult.Fail(ex.Message);
            }
        }

        public OperationResult ClearTempFiles()
        {
            try
            {
                SystemTweaks.ClearTempFiles();
                _log.Add(ActivityKind.Success, "Temporary files cleared.");
                return OperationResult.Ok("Temp cleared.");
            }
            catch (Exception ex) { return OperationResult.Fail(ex.Message); }
        }

        public OperationResult FlushDns()
        {
            try
            {
                SystemTweaks.FlushDns();
                _log.Add(ActivityKind.Success, "DNS cache flushed.");
                return OperationResult.Ok("DNS flushed.");
            }
            catch (Exception ex) { return OperationResult.Fail(ex.Message); }
        }

        public OperationResult ClearRam()
        {
            try
            {
                SystemTweaks.ClearRam();
                _log.Add(ActivityKind.Info, "Memory working set trimmed.");
                return OperationResult.Ok("RAM trim requested.");
            }
            catch (Exception ex) { return OperationResult.Fail(ex.Message); }
        }

        public OperationResult EnableFocusMode()
        {
            try
            {
                SystemTweaks.EnableFocusMode();
                _log.Add(ActivityKind.Success, "Focus mode tweaks applied.");
                return OperationResult.Ok("Focus mode applied.");
            }
            catch (Exception ex) { return OperationResult.Fail(ex.Message); }
        }

        public OperationResult DisableTelemetry()
        {
            try
            {
                SystemTweaks.DisableTelemetry();
                _log.Add(ActivityKind.Warning, "DiagTrack telemetry service disabled.");
                return OperationResult.Ok("Telemetry disabled.");
            }
            catch (Exception ex) { return OperationResult.Fail(ex.Message); }
        }

        public OperationResult DisableHibernation()
        {
            try
            {
                SystemTweaks.RunCommandSilent("powercfg", "-h off");
                _log.Add(ActivityKind.Warning, "Hibernation disabled (powercfg -h off).");
                return OperationResult.Ok("Hibernation disabled.");
            }
            catch (Exception ex) { return OperationResult.Fail(ex.Message); }
        }

        public OperationResult SetSystemResponsivenessZero()
        {
            return RunRegTweak("System responsiveness",
                "add \"HKLM\\SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Multimedia\\SystemProfile\" /v SystemResponsiveness /t REG_DWORD /d 0 /f");
        }

        public OperationResult DisableNetworkThrottling()
        {
            return RunRegTweak("Network throttling",
                "add \"HKLM\\SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Multimedia\\SystemProfile\" /v NetworkThrottlingIndex /t REG_DWORD /d ffffffff /f");
        }

        public OperationResult DisableExplorerStartupDelay()
        {
            return RunRegTweak("Explorer startup delay",
                "add \"HKCU\\Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Serialize\" /v StartupDelayInMSec /t REG_DWORD /d 0 /f");
        }

        public OperationResult DisableWindowsAds()
        {
            return RunRegTweak("Windows tips/ads",
                "add \"HKCU\\Software\\Microsoft\\Windows\\CurrentVersion\\ContentDeliveryManager\" /v SubscribedContent-338389Enabled /t REG_DWORD /d 0 /f");
        }

        public OperationResult DisableSearchIndex()
        {
            try
            {
                SystemTweaks.RunCommandSilent("sc", "stop \"WSearch\"");
                SystemTweaks.RunCommandSilent("sc", "config \"WSearch\" start=disabled");
                _log.Add(ActivityKind.Warning, "Windows Search (WSearch) disabled.");
                return OperationResult.Ok("WSearch disabled.");
            }
            catch (Exception ex) { return OperationResult.Fail(ex.Message); }
        }

        public OperationResult DisableSysMainService()
        {
            try
            {
                SystemTweaks.RunCommandSilent("sc", "stop \"SysMain\"");
                SystemTweaks.RunCommandSilent("sc", "config \"SysMain\" start=disabled");
                _log.Add(ActivityKind.Warning, "SysMain service disabled.");
                return OperationResult.Ok("SysMain disabled.");
            }
            catch (Exception ex) { return OperationResult.Fail(ex.Message); }
        }

        public OperationResult ApplyStealthMode()
        {
            try
            {
                string currentOverlay = PowerManager.GetActivePowerModeGuid();
                string currentPlan = PowerManager.GetActivePlanName();
                
                // Robust detection synchronized with WebAppBridge
                bool noOverlay = string.IsNullOrWhiteSpace(currentOverlay) || currentOverlay == "00000000-0000-0000-0000-000000000000";
                bool isStealth = noOverlay && currentPlan.Equals("Balanced", StringComparison.OrdinalIgnoreCase);
                
                if (isStealth)
                {
                    // Disable: Apply High Performance to clearly show change
                    PowerManager.ApplyPowerPlan("8c5e7fda-e8bf-4a96-9a85-a6e23a8c635c", "High performance");
                    _log.Add(ActivityKind.Success, "Stealth mode disabled (Switched to High Performance).");
                    return OperationResult.Ok("Stealth mode disabled.");
                }
                else
                {
                    // Enable: Apply Balanced plan with focus mode tweaks and explicit zero overlay
                    SystemTweaks.EnableFocusMode();
                    PowerManager.ApplyPowerMode("00000000-0000-0000-0000-000000000000");
                    _log.Add(ActivityKind.Warning, "Stealth mode enabled (Balanced + Quiet tweaks).");
                    return OperationResult.Ok("Stealth mode applied.");
                }
            }
            catch (Exception ex) { return OperationResult.Fail(ex.Message); }
        }

        public OperationResult RunRegTweak(string title, string commandLine)
        {
            try
            {
                SystemTweaks.RunCommandSilent("reg", commandLine);
                _log.Add(ActivityKind.Warning, title + " applied.");
                return OperationResult.Ok(title + " applied.");
            }
            catch (Exception ex) { return OperationResult.Fail(ex.Message); }
        }

        public OperationResult RunScCommand(string title, string args)
        {
            try
            {
                SystemTweaks.RunCommandSilent("sc", args);
                _log.Add(ActivityKind.Warning, title + " (sc) executed.");
                return OperationResult.Ok(title + " executed.");
            }
            catch (Exception ex) { return OperationResult.Fail(ex.Message); }
        }
    }
}

using System;
using System.Collections.Generic;
using PowerHub.Core;

namespace PowerHub.UI.Services
{
    public sealed class PowerService : IPowerService
    {
        private readonly IActivityLogService _log;

        public PowerService(IActivityLogService log)
        {
            _log = log;
        }

        public IReadOnlyList<PowerPlanItem> StandardPlans { get; } = new List<PowerPlanItem>
        {
            new PowerPlanItem { DisplayName = "Power saver", Guid = "a1841308-3541-4fab-bc81-f71556f20b4a", Description = "Reduces system performance and conserves energy." },
            new PowerPlanItem { DisplayName = "Battery saver", Guid = PowerManager.BatterySaverOverlayGuid, Description = "Windows overlay for battery / energy saver behavior." },
            new PowerPlanItem { DisplayName = "Balanced", Guid = "381b4222-f694-41f0-9685-ff5bb260df2e", Description = "Balances performance and energy." },
            new PowerPlanItem { DisplayName = "High performance", Guid = "8c5e7fda-e8bf-4a96-9a85-a6e23a8c635c", Description = "Maximizes CPU performance." },
            new PowerPlanItem { DisplayName = "Ultimate Performance", Guid = "e9a42b02-d5df-448d-aa00-03f14749eb61", Description = "Highest performance; removes micro-latencies." }
        };

        public string GetActivePlanGuid()
        {
            try { return PowerManager.GetActivePlanGuid(); }
            catch { return string.Empty; }
        }

        public string GetActivePlanName()
        {
            try { return PowerManager.GetActivePlanName(); }
            catch { return string.Empty; }
        }

        public string GetActivePowerModeGuid()
        {
            try { return PowerManager.GetActivePowerModeGuid(); }
            catch { return string.Empty; }
        }

        public bool IsEcoModeEnabled()
        {
            try { return PowerManager.IsEcoModeEnabled(); }
            catch { return false; }
        }

        public OperationResult ApplyPowerMode(string overlayGuid)
        {
            try
            {
                PowerManager.ApplyPowerMode(overlayGuid);
                _log.Add(ActivityKind.Success, "Power mode updated.");
                return OperationResult.Ok("Windows power mode updated.");
            }
            catch (Exception ex)
            {
                _log.Add(ActivityKind.Error, "Apply power mode failed: " + ex.Message);
                return OperationResult.Fail(ex.Message);
            }
        }

        public OperationResult ApplyPowerPlan(string planGuid, string displayName)
        {
            try
            {
                string normalized = PowerManager.NormalizePlanDisplayName(displayName);
                PowerManager.ApplyPowerPlan(planGuid, normalized);
                string msg = planGuid.Equals(PowerManager.BatterySaverOverlayGuid, StringComparison.OrdinalIgnoreCase) ||
                             normalized.Equals("Battery saver", StringComparison.OrdinalIgnoreCase)
                    ? "Battery saver activated."
                    : "Power plan switched.";
                _log.Add(ActivityKind.Success, msg);
                return OperationResult.Ok(msg);
            }
            catch (Exception ex)
            {
                _log.Add(ActivityKind.Error, "Apply power plan failed: " + ex.Message);
                return OperationResult.Fail(ex.Message);
            }
        }

        public OperationResult UnlockPlan(string name, string guid)
        {
            try
            {
                PowerManager.UnlockPlan(name, guid);
                _log.Add(ActivityKind.Success, name + " plan unlocked.");
                return OperationResult.Ok(name + " is available.");
            }
            catch (Exception ex)
            {
                return OperationResult.Fail(ex.Message);
            }
        }

        public OperationResult UnlockAllPlans()
        {
            try
            {
                var pairs = new List<KeyValuePair<string, string>>();
                foreach (var p in StandardPlans)
                    pairs.Add(new KeyValuePair<string, string>(p.DisplayName, p.Guid));
                PowerManager.UnlockAllPlans(pairs);
                _log.Add(ActivityKind.Success, "All standard plans unlocked.");
                return OperationResult.Ok("Plans unlocked.");
            }
            catch (Exception ex)
            {
                return OperationResult.Fail(ex.Message);
            }
        }

        public OperationResult ImportPowerPlan(string filePath)
        {
            try
            {
                PowerManager.ImportPowerPlan(filePath);
                _log.Add(ActivityKind.Success, "Imported power scheme: " + filePath);
                return OperationResult.Ok("Import completed.");
            }
            catch (Exception ex)
            {
                return OperationResult.Fail(ex.Message);
            }
        }
    }
}

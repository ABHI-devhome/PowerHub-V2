using System.Collections.Generic;

namespace PowerHub.UI.Services
{
    public sealed class PowerPlanItem
    {
        public string DisplayName { get; init; } = string.Empty;
        public string Guid { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
    }

    public interface IPowerService
    {
        IReadOnlyList<PowerPlanItem> StandardPlans { get; }
        string GetActivePlanGuid();
        string GetActivePlanName();
        string GetActivePowerModeGuid();
        bool IsEcoModeEnabled();
        OperationResult ApplyPowerMode(string overlayGuid);
        OperationResult ApplyPowerPlan(string planGuid, string displayName);
        OperationResult UnlockPlan(string name, string guid);
        OperationResult UnlockAllPlans();
        OperationResult ImportPowerPlan(string filePath);
    }
}

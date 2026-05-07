namespace PowerHub.UI.Services
{
    public interface ITweakService
    {
        OperationResult OneClickBoost();
        OperationResult ClearTempFiles();
        OperationResult FlushDns();
        OperationResult ClearRam();
        OperationResult EnableFocusMode();
        OperationResult DisableTelemetry();
        OperationResult DisableHibernation();
        OperationResult SetSystemResponsivenessZero();
        OperationResult DisableNetworkThrottling();
        OperationResult DisableExplorerStartupDelay();
        OperationResult DisableWindowsAds();
        OperationResult DisableSearchIndex();
        OperationResult DisableSysMainService();
        OperationResult ApplyStealthMode();
        OperationResult RunRegTweak(string title, string commandLine);
        OperationResult RunScCommand(string title, string args);
    }
}

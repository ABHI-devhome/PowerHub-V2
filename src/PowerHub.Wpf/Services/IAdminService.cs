namespace PowerHub.UI.Services
{
    public enum ElevationRequestResult
    {
        Relaunched,
        Cancelled,
        Failed
    }

    public interface IAdminService
    {
        bool IsRunningAsAdministrator();
        ElevationRequestResult RequestRestartElevated(out string message);
    }
}

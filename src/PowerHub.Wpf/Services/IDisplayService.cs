using System.Collections.Generic;

namespace PowerHub.UI.Services
{
    public interface IDisplayService
    {
        IReadOnlyList<int> GetAvailableRefreshRates();
        int? GetCurrentRefreshRate();
        OperationResult ApplyRefreshRate(int hz);
    }
}

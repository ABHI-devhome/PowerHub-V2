using System;
using System.Collections.Generic;
using PowerHub.Core;

namespace PowerHub.UI.Services
{
    public sealed class DisplayService : IDisplayService
    {
        private readonly IActivityLogService _log;

        public DisplayService(IActivityLogService log)
        {
            _log = log;
        }

        public IReadOnlyList<int> GetAvailableRefreshRates() => DisplayManager.GetAvailableRefreshRates();

        public int? GetCurrentRefreshRate() => DisplayManager.GetCurrentRefreshRate();

        public OperationResult ApplyRefreshRate(int hz)
        {
            int code = DisplayManager.ApplyRefreshRate(hz);
            if (code == 0)
            {
                _log.Add(ActivityKind.Success, "Refresh rate set to " + hz + " Hz.");
                return OperationResult.Ok("Display set to " + hz + " Hz.");
            }
            _log.Add(ActivityKind.Error, "Refresh rate change failed. Code: " + code);
            return OperationResult.Fail("ChangeDisplaySettings returned " + code);
        }
    }
}

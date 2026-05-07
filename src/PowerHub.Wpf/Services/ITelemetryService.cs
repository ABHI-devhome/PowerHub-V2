using System.Collections.Generic;

namespace PowerHub.UI.Services
{
    public sealed class TelemetrySnapshot
    {
        public float? CpuUsagePercent { get; set; }
        public float? GpuUsagePercent { get; set; }
        public float? CpuTemperatureC { get; set; }
        public float? GpuTemperatureC { get; set; }
        public float? CpuPackagePowerW { get; set; }
        public IReadOnlyList<float> PowerFluxHistory { get; set; } = new float[0];
    }

    public interface ITelemetryService
    {
        TelemetrySnapshot GetSnapshot();
    }
}

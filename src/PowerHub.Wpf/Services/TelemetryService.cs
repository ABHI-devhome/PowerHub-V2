using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using LibreHardwareMonitor.Hardware;

namespace PowerHub.UI.Services
{
    public sealed class TelemetryService : ITelemetryService, IDisposable
    {
        private readonly object _gate = new object();
        private readonly Computer _computer;
        private readonly PerformanceCounter _cpuCounter;
        private DateTime _lastLightSampleUtc = DateTime.MinValue;
        private DateTime _lastHeavySampleUtc = DateTime.MinValue;
        private readonly Queue<float> _powerFlux = new Queue<float>();
        private TelemetrySnapshot _snapshot = new TelemetrySnapshot();

        public TelemetryService()
        {
            _computer = new Computer
            {
                IsCpuEnabled = true,
                IsGpuEnabled = true,
                IsMotherboardEnabled = true
            };
            _computer.Open();

            _cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            _cpuCounter.NextValue();
        }

        public TelemetrySnapshot GetSnapshot()
        {
            lock (_gate)
            {
                var now = DateTime.UtcNow;
                if ((now - _lastLightSampleUtc).TotalSeconds >= 2)
                {
                    _snapshot.CpuUsagePercent = Clamp(_cpuCounter.NextValue());
                    _lastLightSampleUtc = now;
                }

                if ((now - _lastHeavySampleUtc).TotalSeconds >= 5)
                {
                    RefreshHardwareSensors();
                    _lastHeavySampleUtc = now;
                }

                float flux = (_snapshot.CpuPackagePowerW ?? 0f) +
                             ((_snapshot.CpuUsagePercent ?? 0f) * 0.25f) +
                             ((_snapshot.GpuUsagePercent ?? 0f) * 0.18f);
                EnqueueFlux(flux);

                return new TelemetrySnapshot
                {
                    CpuUsagePercent = _snapshot.CpuUsagePercent,
                    GpuUsagePercent = _snapshot.GpuUsagePercent,
                    CpuTemperatureC = _snapshot.CpuTemperatureC,
                    GpuTemperatureC = _snapshot.GpuTemperatureC,
                    CpuPackagePowerW = _snapshot.CpuPackagePowerW,
                    PowerFluxHistory = _powerFlux.ToArray()
                };
            }
        }

        private void RefreshHardwareSensors()
        {
            float? cpuTemp = null;
            float? gpuTemp = null;
            float? gpuLoad = null;
            float? cpuPower = null;

            foreach (var hardware in _computer.Hardware)
            {
                hardware.Update();
                foreach (var subHardware in hardware.SubHardware)
                    subHardware.Update();

                bool isCpu = hardware.HardwareType == HardwareType.Cpu;
                bool isGpu = hardware.HardwareType == HardwareType.GpuAmd ||
                             hardware.HardwareType == HardwareType.GpuIntel ||
                             hardware.HardwareType == HardwareType.GpuNvidia;

                foreach (var sensor in hardware.Sensors.Concat(hardware.SubHardware.SelectMany(x => x.Sensors)))
                {
                    if (!sensor.Value.HasValue) continue;

                    if (isCpu && sensor.SensorType == SensorType.Temperature && cpuTemp == null)
                        cpuTemp = sensor.Value.Value;

                    if (isGpu && sensor.SensorType == SensorType.Temperature && gpuTemp == null)
                        gpuTemp = sensor.Value.Value;

                    if (isGpu && sensor.SensorType == SensorType.Load)
                        gpuLoad = Math.Max(gpuLoad ?? 0f, sensor.Value.Value);

                    if (isCpu && sensor.SensorType == SensorType.Power && sensor.Name.IndexOf("package", StringComparison.OrdinalIgnoreCase) >= 0)
                        cpuPower = sensor.Value.Value;
                }
            }

            _snapshot.CpuTemperatureC = cpuTemp;
            _snapshot.GpuTemperatureC = gpuTemp;
            _snapshot.GpuUsagePercent = gpuLoad;
            _snapshot.CpuPackagePowerW = cpuPower;
        }

        private void EnqueueFlux(float value)
        {
            if (float.IsNaN(value) || float.IsInfinity(value)) value = 0f;
            _powerFlux.Enqueue(MathF.Round(value, 1));
            while (_powerFlux.Count > 24)
                _powerFlux.Dequeue();
        }

        private static float Clamp(float value)
        {
            if (float.IsNaN(value) || float.IsInfinity(value)) return 0f;
            if (value < 0) return 0;
            if (value > 100) return 100;
            return value;
        }

        public void Dispose()
        {
            _cpuCounter.Dispose();
            _computer.Close();
        }
    }
}

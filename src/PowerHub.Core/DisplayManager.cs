using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace PowerHub.Core
{
    public static class DisplayManager
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct DEVMODE
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)] public string dmDeviceName;
            public short dmSpecVersion;
            public short dmDriverVersion;
            public short dmSize;
            public short dmDriverExtra;
            public int dmFields;
            public int dmPositionX;
            public int dmPositionY;
            public int dmDisplayOrientation;
            public int dmDisplayFixedOutput;
            public short dmColor;
            public short dmDuplex;
            public short dmYResolution;
            public short dmTTOption;
            public short dmCollate;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)] public string dmFormName;
            public short dmLogPixels;
            public int dmBitsPerPel;
            public int dmPelsWidth;
            public int dmPelsHeight;
            public int dmDisplayFlags;
            public int dmDisplayFrequency;
        }

        private const int ENUM_CURRENT_SETTINGS = -1;
        private const int DM_DISPLAYFREQUENCY = 0x400000;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern bool EnumDisplaySettings(string? lpszDeviceName, int iModeNum, ref DEVMODE lpDevMode);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int ChangeDisplaySettings(ref DEVMODE lpDevMode, int dwFlags);

        public static List<int> GetAvailableRefreshRates()
        {
            DEVMODE mode = new DEVMODE();
            mode.dmSize = (short)Marshal.SizeOf(typeof(DEVMODE));
            int index = 0;
            List<int> rates = new List<int>();
            while (EnumDisplaySettings(null, index, ref mode))
            {
                if (!rates.Contains(mode.dmDisplayFrequency) && mode.dmDisplayFrequency > 1)
                    rates.Add(mode.dmDisplayFrequency);
                index++;
            }
            rates.Sort();
            return rates;
        }

        public static int? GetCurrentRefreshRate()
        {
            DEVMODE mode = new DEVMODE();
            mode.dmSize = (short)Marshal.SizeOf(typeof(DEVMODE));
            if (!EnumDisplaySettings(null, ENUM_CURRENT_SETTINGS, ref mode))
                return null;
            return mode.dmDisplayFrequency;
        }

        public static int ApplyRefreshRate(int frequency)
        {
            DEVMODE mode = new DEVMODE();
            mode.dmSize = (short)Marshal.SizeOf(typeof(DEVMODE));
            if (!EnumDisplaySettings(null, ENUM_CURRENT_SETTINGS, ref mode))
                return -1;

            mode.dmDisplayFrequency = frequency;
            mode.dmFields = DM_DISPLAYFREQUENCY;
            return ChangeDisplaySettings(ref mode, 0);
        }

        public static bool IsRefreshRateApplySuccessful(int result)
        {
            // ChangeDisplaySettings returns DISP_CHANGE_SUCCESSFUL (0) on success
            return result == 0;
        }
    }
}

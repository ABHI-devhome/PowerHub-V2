using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace PowerHub.Core
{
    public static class SystemTweaks
    {
        [DllImport("psapi.dll")]
        public static extern int EmptyWorkingSet(IntPtr hwProc);

        public static void RunCommandSilent(string process, string args)
        {
            try { Process.Start(new ProcessStartInfo(process, args) { CreateNoWindow = true, UseShellExecute = false })?.WaitForExit(); } catch { }
        }

        public static string RunCommandOutput(string process, string args)
        {
            try
            {
                Process? p = Process.Start(new ProcessStartInfo(process, args) { CreateNoWindow = true, UseShellExecute = false, RedirectStandardOutput = true });
                if (p == null) return string.Empty;
                string output = p.StandardOutput.ReadToEnd();
                p.WaitForExit();
                return output;
            }
            catch { return string.Empty; }
        }

        public static void ClearTempFiles()
        {
            try
            {
                foreach (string file in Directory.GetFiles(Path.GetTempPath()))
                {
                    try { File.Delete(file); } catch { }
                }
            }
            catch { }
        }

        public static void FlushDns()
        {
            RunCommandSilent("ipconfig", "/flushdns");
        }

        public static void DisableTelemetry()
        {
            RunCommandSilent("sc", "config DiagTrack start= disabled");
            RunCommandSilent("sc", "stop DiagTrack");
        }

        public static void ClearRam()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            try { EmptyWorkingSet(Process.GetCurrentProcess().Handle); } catch { }
        }

        public static void EnableFocusMode()
        {
            RunCommandSilent("reg", "add \"HKCU\\Software\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize\" /v EnableTransparency /t REG_DWORD /d 0 /f");
            RunCommandSilent("reg", "add \"HKCU\\Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\VisualEffects\" /v VisualFXSetting /t REG_DWORD /d 2 /f");
            RunCommandSilent("reg", "add \"HKCU\\Software\\Microsoft\\Windows\\CurrentVersion\\BackgroundAccessApplications\" /v GlobalUserDisabled /t REG_DWORD /d 1 /f");
        }

        public static void DisableFocusMode()
        {
            RunCommandSilent("reg", "add \"HKCU\\Software\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize\" /v EnableTransparency /t REG_DWORD /d 1 /f");
            RunCommandSilent("reg", "add \"HKCU\\Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\VisualEffects\" /v VisualFXSetting /t REG_DWORD /d 0 /f");
            RunCommandSilent("reg", "add \"HKCU\\Software\\Microsoft\\Windows\\CurrentVersion\\BackgroundAccessApplications\" /v GlobalUserDisabled /t REG_DWORD /d 0 /f");
        }

        public static void OneClickBoost()
        {
            ClearTempFiles();
            FlushDns();
            ClearRam();
            EnableFocusMode();
            RunCommandSilent("reg", "add \"HKLM\\SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Multimedia\\SystemProfile\" /v SystemResponsiveness /t REG_DWORD /d 0 /f");
            RunCommandSilent("reg", "add \"HKLM\\SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Multimedia\\SystemProfile\" /v NetworkThrottlingIndex /t REG_DWORD /d ffffffff /f");
            RunCommandSilent("sc", "stop \"SysMain\"");
            RunCommandSilent("sc", "config \"SysMain\" start=disabled");
        }
    }
}

using System;
using System.Diagnostics;
using System.Reflection;
using System.Security.Principal;
using System.Windows;

namespace PowerHub.UI.Services
{
    public sealed class AdminService : IAdminService
    {
        public bool IsRunningAsAdministrator()
        {
            try
            {
                using var identity = WindowsIdentity.GetCurrent();
                var principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            catch
            {
                return false;
            }
        }

        public ElevationRequestResult RequestRestartElevated(out string message)
        {
            try
            {
                string path = Environment.ProcessPath ?? String.Empty;
                var process = Process.Start(new ProcessStartInfo
                {
                    UseShellExecute = true,
                    WorkingDirectory = Environment.CurrentDirectory,
                    FileName = path,
                    Verb = "runas"
                });
                if (process != null)
                {
                    message = "PowerHub relaunched with administrator privileges.";
                    return ElevationRequestResult.Relaunched;
                }

                message = "PowerHub could not start the elevated process.";
                return ElevationRequestResult.Failed;
            }
            catch (System.ComponentModel.Win32Exception ex) when (ex.NativeErrorCode == 1223)
            {
                message = "PowerHub needs administrator permission to change power plans, display settings, and system tweaks. UAC was cancelled before the elevated app could open.";
                return ElevationRequestResult.Cancelled;
            }
            catch (Exception ex)
            {
                message = "PowerHub needs administrator permission to change power plans, display settings, and system tweaks. The elevated launch failed: " + ex.Message;
                return ElevationRequestResult.Failed;
            }
        }
    }
}

using Microsoft.Win32.SafeHandles;
using System;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Threading.Tasks;

namespace EasyOC.Core
{
    public class WindowsUserLoginInfo
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string DomainName { get; set; }
    }
    public class RunAsDifferentUserPrincipal
    {
        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool LogonUser(String lpszUsername, String lpszDomain, String lpszPassword,
           int dwLogonType, int dwLogonProvider, out SafeAccessTokenHandle phToken);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public extern static bool CloseHandle(IntPtr handle);

        const int LOGON32_PROVIDER_DEFAULT = 0;

        const int LOGON32_LOGON_INTERACTIVE = 2;

        public static async Task RunImpersonatedAsync(WindowsUserLoginInfo windowsUserLogin, Func<Task> func)
        {
            SafeAccessTokenHandle safeAccessTokenHandle;
            bool returnValue = LogonUser(windowsUserLogin.UserName, windowsUserLogin.DomainName, windowsUserLogin.Password,
                LOGON32_LOGON_INTERACTIVE, LOGON32_PROVIDER_DEFAULT,
                out safeAccessTokenHandle);

            if (false == returnValue)
            {
                int ret = Marshal.GetLastWin32Error();
                Console.WriteLine("LogonUser failed with error code : {0}", ret);
                throw new System.ComponentModel.Win32Exception(ret);
            }
#pragma warning disable CA1416 // 验证平台兼容性
            Console.WriteLine("模拟前 User:" + WindowsIdentity.GetCurrent().Name);
            await WindowsIdentity.RunImpersonatedAsync(safeAccessTokenHandle, async () =>
            {
                Console.WriteLine("模拟 User:" + WindowsIdentity.GetCurrent().Name);
                await func();
            });
#pragma warning restore CA1416 // 验证平台兼容性

        }
    }
}




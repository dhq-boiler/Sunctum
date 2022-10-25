using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Windows.Security.Credentials;
using Windows.Security.Credentials.UI;

namespace Sunctum.Domain.Logic.Encrypt
{
#pragma warning disable CA1416
    public static class PasswordManager
    {
        public static void SetPassword(string libraryId, string password, string userName)
        {
            PasswordVault myVault = new PasswordVault();
            myVault.Add(new PasswordCredential(libraryId, userName, password));
        }

        public static void RemovePassword(string libraryId, string userName)
        {
            PasswordVault myVault = new PasswordVault();

            var password = myVault.Retrieve(libraryId, userName);
            if (password != null)
                myVault.Remove(password);
        }

        public static async Task<string> SignInAsync(string libraryId, string userName)
        {
            var result = await UserConsentVerifierInterop.RequestVerificationForWindowAsync(Process.GetCurrentProcess().MainWindowHandle, "このライブラリは暗号化されています。閲覧するには資格情報が必要です。");
            if (result != UserConsentVerificationResult.Verified)
                return null;

            try
            {
                var vault = new PasswordVault();
                var credentials = vault.Retrieve(libraryId, userName);
                return credentials?.Password;
            }
            catch (COMException)
            {
                return null;
            }
        }
    }
#pragma warning restore CA1416
}

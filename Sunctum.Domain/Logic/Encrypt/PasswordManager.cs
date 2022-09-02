using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Credentials;
using Windows.Security.Credentials.UI;

namespace Sunctum.Domain.Logic.Encrypt
{
    public static class PasswordManager
    {
        private const string AppID = "ADC698D1-29E4-4347-BD7D-A8397AB5AE66";

        public static void SetPassword(string password, string userName)
        {
            PasswordVault myVault = new PasswordVault();
            myVault.Add(new PasswordCredential(AppID, userName, password));
        }

        public static void RemovePassword(string userName)
        {
            PasswordVault myVault = new PasswordVault();

            var password = myVault.Retrieve(AppID, userName);
            if (password != null)
                myVault.Remove(password);
        }

        public static async Task<string> SignInAsync(string userName)
        {
            var result = await UserConsentVerifier.RequestVerificationAsync("このライブラリは暗号化されています。閲覧するには資格情報が必要です。");
            if (result != UserConsentVerificationResult.Verified)
                return null;

            var vault = new PasswordVault();
            var credentials = vault.Retrieve(AppID, userName);

            return credentials?.Password;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.XtraEditors;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using DevExpress.XtraSplashScreen;
using DevExpress.XtraEditors;
using System.Windows.Forms;
using System.Net;
using System.Security.Cryptography;
namespace ERP_Exact_Machine.Utils
{
    class Tools
    {
        public static XtraForm currentForm;
        public static bool deencode = true;
        public static class Api
        {
            public static bool ServerCertificate = Config.ServerCertificate;
            private static string urlHost = Config.urlHost;
            public static HttpResponseMessage call(string url , string type ="get", string content="",
                bool checkToken=true, bool autoGetToken=true, bool showWaiting = true)
            {
                string error = "";
                if (showWaiting)
                {
                    SplashScreenManager.ShowForm(currentForm, typeof(WaitForm1), true, true, false);
                    SplashScreenManager.Default.SetWaitFormCaption("Please wait");
                    SplashScreenManager.Default.SetWaitFormDescription("System is loading data");
                }
                if (!ServerCertificate)
                {
                    ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
                    ServerCertificate = true;
                }
                HttpResponseMessage res = new HttpResponseMessage();
                res.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                HttpClient client = new HttpClient();
                try
                {
                    url = url.Replace("%", "%25");
                    client.BaseAddress = new Uri(urlHost);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json");
                    if (checkToken)
                    {
                        DateTime now =  DateTime.Now;

                    }
                }
                catch (Exception ex)
                {
                    if (showWaiting)
                    {
                        SplashScreenManager.CloseForm();
                        MessageBox.Show("Can not connect to Server, please check your connection. Thank you");
                        return res;
                    }
                }
            }
        }
        public static string TripleDesDecrypt(string endcodedText, string key=null)
        {
            string plaintext = endcodedText;
            if (key == null)
            {
                key = Utils.loginInfo.access_token;
            }
            try
            {
                if (deencode)
                {
                    TripleDESCryptoServiceProvider desCryptoServiceProvider = new TripleDESCryptoServiceProvider();
                    MD5CryptoServiceProvider hashMD5Provider = new MD5CryptoServiceProvider();
                    byte[] byteHash;
                    byte[] byteBuff;
                    byteHash = hashMD5Provider.ComputeHash(Encoding.UTF8.GetBytes(key));
                    desCryptoServiceProvider.Key = byteHash;
                    desCryptoServiceProvider.Mode = CipherMode.ECB;
                    byteBuff = Convert.FromBase64String(endcodedText);
                    plaintext = Encoding.UTF8.GetString(desCryptoServiceProvider.CreateDecryptor().TransformFinalBlock(byteBuff, 0, byteBuff.Length));
                }
            }
            catch (Exception ex)
            {

            }
            return plaintext;
        }

    }
}

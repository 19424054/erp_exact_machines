using System;
using System.Collections.Generic;
using System.Drawing;
namespace ERP_Exact_Machine.Utils
{
   public static class loginInfo
    {
        public static string user_id;
        public static string full_name;
        public static string account_id;
        public static string user_name;
        public static string password;
        public static string company_id;
        public static DateTime expiry_token;
        public static string access_token;
        public static string user_code;
        public static string avatar;
        public static string currency_id;
        public static string currency_name;
        public static Image logoCompany;
        public static string mobileCompany;
        public static string faxCompany;
        public static string taxCompany;
        public static Image imgAvatar;
        public static bool isAdmin = false;
        
        //Get info Company
        public static Dictionary<string,object> getinfocompany(string language)
        {
            Dictionary<string, object> list = new Dictionary<string, object>();

            list.Add("address", "");
            list.Add("name", "");
            list.Add("fax", "");
            list.Add("tax", "");
            list.Add("mobile", "");
            list.Add("logo", null);
            //Get api company
            var res = Utils.Tools.Api.call("api/langrels/laydanhsachtheocompany_idvalanguagecode/-1/-1/null/null"+Utils.loginInfo.company_id+"/"+language);
            


        }
    }
}

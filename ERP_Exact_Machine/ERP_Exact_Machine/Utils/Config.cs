
namespace ERP_Exact_Machine.Utils
{
   public static class Config
    {
        public static string urlHost = "http://192.168.3.168:5000";
        //Verison
        public static string sersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.Major.ToString()
            + "." + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.Minor.ToString()
            + "." + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.Build.ToString()
            + "." + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.Revision.ToString();
        //Email
        public static bool ServerCertificate = false;
        //Folder direction
        public static string direction_draw_file = "product/drawfile";
        public static string direction_employee_avatar = "Employees";
        public static string dicrection_product_avatar = "products";

        public static bool isEncrypt = false;
    }
}

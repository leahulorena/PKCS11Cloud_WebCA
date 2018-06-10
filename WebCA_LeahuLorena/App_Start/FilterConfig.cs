using System.Web;
using System.Web.Mvc;

namespace WebCA_LeahuLorena
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}

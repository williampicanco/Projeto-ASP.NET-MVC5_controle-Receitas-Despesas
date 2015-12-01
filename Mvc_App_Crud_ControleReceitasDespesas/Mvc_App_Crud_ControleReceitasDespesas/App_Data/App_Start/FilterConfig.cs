using System.Web;
using System.Web.Mvc;

namespace Mvc_App_Crud_ControleReceitasDespesas
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}

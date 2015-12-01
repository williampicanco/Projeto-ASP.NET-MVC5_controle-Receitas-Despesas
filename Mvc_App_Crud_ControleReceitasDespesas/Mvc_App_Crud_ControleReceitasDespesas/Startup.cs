using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Mvc_App_Crud_ControleReceitasDespesas.Startup))]
namespace Mvc_App_Crud_ControleReceitasDespesas
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

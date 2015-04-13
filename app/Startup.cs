using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Routing;
using Microsoft.AspNet.Mvc;
using Microsoft.Framework.DependencyInjection;

namespace uwcua
{
    public class Startup
    {
        public void Configure(IApplicationBuilder app)
        {
            app.UseServices(services =>
            {
                services.AddMvc();
            });

            app.UseMvc(routes =>
           {
               routes.MapRoute(
                    name: "default",
                    template: "{action}/{id?}",
                    defaults: new { controller = "Home", action = "Index" });

               routes.MapRoute(
                   name: "Error", 
                   template: "{*url}", 
                   defaults: new { controller = "Home", action = "Error"});
           });
        }       
    }
}
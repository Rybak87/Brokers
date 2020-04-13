using System.Web.Http;

namespace MessagesAPI
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "Author",
                routeTemplate: "api/GetTopAuthorsByViews",
                defaults: new { controller = "author"}
            );
        }
    }
}

using System.Linq;
using System.Net;
using System.Web.Http;

public partial class WebAPIConfig
{
    public static void Register(HttpConfiguration config)
    {
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        // Web API 設定和服務

        // Web API 路由
        config.MapHttpAttributeRoutes();

        //config.Routes.MapHttpRoute(
        //    name: "DefaultApi",
        //    routeTemplate: "api/{controller}/{id}",
        //    defaults: new { id = RouteParameter.Optional }
        //);

        config.Routes.MapHttpRoute(
            name: "ActionApi",
            routeTemplate: "api/{controller}/{action}"          
        );

        var appXmlType = config.Formatters.XmlFormatter.SupportedMediaTypes.FirstOrDefault(t => t.MediaType == "application/xml");
        config.Formatters.XmlFormatter.SupportedMediaTypes.Remove(appXmlType);

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace MvcApi
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(
                name: "DefaultApi2",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );


            // api cors 跨域
            config.Filters.Add(new MvcApi.Common.CorsFilter("*"));

            //记录日志
            log4net.Config.XmlConfigurator.Configure();
        }
    }
}

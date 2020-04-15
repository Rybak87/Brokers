using log4net.Config;
using MessagesAPI.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace MessagesAPI
{
    public class WebApiApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AutofacConfig.ConfigureContainer();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            XmlConfigurator.Configure();
        }
    }
}

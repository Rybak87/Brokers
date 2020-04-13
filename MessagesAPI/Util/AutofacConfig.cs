﻿using Autofac;
using Autofac.Integration.WebApi;
using Brokers.DAL.Elastic;
using Brokers.DAL.Interfaces;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Http.SelfHost;

namespace MessagesAPI.Util
{
    public class AutofacConfig
    {
        public static void ConfigureContainer()
        {
            var builder = new ContainerBuilder();
            var config = GlobalConfiguration.Configuration;
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            builder.RegisterType<ElasticCalculation>().As<IMessagesReportBuilder>();
            var settElastic = new ConnectionSettings(new Uri("http://localhost:9200")).DefaultIndex("messages");
            builder.RegisterType<ElasticClient>()
                .WithParameter("connectionSettings", settElastic)
                .SingleInstance();

            var container = builder.Build();
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
        }
    }
}
using Microsoft.Owin;

[assembly: OwinStartup(typeof(IIS.SampleForMetrics.Startup))]
namespace IIS.SampleForMetrics
{
    using App.Metrics;
    using App.Metrics.Extensions.Owin.DependencyInjection.Options;
    using App.Metrics.Extensions.Owin.WebApi;
    using ICSSoft.STORMNET.Business;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Practices.Unity.Configuration;
    using Owin;
    using System.IO;
    using System.Web.Http;
    using Unity;

    public class Startup
    {
        public void Configuration(IAppBuilder appBuilder)
        {
            IUnityContainer container = new UnityContainer();
            container.LoadConfiguration();
            container.RegisterInstance(DataServiceProvider.DataService);
            GlobalConfiguration.Configure(configuration => ODataConfig.Configure(configuration, container, GlobalConfiguration.DefaultServer));

            var httpConfiguration = new HttpConfiguration();
            httpConfiguration.MessageHandlers.Add(new MetricsWebApiMessageHandler());
            httpConfiguration.RegisterWebApi();

            var services = new ServiceCollection();
            ConfigureServices(services);

            var provider = services.SetDependencyResolver(httpConfiguration);
            appBuilder.UseMetrics(provider);
            appBuilder.UseMetricsReporting(provider);
            appBuilder.UseWebApi(httpConfiguration);
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging();
            services.AddMetricsMiddleware();
            services.AddControllersAsServices();

            services.AddSingleton(new MetricsOptions());
            services.AddSingleton(new OwinMetricsOptions());

            services.AddMetrics(x =>
            {
                x.Report.ToTextFile(o => o.OutputPathAndFileName = Path.Combine(Path.GetTempPath(), "metrics.txt"));
                x.OutputMetrics.AsJson();
            });

        }
    }
}
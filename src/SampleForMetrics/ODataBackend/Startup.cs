using Microsoft.Owin;

[assembly: OwinStartup(typeof(IIS.SampleForMetrics.Startup))]
namespace IIS.SampleForMetrics
{
    using App.Metrics;
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
            services.AddControllersAsServices();

            services.AddMetrics(
                metricsOptions =>
                {
                    metricsOptions.Report.ToTextFile(o => o.OutputPathAndFileName = Path.Combine(Path.GetTempPath(), "metrics.txt"));
                    metricsOptions.OutputMetrics.AsJson();
                },
                owinMetricsOptions => {
                    owinMetricsOptions.MetricsEndpointEnabled = true;
                    owinMetricsOptions.IgnoredRoutesRegexPatterns = new []{ "metrics" };
                }
            );

        }
    }
}
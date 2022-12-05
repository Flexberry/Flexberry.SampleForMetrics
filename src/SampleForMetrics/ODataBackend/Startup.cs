using Microsoft.Owin;

[assembly: OwinStartup(typeof(IIS.SampleForMetrics.Startup))]
namespace IIS.SampleForMetrics
{
    using App.Metrics;
    using App.Metrics.Extensions.Owin.WebApi;
    using App.Metrics.Extensions.Reporting.TextFile;
    using App.Metrics.Reporting.Abstractions;
    using ICSSoft.STORMNET.Business;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Practices.Unity.Configuration;
    using Owin;
    using System;
    using System.IO;
    using System.Reflection;
    using System.Threading.Tasks;
    using System.Web.Hosting;
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
            //DEVNOTE: If already using Autofac for example for DI, you would just build the 
            // servicecollection, resolve IMetrics and register that with your container instead.
            var provider = services.SetDependencyResolver(httpConfiguration);

            appBuilder.UseMetrics(provider);

            HostingEnvironment.QueueBackgroundWorkItem(cancellationToken =>
            {
                var reportFactory = provider.GetRequiredService<IReportFactory>();
                var metrics = provider.GetRequiredService<IMetrics>();
                var reporter = reportFactory.CreateReporter();
                reporter.RunReports(metrics, cancellationToken);
            });

            appBuilder.UseWebApi(httpConfiguration);
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging();

            services.AddControllersAsServices();
            var metricsFilePath = Path.Combine(Path.GetTempPath(), "metrics.txt");
            services
                .AddMetrics(options =>
                {
                    options.DefaultContextLabel = "SampleForMetrics.ODataBackend";
                    options.ReportingEnabled = true;
                }, Assembly.GetExecutingAssembly().GetName())
                .AddReporting(factory=>{
                    factory.AddTextFile(new TextFileReporterSettings {
                        FileName = metricsFilePath,
                        ReportInterval = TimeSpan.FromSeconds(10)
                    });
                })
                .AddHealthChecks(factory =>
                {
                    factory.RegisterProcessPrivateMemorySizeHealthCheck("Private Memory Size", 200);
                    factory.RegisterProcessVirtualMemorySizeHealthCheck("Virtual Memory Size", 200);
                    factory.RegisterProcessPhysicalMemoryHealthCheck("Working Set", 200);
                    factory.Register("MetricsFilePath:", () => Task.FromResult(metricsFilePath));
                })
                .AddJsonSerialization()
                .AddMetricsMiddleware(options =>
                {

                });
        }
    }
}
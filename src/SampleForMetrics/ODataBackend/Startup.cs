using Microsoft.Owin;

[assembly: OwinStartup(typeof(IIS.SampleForMetrics.Startup))]
namespace IIS.SampleForMetrics
{
    using App.Metrics;
    using App.Metrics.Extensions.Owin.DependencyInjection.Options;
    using App.Metrics.Extensions.Owin.Middleware;
    using ICSSoft.STORMNET.Business;
    using Microsoft.Practices.Unity.Configuration;
    using Owin;
    using System;
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
            AddMetrics(container);
            UseMetrics(appBuilder, container);
        }

        public void AddMetrics(IUnityContainer container)
        {
            var metricsBuilder = new MetricsBuilder();
            var metrics = metricsBuilder.Build();
            container.RegisterInstance(metrics.Options);
            container.RegisterInstance(metrics.DefaultOutputMetricsFormatter);
            container.RegisterInstance<IMetrics>(metrics);
            container.RegisterInstance<IMetricsRoot>(metrics);
        }

        public static IAppBuilder UseMetrics(IAppBuilder app, IUnityContainer provider)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            var appMetricsOptions = provider.Resolve<MetricsOptions>();
            var owinMetricsOptions = provider.Resolve<OwinMetricsOptions>();

            if (appMetricsOptions.Enabled)
            {
                app.Use(provider.Resolve<ErrorRequestMeterMiddleware>());
                app.Use(provider.Resolve<PingEndpointMiddleware>());
                app.Use(provider.Resolve<PerRequestTimerMiddleware>());
                app.Use(provider.Resolve<PostAndPutRequestSizeHistogramMiddleware>());
                app.Use(provider.Resolve<RequestTimerMiddleware>());
                app.Use(provider.Resolve<ApdexMiddleware>());
                app.Use(provider.Resolve<ActiveRequestCounterEndpointMiddleware>());
            }

            if (owinMetricsOptions.MetricsEndpointEnabled && appMetricsOptions.Enabled)
            {
                app.Use(provider.Resolve<MetricsEndpointMiddleware>());
            }

            return app;
        }
    }

}
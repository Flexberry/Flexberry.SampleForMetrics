using Microsoft.Owin;

[assembly: OwinStartup(typeof(IIS.SampleForMetrics.Startup))]
namespace IIS.SampleForMetrics
{
    using ICSSoft.STORMNET.Business;
    using Microsoft.Practices.Unity.Configuration;
    using Owin;
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
        }
    }
}
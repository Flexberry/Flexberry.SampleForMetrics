using System.Web.Http;

namespace IIS.SampleForMetrics
{
    public static class WebApiConfig
    {
        public static void RegisterWebApi(this HttpConfiguration httpConfiguration)
        {
            httpConfiguration.MapHttpAttributeRoutes();
        }
    }
}
namespace IIS.SampleForMetrics
{
    using ICSSoft.Services;
    using ICSSoft.STORMNET;
    using IIS.Caseberry.Logging.Objects;
    using NewPlatform.Flexberry;
    using NewPlatform.Flexberry.ORM.ODataService.Extensions;
    using NewPlatform.Flexberry.ORM.ODataService.Files;
    using NewPlatform.Flexberry.ORM.ODataService.Functions;
    using NewPlatform.Flexberry.ORM.ODataService.Model;
    using NewPlatform.Flexberry.ORM.ODataService.WebApi.Extensions;
    using NewPlatform.Flexberry.Services;
    using System;
    using System.Reflection;
    using System.Web.Http;
    using Unity;
    using Unity.AspNet.WebApi;

    /// <summary>
    /// Configure OData Service.
    /// </summary>
    internal static class ODataConfig
    {
        /// <summary>
        /// Configure OData by DataObjects assembly.
        /// </summary>
        /// <param name="config">Http configuration object.</param>
        /// <param name="container">Unity container.</param>
        public static void Configure(HttpConfiguration config, IUnityContainer container, HttpServer httpServer)
        {
            if (config == null)
            {
                throw new ArgumentNullException("config");
            }

            if (container == null)
            {
                throw new ArgumentNullException("container");
            }

            // To support CORS uncomment the line below.
            // config.EnableCors(new DynamicCorsPolicyProvider());
            // Use constructor with true first parameter for enable SupportsCredentials.

            // Use Unity as WebAPI dependency resolver
            config.DependencyResolver = new UnityDependencyResolver(container);

            // Config file upload.
            const string fileControllerPath = "api/File";
            config.MapODataServiceFileRoute("File", fileControllerPath);
            var fileAccessor = new DefaultDataObjectFileAccessor(new Uri("http://localhost"), fileControllerPath, "Uploads");
            container.RegisterInstance<IDataObjectFileAccessor>(fileAccessor);
            
            // Create EDM model builder
            var assemblies = new[]
            {  
                Assembly.Load("SampleForMetrics.Objects"),
                typeof(ApplicationLog).Assembly,
                typeof(UserSetting).Assembly,
                typeof(FlexberryUserSetting).Assembly,
                typeof(Lock).Assembly
            };
            var builder = new DefaultDataObjectEdmModelBuilder(assemblies);

            // Map OData Service
            var token = config.MapDataObjectRoute(builder, httpServer);

            // User functions
            token.Functions.Register(new Func<QueryParameters, string>(Test));

            // Event handlers
            token.Events.CallbackAfterCreate = CallbackAfterCreate;
        }

        private static void CallbackAfterCreate(DataObject dataObject)
        {
            // TODO: implement handler
        }

        private static string Test(QueryParameters queryParameters)
        {
            throw new NotImplementedException();
        }
    }
}
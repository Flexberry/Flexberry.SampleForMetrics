// Copyright (c) Allan Hardy. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace App.Metrics
{
    using Extensions.Owin.DependencyInjection.Internal;
    using Extensions.Owin.DependencyInjection.Options;
    using Extensions.Owin.Middleware;
    using Formatters;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Owin;
    using System;
    using System.Threading.Tasks;
    using System.Web.Hosting;

    public static class OwinMetricsAppBuilderExtensions
    {
        /// <summary>
        ///     Adds App Metrics Middleware to the <see cref="T:Microsoft.AspNetCore.Builder.IApplicationBuilder" /> request execution pipeline.
        /// </summary>
        /// <param name="app">The <see cref="T:Microsoft.AspNetCore.Builder.IApplicationBuilder" />.</param>
        /// <param name="provider">The provider.</param>
        /// <returns>
        ///     A reference to this instance after the operation has completed.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        ///     <see cref="T:Microsoft.AspNetCore.Builder.IApplicationBuilder" /> cannot be null.
        /// </exception>
        public static IAppBuilder UseMetrics(this IAppBuilder app, IServiceProvider provider)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            provider.ThrowIfMetricsNotRegistered();
            var appMetricsOptions = provider.GetRequiredService<MetricsOptions>();
            var owinMetricsOptions = provider.GetRequiredService<OwinMetricsOptions>();
            var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
            var metrics = provider.GetRequiredService<IMetrics>();


            if (appMetricsOptions.Enabled)
            {
                //app.Use(new ErrorRequestMeterMiddleware(owinMetricsOptions, loggerFactory, metrics));
                //app.Use(new PingEndpointMiddleware(owinMetricsOptions, loggerFactory, metrics));
                //app.Use(new PerRequestTimerMiddleware(owinMetricsOptions, loggerFactory, metrics));
                //app.Use(new PostAndPutRequestSizeHistogramMiddleware(owinMetricsOptions, loggerFactory, metrics));
                //app.Use(new RequestTimerMiddleware(owinMetricsOptions, loggerFactory, metrics));
                //app.Use(new ApdexMiddleware(owinMetricsOptions, loggerFactory, metrics));
                app.Use<ActiveRequestCounterEndpointMiddleware>();
            }

            if (owinMetricsOptions.MetricsEndpointEnabled && appMetricsOptions.Enabled)
            {
                var formatter = provider.GetRequiredService<IMetricsOutputFormatter>();
               // app.Use(new MetricsEndpointMiddleware(owinMetricsOptions, loggerFactory, metrics, formatter));
            }

            return app;
        }

        /// <summary>
        /// Runs the configured App Metrics Reporting options once the application has started.
        /// </summary>
        /// <param name="builder">The <see cref="T:Microsoft.AspNetCore.Builder.IApplicationBuilder" />.</param>
        /// <param name="provider">The <see cref="T:System.IServiceProvider" />.</param>
        /// <returns>
        ///     A reference to this instance after the operation has completed.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        ///     <see cref="T:Microsoft.AspNetCore.Builder.IApplicationBuilder" /> &amp;
        ///     <see cref="T:System.IServiceProvider" /> cannot be null
        /// </exception>
        public static IAppBuilder UseMetricsReporting(this IAppBuilder builder, IServiceProvider provider)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (provider == null)
            {
                throw new ArgumentNullException(nameof(provider));
            }

            HostingEnvironment.QueueBackgroundWorkItem(async cancellationToken =>
            {
                var metricsRoot = provider.GetRequiredService<IMetricsRoot>();
                await Task.WhenAll(metricsRoot.ReportRunner.RunAllAsync(cancellationToken));
            });

            return builder;
        }
    }
}
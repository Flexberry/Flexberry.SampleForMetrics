// Copyright (c) Allan hardy. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using App.Metrics.Extensions.Owin.DependencyInjection.Options;
using System;

// ReSharper disable CheckNamespace

namespace Microsoft.Extensions.DependencyInjection
// ReSharper restore CheckNamespace
{
    public static class OwinMetricsCoreBuilderExtensions
    {

        public static IServiceCollection AddMetricsMiddleware(this IServiceCollection builder)
        {
            builder.AddRequiredAspNetPlatformServices();
            return builder;
        }

        public static IServiceCollection AddMetricsMiddleware(this IServiceCollection builder, Action<OwinMetricsOptions> configuration)
        {
            builder.Configure<OwinMetricsOptions>(configuration);
            return builder.AddMetricsMiddleware();
        }

        public static IServiceCollection AddMetricsMiddleware(this IServiceCollection builder, Action<OwinMetricsOptions> configuration,
            Action<OwinMetricsOptions> setupAction)
        {
            builder.Configure<OwinMetricsOptions>(configuration);
            builder.Configure(setupAction);
            return builder.AddMetricsMiddleware();
        }
    }
}
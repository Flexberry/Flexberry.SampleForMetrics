// Copyright (c) Allan hardy. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace Microsoft.Extensions.DependencyInjection
{
    using App.Metrics;
    using App.Metrics.Extensions.Owin.DependencyInjection.Internal;
    using App.Metrics.Extensions.Owin.DependencyInjection.Options;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.Extensions.Options;
    using System;

    public static class MetricsHostBuilderExtensionsOwin
    {
        public static IServiceCollection AddRequiredAspNetPlatformServices(this IServiceCollection builder)
        {
            builder.TryAddSingleton<OwinMetricsMarkerService, OwinMetricsMarkerService>();
            builder.AddSingleton(resolver => resolver.GetRequiredService<IOptions<OwinMetricsOptions>>().Value);
            return builder;
        }

        public static IServiceCollection AddMetrics(this IServiceCollection builder)
        {
            builder.AddMetrics(c => { });
            return builder;
        }

        public static IServiceCollection AddMetrics(this IServiceCollection builder, Action<MetricsBuilder> onConfig)
        {
            builder.AddSingleton(new OwinMetricsOptions());
            var metricsBuilder = new MetricsBuilder();
            onConfig(metricsBuilder);
            var metrics = metricsBuilder.Build();
            builder.AddSingleton(metrics.DefaultOutputMetricsFormatter);
            builder.AddSingleton<IMetrics>(metrics);
            builder.AddSingleton<IMetricsRoot>(metrics);
            return builder;
        }
    }
}
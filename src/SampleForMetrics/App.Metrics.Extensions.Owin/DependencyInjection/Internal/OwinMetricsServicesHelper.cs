// Copyright (c) Allan hardy. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace App.Metrics.Extensions.Owin.DependencyInjection.Internal
{
    using System;

    internal static class OwinMetricsServicesHelper
    {
        /// <summary>
        /// Throws InvalidOperationException when MetricsMarkerService is not present in the list of services.
        /// </summary>
        /// <param name="services">The list of services.</param>
        public static void ThrowIfMetricsNotRegistered(this IServiceProvider services)
        {
            if (services.GetService(typeof(OwinMetricsMarkerService)) == null)
            {
                throw new InvalidOperationException("IServiceCollection.OwinMetrics()\n");
            }
        }
    }
}
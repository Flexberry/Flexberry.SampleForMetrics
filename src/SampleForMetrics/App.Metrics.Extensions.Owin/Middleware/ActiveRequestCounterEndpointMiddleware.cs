// Copyright (c) Allan hardy. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace App.Metrics.Extensions.Owin.Middleware
{
    using DependencyInjection.Options;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class ActiveRequestCounterEndpointMiddleware : AppMetricsMiddleware<OwinMetricsOptions>
    {
        public ActiveRequestCounterEndpointMiddleware(OwinMetricsOptions owinOptions, IMetrics metrics) : base(owinOptions, metrics)
        {
            if (owinOptions == null)
            {
                throw new ArgumentNullException(nameof(owinOptions));
            }
        }

        public async Task Invoke(IDictionary<string, object> environment)
        {
            if (PerformMetric(environment))
            {
                MiddlewareExecuting();
                Metrics.IncrementActiveRequests();
                await Next(environment);
                Metrics.DecrementActiveRequests();
                MiddlewareExecuted();
            }
            else
            {
                await Next(environment);
            }
        }
    }
}
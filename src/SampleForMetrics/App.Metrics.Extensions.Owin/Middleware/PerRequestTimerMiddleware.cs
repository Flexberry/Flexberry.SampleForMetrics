// Copyright (c) Allan hardy. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace App.Metrics.Extensions.Owin.Middleware
{
    using DependencyInjection.Options;
    using Microsoft.AspNetCore.Http;
    using System.Collections.Generic;
    using System.Net;
    using System.Threading.Tasks;

    public class PerRequestTimerMiddleware : AppMetricsMiddleware<OwinMetricsOptions>
    {
        private const string TimerItemsKey = "__App.Metrics.PerRequestStartTime__";

        public PerRequestTimerMiddleware(OwinMetricsOptions owinOptions, IMetrics metrics) : base(owinOptions, metrics)
        {
        }

        public async Task Invoke(IDictionary<string, object> environment)
        {
            if (PerformMetric(environment))
            {
                MiddlewareExecuting();

                var httpResponseStatusCode = int.Parse(environment["owin.ResponseStatusCode"].ToString());

                environment[TimerItemsKey] = Metrics.Clock.Nanoseconds;

                await Next(environment);

                if (environment.HasMetricsCurrentRouteName() && httpResponseStatusCode != (int)HttpStatusCode.NotFound)
                {
                    var clientId = environment.OAuthClientId();

                    var startTime = (long)environment[TimerItemsKey];
                    var elapsed = Metrics.Clock.Nanoseconds - startTime;

                    Metrics.RecordEndpointRequestTime(clientId, environment.GetMetricsCurrentRouteName(), elapsed);
                }

                MiddlewareExecuted();
            }
            else
            {
                await Next(environment);
            }
        }
    }
}
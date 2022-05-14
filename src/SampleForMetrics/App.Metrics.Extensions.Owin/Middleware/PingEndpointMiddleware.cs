// Copyright (c) Allan hardy. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace App.Metrics.Extensions.Owin.Middleware
{
    using DependencyInjection.Options;
    using Extensions;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class PingEndpointMiddleware : AppMetricsMiddleware<OwinMetricsOptions>
    {
        public PingEndpointMiddleware(OwinMetricsOptions owinOptions, IMetrics metrics) : base(owinOptions, metrics)
        {

        }

        public async Task Invoke(IDictionary<string, object> environment)
        {
            var requestPath = environment["owin.RequestPath"] as string;

            if (Options.PingEndpointEnabled && Options.PingEndpoint.IsPresent() && Options.PingEndpoint == requestPath)
            {
                MiddlewareExecuting();

                await WriteResponseAsync(environment, "pong", "text/plain");

                MiddlewareExecuted();

                return;
            }

            await Next(environment);
        }
    }
}
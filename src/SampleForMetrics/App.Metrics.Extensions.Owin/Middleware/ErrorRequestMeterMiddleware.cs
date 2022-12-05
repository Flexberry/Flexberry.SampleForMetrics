// Copyright (c) Allan hardy. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace App.Metrics.Extensions.Owin.Middleware
{
    using DependencyInjection.Options;
    using Microsoft.AspNetCore.Http;
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Threading.Tasks;

    /// <summary>
    ///     Measures the overall error request rate as well as the rate per endpoint.
    ///     Also measures these error rates per OAuth2 Client as a separate metric
    /// </summary>
    public class ErrorRequestMeterMiddleware : AppMetricsMiddleware<OwinMetricsOptions>
    {
        public ErrorRequestMeterMiddleware(OwinMetricsOptions owinOptions, IMetrics metrics) : base(owinOptions, metrics)
        {
            if (owinOptions == null)
            {
                throw new ArgumentNullException(nameof(owinOptions));
            }

            if (metrics == null)
            {
                throw new ArgumentNullException(nameof(metrics));
            }
        }

        public async Task Invoke(IDictionary<string, object> environment)
        {
            await Next(environment);

            if (PerformMetric(environment))
            {
                MiddlewareExecuting();

                var routeTemplate = environment.GetMetricsCurrentRouteName();

                var httpResponseStatusCode = int.Parse(environment["owin.ResponseStatusCode"].ToString());

                if (!(httpResponseStatusCode >= (int)HttpStatusCode.OK && httpResponseStatusCode <= 299))
                {
                    Metrics.MarkHttpRequestEndpointError(routeTemplate, httpResponseStatusCode);
                    Metrics.MarkHttpRequestError(httpResponseStatusCode);
                    Metrics.ErrorRequestPercentage();
                }
            }

            MiddlewareExecuted();
        }
    }
}
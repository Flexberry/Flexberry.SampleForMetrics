// Copyright (c) Allan hardy. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace App.Metrics.Extensions.Owin.Middleware
{
    using DependencyInjection.Options;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class PostAndPutRequestSizeHistogramMiddleware : AppMetricsMiddleware<OwinMetricsOptions>
    {
        public PostAndPutRequestSizeHistogramMiddleware(OwinMetricsOptions owinOptions, IMetrics metrics) : base(owinOptions, metrics)
        {
        }

        public async Task Invoke(IDictionary<string, object> environment)
        {
            if (PerformMetric(environment))
            {
                MiddlewareExecuting();

                var httpMethod = environment["owin.RequestMethod"].ToString().ToUpper();

                if (httpMethod == "POST" || httpMethod == "PUT")
                {
                    var headers = (IDictionary<string, string[]>)environment["owin.RequestHeaders"];
                    if (headers != null && headers.ContainsKey("Content-Length"))
                    {
                        Metrics.UpdatePostAndPutRequestSize(long.Parse(headers["Content-Length"].First()));
                    }
                }

                MiddlewareExecuted();
            }

            await Next(environment);
        }
    }
}
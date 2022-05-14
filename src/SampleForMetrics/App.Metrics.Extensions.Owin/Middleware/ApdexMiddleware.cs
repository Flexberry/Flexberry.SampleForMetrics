// Copyright (c) Allan Hardy. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace App.Metrics.Extensions.Owin.Middleware
{
    using Apdex;
    using DependencyInjection.Options;
    using Internal;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;


    public class ApdexMiddleware : AppMetricsMiddleware<OwinMetricsOptions>
    {
        private const string ApdexItemsKey = "__App.Metrics.Apdex__";
        private readonly IApdex _apdexTracking;

        public ApdexMiddleware(OwinMetricsOptions owinOptions, IMetrics metrics) : base(owinOptions, metrics)
        {
            _apdexTracking = Metrics.Provider.Apdex.Instance(OwinMetricsRegistry.Contexts.HttpRequests.ApdexScores.Apdex(owinOptions.ApdexTSeconds));
        }

        public async Task Invoke(IDictionary<string, object> environment)
        {
            if (PerformMetric(environment) && Options.ApdexTrackingEnabled)
            {
                MiddlewareExecuting();

                environment[ApdexItemsKey] = _apdexTracking.NewContext();

                await Next(environment);

                var apdex = environment[ApdexItemsKey];
                using (apdex as IDisposable)
                {
                }
                environment.Remove(ApdexItemsKey);

                MiddlewareExecuted();
            }
            else
            {
                await Next(environment);
            }
        }
    }
}
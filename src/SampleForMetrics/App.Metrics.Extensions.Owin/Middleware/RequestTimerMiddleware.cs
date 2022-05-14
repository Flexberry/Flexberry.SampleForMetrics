// Copyright (c) Allan hardy. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace App.Metrics.Extensions.Owin.Middleware
{
    using DependencyInjection.Options;
    using Internal;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Timer;

    public class RequestTimerMiddleware : AppMetricsMiddleware<OwinMetricsOptions>
    {
        private const string TimerItemsKey = "__App.Metrics.RequestTimer__";
        private readonly ITimer _requestTimer;

        public RequestTimerMiddleware(OwinMetricsOptions owinOptions, IMetrics metrics) : base(owinOptions, metrics)
        {
            _requestTimer = Metrics.Provider.Timer.Instance(OwinMetricsRegistry.Contexts.HttpRequests.Timers.WebRequestTimer);
        }

        public async Task Invoke(IDictionary<string, object> environment)
        {
            if (PerformMetric(environment))
            {
                MiddlewareExecuting();

                environment[TimerItemsKey] = _requestTimer.NewContext();

                await Next(environment);

                var timer = environment[TimerItemsKey];
                using (timer as IDisposable)
                {
                }
                environment.Remove(TimerItemsKey);

                MiddlewareExecuted();
            }
            else
            {
                await Next(environment);
            }
        }
    }
}
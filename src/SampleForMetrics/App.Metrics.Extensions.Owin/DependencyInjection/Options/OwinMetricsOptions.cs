// Copyright (c) Allan hardy. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.Collections.Generic;
using App.Metrics.Extensions.Owin.Extensions;
using App.Metrics.Extensions.Owin.Internal;

namespace App.Metrics.Extensions.Owin.DependencyInjection.Options
{
    public class OwinMetricsOptions
    {
        public OwinMetricsOptions()
        {
            MetricsEndpointEnabled = true;
            PingEndpointEnabled = true;
            ApdexTrackingEnabled = true;
            ApdexTSeconds = Constants.ReservoirSampling.DefaultApdexTSeconds;
        }

        public bool ApdexTrackingEnabled { get; set; }

        public double ApdexTSeconds { get; set; }

        public IList<string> IgnoredRoutesRegexPatterns { get; set; } = new List<string>();

        public string MetricsEndpoint { get; set; } = Constants.DefaultRoutePaths.MetricsEndpoint.EnsureLeadingSlash();

        public bool MetricsEndpointEnabled { get; set; }

        public string PingEndpoint { get; set; } = Constants.DefaultRoutePaths.PingEndpoint.EnsureLeadingSlash();

        public bool PingEndpointEnabled { get; set; }
    }
}
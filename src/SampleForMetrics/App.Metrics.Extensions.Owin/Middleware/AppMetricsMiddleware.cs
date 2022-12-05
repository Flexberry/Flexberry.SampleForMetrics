﻿// Copyright (c) Allan hardy. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace App.Metrics.Extensions.Owin.Middleware
{
    using DependencyInjection.Options;
    using Extensions;
    using Logging;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Text.RegularExpressions;
    using AppFunc = Func<IDictionary<string, object>, Task>;

    public abstract class AppMetricsMiddleware<TOptions> where TOptions : OwinMetricsOptions, new()
    {
        private readonly Func<string, bool> _shouldRecordMetric;

        private string _middlewareType;

        protected AppMetricsMiddleware(TOptions owinOptions, IMetrics metrics)
        {
            if (owinOptions == null)
            {
                throw new ArgumentNullException(nameof(owinOptions));
            }

            if (metrics == null)
            {
                throw new ArgumentNullException(nameof(metrics));
            }

            Options = owinOptions;

            Metrics = metrics;

            IReadOnlyList<Regex> ignoredRoutes = Options.IgnoredRoutesRegexPatterns
                .Select(p => new Regex(p, RegexOptions.Compiled | RegexOptions.IgnoreCase))
                .ToList();

            if (ignoredRoutes.Any())
            {
                _shouldRecordMetric = path => !ignoredRoutes.Any(ignorePattern => ignorePattern.IsMatch(path.ToString().RemoveLeadingSlash()));
            }
            else
            {
                _shouldRecordMetric = path => true;
            }

            _middlewareType = GetType().Name;
            Logger = LogProvider.GetLogger(_middlewareType);
        }

        protected void MiddlewareExecuted()
        {
            Logger.Debug($"Executed Owin Metrics Middleare {_middlewareType}");
        }

        protected void MiddlewareExecuting()
        {
            Logger.Debug($"Executing Owin Metrics Middleare {_middlewareType}");
        }

        public ILog Logger { get; set; }

        public IMetrics Metrics { get; set; }

        public AppFunc Next { get; set; }

        public TOptions Options { get; set; }

        public void Initialize(AppFunc next)
        {
            Next = next;
        }

        protected bool PerformMetric(IDictionary<string, object> environment)
        {
            var requestPath = environment["owin.RequestPath"] as string;

            if (Options.IgnoredRoutesRegexPatterns == null)
            {
                return true;
            }

            if (string.IsNullOrWhiteSpace(requestPath))
            {
                return false;
            }

            return _shouldRecordMetric(requestPath);
        }

        protected Task WriteResponseAsync(IDictionary<string, object> environment, string content, string contentType,
            HttpStatusCode code = HttpStatusCode.OK, string warning = null)
        {
            var response = environment["owin.ResponseBody"] as Stream;
            var headers = environment["owin.ResponseHeaders"] as IDictionary<string, string[]>;
            var contentBytes = Encoding.UTF8.GetBytes(content);

            headers["Content-Type"] = new[] { contentType };
            headers["Cache-Control"] = new[] { "no-cache, no-store, must-revalidate" };
            headers["Pragma"] = new[] { "no-cache" };
            headers["Expires"] = new[] { "0" };

            if (warning.IsPresent())
            {
                headers["Warning"] = new[] { $"Warning: 100 '{warning}'" };
            }

            environment["owin.ResponseStatusCode"] = (int)code;
            return response.WriteAsync(contentBytes, 0, contentBytes.Length);
        }
    }
}
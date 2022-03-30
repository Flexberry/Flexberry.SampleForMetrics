﻿// <copyright file="DefaultTimerMetric.cs" company="Allan Hardy">
// Copyright (c) Allan Hardy. All rights reserved.
// </copyright>

using System;
using App.Metrics.Abstractions.MetricTypes;
using App.Metrics.Abstractions.ReservoirSampling;
using App.Metrics.Concurrency;
using App.Metrics.Histogram;
using App.Metrics.Meter;

namespace App.Metrics.Timer
{
    public sealed class DefaultTimerMetric : ITimerMetric, IDisposable
    {
        private readonly StripedLongAdder _activeSessionsCounter = new StripedLongAdder();
        private readonly IClock _clock;
        private readonly IHistogramMetric _histogram;
        private readonly IMeterMetric _meter;
        private bool _disposed;

        /// <summary>
        ///     Initializes a new instance of the <see cref="DefaultTimerMetric" /> class.
        /// </summary>
        /// <param name="histogram">The histogram implementation to use.</param>
        /// <param name="clock">The clock to use to measure processing duration.</param>
        public DefaultTimerMetric(IHistogramMetric histogram, IClock clock)
        {
            _clock = clock;
            _histogram = histogram;
            _meter = new DefaultMeterMetric(clock);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="DefaultTimerMetric" /> class.
        /// </summary>
        /// <param name="reservoir">The reservoir implementation to use for sampling values to generate the histogram.</param>
        /// <param name="clock">The clock to use to measure processing duration.</param>
        public DefaultTimerMetric(Lazy<IReservoir> reservoir, IClock clock)
        {
            _clock = clock;
            _histogram = new DefaultHistogramMetric(reservoir);
            _meter = new DefaultMeterMetric(clock);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="DefaultTimerMetric" /> class.
        /// </summary>
        /// <param name="reservoir">The reservoir to use for sampling within the histogram.</param>
        /// <param name="meter">The meter implementation to use to genreate the rate of events over time.</param>
        /// <param name="clock">The clock to use to measure processing duration.</param>
        public DefaultTimerMetric(Lazy<IReservoir> reservoir, IMeterMetric meter, IClock clock)
            : this(new DefaultHistogramMetric(reservoir), meter, clock)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="DefaultTimerMetric" /> class.
        /// </summary>
        /// <param name="histogram">The histogram implementation to use.</param>
        /// <param name="meter">The meter implementation to use to genreate the rate of events over time.</param>
        /// <param name="clock">The clock to use to measure processing duration.</param>
        public DefaultTimerMetric(IHistogramMetric histogram, IMeterMetric meter, IClock clock)
        {
            _clock = clock;
            _meter = meter;
            _histogram = histogram;
        }

        /// <inheritdoc />
        public TimerValue Value => GetValue();

        /// <inheritdoc />
        public long CurrentTime()
        {
            return _clock.Nanoseconds;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
        }

        // ReSharper disable MemberCanBePrivate.Global
        public void Dispose(bool disposing)
            // ReSharper restore MemberCanBePrivate.Global
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Free any other managed objects here.
                    _histogram?.Dispose();
                    _meter?.Dispose();
                }
            }

            _disposed = true;
        }

        /// <inheritdoc />
        public long EndRecording()
        {
            _activeSessionsCounter.Decrement();
            return _clock.Nanoseconds;
        }

        /// <inheritdoc />
        public TimerValue GetValue(bool resetMetric = false)
        {
            return new TimerValue(
                _meter.GetValue(resetMetric),
                _histogram.GetValue(resetMetric),
                _activeSessionsCounter.GetValue(),
                TimeUnit.Nanoseconds);
        }

        /// <inheritdoc />
        public TimerContext NewContext(string userValue)
        {
            return new TimerContext(this, userValue);
        }

        /// <inheritdoc />
        public TimerContext NewContext()
        {
            return NewContext(null);
        }

        /// <inheritdoc />
        public void Record(long duration, TimeUnit unit, string userValue)
        {
            var nanos = unit.ToNanoseconds(duration);
            if (nanos < 0)
            {
                return;
            }

            _histogram.Update(nanos, userValue);
            _meter.Mark();
        }

        /// <inheritdoc />
        public void Record(long time, TimeUnit unit)
        {
            Record(time, unit, null);
        }

        /// <inheritdoc />
        public void Reset()
        {
            _meter.Reset();
            _histogram.Reset();
        }

        /// <inheritdoc />
        public long StartRecording()
        {
            _activeSessionsCounter.Increment();
            return _clock.Nanoseconds;
        }

        /// <inheritdoc />
        public void Time(Action action, string userValue)
        {
            var start = _clock.Nanoseconds;
            try
            {
                _activeSessionsCounter.Increment();
                action();
            }
            finally
            {
                _activeSessionsCounter.Decrement();
                Record(_clock.Nanoseconds - start, TimeUnit.Nanoseconds, userValue);
            }
        }

        /// <inheritdoc />
        public T Time<T>(Func<T> action, string userValue)
        {
            var start = _clock.Nanoseconds;
            try
            {
                _activeSessionsCounter.Increment();
                return action();
            }
            finally
            {
                _activeSessionsCounter.Decrement();
                Record(_clock.Nanoseconds - start, TimeUnit.Nanoseconds, userValue);
            }
        }

        /// <inheritdoc />
        public void Time(Action action)
        {
            Time(action, null);
        }

        /// <inheritdoc />
        public T Time<T>(Func<T> action)
        {
            return Time(action, null);
        }
    }
}
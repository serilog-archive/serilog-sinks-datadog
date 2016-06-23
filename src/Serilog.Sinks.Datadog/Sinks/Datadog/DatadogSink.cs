// Copyright 2014 Serilog Contributors
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Sinks.PeriodicBatching;
using StatsdClient;

namespace Serilog.Sinks.Datadog
{
    internal class DatadogSink : PeriodicBatchingSink
    {
        private readonly DatadogConfiguration _datadogConfiguration;
        private readonly ITextFormatter _textFormatter;
        private readonly StatsdUDP _statsdUdp;
        private readonly Statsd _statsd;

        /// <summary>
        /// A reasonable default for the number of events posted in
        /// each batch.
        /// </summary>
        public const int DefaultBatchPostingLimit = 100;

        /// <summary>
        /// A reasonable default time to wait between checking for event batches.
        /// </summary>
        public static readonly TimeSpan DefaultPeriod = TimeSpan.FromSeconds(30);

        /// <summary>
        /// Construct a sink that uses datadog with the specified details.
        /// </summary>
        /// <param name="datadogConfiguration">Connection information used to construct the Datadog client.</param>
        /// <param name="batchSizeLimit">The maximum number of events to post in a single batch.</param>
        /// <param name="period">The time to wait between checking for event batches.</param>
        /// <param name="textFormatter">Supplies culture-specific formatting information, or null.</param>
        public DatadogSink(DatadogConfiguration datadogConfiguration, int batchSizeLimit, TimeSpan period, ITextFormatter textFormatter)
            : base(batchSizeLimit, period)
        {
            if (datadogConfiguration == null) throw new ArgumentNullException("datadogConfiguration");

            _datadogConfiguration = datadogConfiguration;
            _textFormatter = textFormatter;
            _statsdUdp = new StatsdUDP(datadogConfiguration.StatsdServer, datadogConfiguration.StatsdPort);
            _statsd = new Statsd(_statsdUdp);
        }

        /// <summary>
        /// Free resources held by the sink.
        /// </summary>
        /// <param name="disposing">If true, called because the object is being disposed; if false,
        /// the object is being disposed from the finalizer.</param>
        protected override void Dispose(bool disposing)
        {
            // First flush the buffer
            base.Dispose(disposing);

            if (disposing)
            {
                _statsdUdp.Dispose();
            }
        }

        /// <summary>
        /// Emit a batch of log events to datadog.
        /// </summary>
        /// <param name="events">The events to emit.</param>
        /// <remarks>Override either <see cref="PeriodicBatchingSink.EmitBatch"/> or <see cref="PeriodicBatchingSink.EmitBatchAsync"/>,
        /// not both.</remarks>
        protected override void EmitBatch(IEnumerable<LogEvent> events)
        {
            if (events == null)
                throw new ArgumentNullException("events");

            foreach (var logEvent in events)
            {
                var payload = new StringWriter();
                var title = "Log Event - " + logEvent.Level;
                var alertType = GetAlertTypeFromEvent(logEvent);
                var tags = GetTagsFromEvent(logEvent,_datadogConfiguration.Tags);

                _textFormatter.Format(logEvent, payload);
                
                _statsd.Add(title, payload.ToString(), alertType, hostname: _datadogConfiguration.Hostname, tags: tags);
            }

            _statsd.Send();
        }

        private static string GetAlertTypeFromEvent(LogEvent logEvent)
        {
            switch (logEvent.Level)
            {
                case LogEventLevel.Error:
                case LogEventLevel.Fatal:
                    return "error";
                case LogEventLevel.Warning:
                    return "warning";
                default:
                    return "info";
            }
        }

        private static string[] GetTagsFromEvent(LogEvent logEvent, string[] tags)
        {
            // For each Property Value Add a Tag

            var tagList = tags.ToList();

            tagList.Add($"LogLevel:{logEvent.Level}");
            tagList.Add($"LogMessageTemplate:{logEvent.MessageTemplate}");
            tagList.Add($"LogTimeStamp:{logEvent.Timestamp}");

            if (logEvent.Exception != null)
            {
                tagList.Add($"Exception:{logEvent.Exception}");
            }
         
            tagList.AddRange(logEvent.Properties.Where(property => property.Value != null).Select(property => $"{property.Key}:{property.Value.ToString()}"));

            return tagList.ToArray();

        }
    }
}

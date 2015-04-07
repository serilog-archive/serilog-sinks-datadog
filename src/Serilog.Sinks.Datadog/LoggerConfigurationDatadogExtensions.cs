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
using Serilog.Configuration;
using Serilog.Events;
using Serilog.Formatting.Display;
using Serilog.Sinks.Datadog;

namespace Serilog
{
    /// <summary>
    /// Adds the WriteTo.Datadog() extension method to <see cref="LoggerConfiguration"/>.
    /// </summary>
    public static class LoggerConfigurationDatadogExtensions
    {
        private const string DefaultOutputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] {Message}{NewLine}{Exception}";

        /// <summary>
        /// Adds a sink that sends log events via datadog.
        /// </summary>
        /// <param name="loggerConfiguration">The logger configuration.</param>
        /// <param name="datadogConfiguration">The configuration used for writing events to datadog.</param>
        /// <param name="outputTemplate">A message template describing the format used to write to the sink.
        /// the default is "{Timestamp} [{Level}] {Message}{NewLine}{Exception}".</param>
        /// <param name="restrictedToMinimumLevel">The minimum log event level required in order to write an event to the sink.</param>
        /// <param name="batchPostingLimit">The maximum number of events to post in a single batch.</param>
        /// <param name="period">The time to wait between checking for event batches.</param>
        /// <param name="formatProvider">Supplies culture-specific formatting information, or null.</param>
        /// <returns>Logger configuration, allowing datadogConfiguration to continue.</returns>
        /// <exception cref="ArgumentNullException">A required parameter is null.</exception>
        public static LoggerConfiguration Datadog(
            this LoggerSinkConfiguration loggerConfiguration,
            DatadogConfiguration datadogConfiguration,
            string outputTemplate = DefaultOutputTemplate,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
            int batchPostingLimit = DatadogSink.DefaultBatchPostingLimit,
            TimeSpan? period = null,
            IFormatProvider formatProvider = null)
        {
            if (datadogConfiguration == null) throw new ArgumentNullException("datadogConfiguration");

            var defaultedPeriod = period ?? DatadogSink.DefaultPeriod;
            var formatter = new MessageTemplateTextFormatter(outputTemplate, formatProvider);
            var datadogSink = new DatadogSink(datadogConfiguration, batchPostingLimit, defaultedPeriod, formatter);

            return loggerConfiguration.Sink(datadogSink, restrictedToMinimumLevel);
        }
    }
}

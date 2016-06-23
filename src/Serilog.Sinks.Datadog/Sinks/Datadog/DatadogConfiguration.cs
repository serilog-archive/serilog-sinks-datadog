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

namespace Serilog.Sinks.Datadog
{
    /// <summary>
    /// Connection information for use by the Datadog sink.
    /// </summary>
    public class DatadogConfiguration
    {
        private const string DefaultStatsdServer = "127.0.0.1";
        private const int DefaultStatsdPort = 8125;

        /// <summary>
        /// The Statsd server to use, if not provided the default is 127.0.0.1.
        /// </summary>
        public string StatsdServer { get; private set; }

        /// <summary>
        /// The Statsd port to use, if not provided the default is 8125.
        /// </summary>
        public int StatsdPort { get; private set; }

        /// <summary>
        /// The hostname to assign to written events.
        /// </summary>
        public string Hostname { get; private set; }

        /// <summary>
        /// The tags to assign to written events.
        /// </summary>
        public string[] Tags { get; private set; }

        /// <summary>
        /// Prepended prefix written to events.
        /// </summary>
        public string Prefix { get; set; }

        /// <summary>
        /// Constructs the <see cref="DatadogConfiguration"/> with the default configuration.
        /// </summary>
        public DatadogConfiguration()
        {
            StatsdServer = DefaultStatsdServer;
            StatsdPort = DefaultStatsdPort;
        }

        /// <summary>
        /// Constructs the <see cref="DatadogConfiguration"/> with the provided configuration.
        /// </summary>
        /// <param name="statsdServer">The statsd server to use, if not provided the default 127.0.0.1 is used.</param>
        /// <param name="statsdPort">The port of the statsd server, if not provided the default 8125 is used.</param>
        /// <param name="hostname">The hostname to assign to written events.</param>
        /// <param name="tags">The tags to assign to written events.</param>
        public DatadogConfiguration(string statsdServer, int? statsdPort, string hostname, string[] tags)
        {
            StatsdServer = statsdServer ?? DefaultStatsdServer;
            StatsdPort = statsdPort ?? DefaultStatsdPort;
            Hostname = hostname;
            Tags = tags;
        }

        /// <summary>
        /// Configure the statsd server to use for writing events.
        /// </summary>
        /// <param name="statsdServer">The statsd server to use.</param>
        /// <param name="statsdPort">The port of the statsd server.</param>
        /// <returns>A copy of the configuration with the statsd server details set.</returns>
        public DatadogConfiguration WithStatsdServer(string statsdServer, int? statsdPort)
        {
            return new DatadogConfiguration(statsdServer, statsdPort, Hostname, Tags);
        }

        /// <summary>
        /// Configure the hostname assigned to written events.
        /// </summary>
        /// <param name="hostname">The hostname to assign to written events.</param>
        /// <returns>A copy of the configuration with the hostname set.</returns>
        public DatadogConfiguration WithHostname(string hostname)
        {
            return new DatadogConfiguration(StatsdServer, StatsdPort, hostname, Tags);
        }

        /// <summary>
        /// Configure the tags assigned to events written events.
        /// </summary>
        /// <param name="tags">The tags to assign to written events.</param>
        /// <returns>A copy of the configuration with the tags set.</returns>
        public DatadogConfiguration WithTags(params string[] tags)
        {
            return new DatadogConfiguration(StatsdServer, StatsdPort, Hostname, tags);
        }
    }
}

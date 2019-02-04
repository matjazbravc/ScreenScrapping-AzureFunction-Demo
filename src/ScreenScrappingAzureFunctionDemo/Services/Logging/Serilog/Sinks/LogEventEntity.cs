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
using System.IO;
using Microsoft.WindowsAzure.Storage.Table;
using ScreenScrappingAzureFunctionDemo.Services.Logging.Serilog.Services;
using Serilog.Events;
using Serilog.Formatting.Json;

namespace ScreenScrappingAzureFunctionDemo.Services.Logging.Serilog.Sinks
{
    // todo: Figure out a better name than LogEventEntity given the table name is the same and it is weird...
    /// <summary>
    ///     Represents a single log event for the Serilog Azure Table Storage Sink.
    /// </summary>
    public class LogEventEntity : TableEntity
    {
        /// <summary>
        ///     Default constructor for the Storage Client library to re-hydrate entities when querying.
        /// </summary>
        public LogEventEntity() { }

        /// <summary>
        ///     Create a log event entity from a Serilog <see cref="LogEvent" />.
        /// </summary>
        /// <param name="log">The event to log</param>
        /// <param name="formatProvider">Supplies culture-specific formatting information, or null.</param>
        /// <param name="partitionKey">partition key to store</param>
        /// <param name="rowKey">row key to store</param>
        public LogEventEntity(LogEvent log, IFormatProvider formatProvider, string partitionKey, string rowKey)
        {
            Timestamp = log.Timestamp.ToUniversalTime().DateTime;
            PartitionKey = partitionKey;
            log.Properties.TryGetValue("SourceContext", out LogEventPropertyValue value);
            var rk = (value?.ToString() ?? GetValidRowKey(rowKey)).Replace("\"", string.Empty).Trim();
            RowKey = rk;
            Level = log.Level.ToString();
            Exception = log.Exception?.ToString();
            Message = log.RenderMessage(formatProvider);
            var s = new StringWriter();
            new JsonFormatter("", formatProvider: formatProvider).Format(log, s);
            Data = s.ToString();
        }

        /// <summary>
        ///     The level of the log.
        /// </summary>
        public string Level { get; set; }

        /// <summary>
        ///     A string representation of the exception that was attached to the log (if any).
        /// </summary>
        public string Exception { get; set; }

        /// <summary>
        ///     The rendered log message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        ///     A JSON-serialised representation of the data attached to the log message.
        /// </summary>
        public string Data { get; set; }

        // http://msdn.microsoft.com/en-us/library/windowsazure/dd179338.aspx
        private static string GetValidRowKey(string rowKey)
        {
            rowKey = ObjectNaming.KeyFieldValueCharactersNotAllowedMatch.Replace(rowKey, "");
            return rowKey.Length > 1024 ? rowKey.Substring(0, 1024) : rowKey;
        }
    }
}
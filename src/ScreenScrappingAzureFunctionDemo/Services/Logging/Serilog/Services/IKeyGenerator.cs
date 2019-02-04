﻿using Serilog.Events;

namespace ScreenScrappingAzureFunctionDemo.Services.Logging.Serilog.Services
{
    /// <summary>
    ///     Interface used to generate row keys for <see cref="LogEvent" />s
    /// </summary>
    public interface IKeyGenerator
    {
        /// <summary>
        ///     Generate the patition key based on the supplied <paramref name="logEvent" />
        /// </summary>
        /// <param name="logEvent">the log event</param>
        /// <returns></returns>
        string GeneratePartitionKey(LogEvent logEvent);

        /// <summary>
        ///     Generate a row key for the supplied <paramref name="logEvent" />.
        /// </summary>
        /// <param name="logEvent">the log event</param>
        /// <param name="suffix">suffix to the RowKey</param>
        /// <returns>the row key that should be stored in the Azure table</returns>
        string GenerateRowKey(LogEvent logEvent, string suffix = null);
    }
}
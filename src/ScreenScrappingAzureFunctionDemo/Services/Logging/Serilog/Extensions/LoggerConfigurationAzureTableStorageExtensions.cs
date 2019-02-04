using System;
using Microsoft.WindowsAzure.Storage;
using ScreenScrappingAzureFunctionDemo.Services.Logging.Serilog.Services;
using ScreenScrappingAzureFunctionDemo.Services.Logging.Serilog.Sinks;
using Serilog;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Debugging;
using Serilog.Events;

namespace ScreenScrappingAzureFunctionDemo.Services.Logging.Serilog.Extensions
{
    /// <summary>
    ///     Adds the WriteTo.AzureTableStorage() extension method to <see cref="LoggerConfiguration" />.
    /// </summary>
    public static class LoggerConfigurationAzureTableStorageExtensions
    {
        /// <summary>
        ///     A reasonable default for the number of events posted in
        ///     each batch.
        /// </summary>
        public const int DEFAULT_BATCH_POSTING_LIMIT = 50;

        /// <summary>
        ///     A reasonable default time to wait between checking for event batches.
        /// </summary>
        public static readonly TimeSpan DefaultPeriod = TimeSpan.FromSeconds(2);

        /// <summary>
        ///     Adds a sink that writes log events as records in the 'LogEventEntity' Azure Table Storage table in the given
        ///     storage account.
        /// </summary>
        /// <param name="loggerConfiguration">The logger configuration.</param>
        /// <param name="storageAccount">The Cloud Storage Account to use to insert the log entries to.</param>
        /// <param name="restrictedToMinimumLevel">The minimum log event level required in order to write an event to the sink.</param>
        /// <param name="formatProvider">Supplies culture-specific formatting information, or null.</param>
        /// <param name="storageTableName">
        ///     Table name that log entries will be written to. Note: Optional, setting this may impact
        ///     performance
        /// </param>
        /// <param name="writeInBatches">
        ///     Use a periodic batching sink, as opposed to a synchronous one-at-a-time sink; this alters the partition
        ///     key used for the events so is not enabled by default.
        /// </param>
        /// <param name="batchPostingLimit">The maximum number of events to post in a single batch.</param>
        /// <param name="period">The time to wait between checking for event batches.</param>
        /// <param name="keyGenerator">The key generator used to create the PartitionKey and the RowKey for each log entry</param>
        /// <returns>Logger configuration, allowing configuration to continue.</returns>
        /// <exception cref="ArgumentNullException">A required parameter is null.</exception>
        public static LoggerConfiguration AzureTableStorage(this LoggerSinkConfiguration loggerConfiguration,
            CloudStorageAccount storageAccount,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
            IFormatProvider formatProvider = null,
            string storageTableName = null,
            bool writeInBatches = false,
            TimeSpan? period = null,
            int? batchPostingLimit = null,
            IKeyGenerator keyGenerator = null)
        {
            if (loggerConfiguration == null)
            {
                throw new ArgumentNullException(nameof(loggerConfiguration));
            }
            if (storageAccount == null)
            {
                throw new ArgumentNullException(nameof(storageAccount));
            }

            ILogEventSink sink;

            try
            {
                sink = writeInBatches ? (ILogEventSink)new AzureBatchingTableStorageSink(storageAccount, formatProvider, batchPostingLimit ?? DEFAULT_BATCH_POSTING_LIMIT, period ?? DefaultPeriod, storageTableName, keyGenerator) : new AzureTableStorageSink(storageAccount, formatProvider, storageTableName, keyGenerator);
            }
            catch (Exception ex)
            {
                SelfLog.WriteLine("Error configuring AzureTableStorage: {0}", ex);
                sink = new LoggerConfiguration().CreateLogger();
            }

            return loggerConfiguration.Sink(sink, restrictedToMinimumLevel);
        }

        /// <summary>
        ///     Adds a sink that writes log events as records in the 'LogEventEntity' Azure Table Storage table in the given
        ///     storage account.
        /// </summary>
        /// <param name="loggerConfiguration">The logger configuration.</param>
        /// <param name="connectionString">The Cloud Storage Account connection string to use to insert the log entries to.</param>
        /// <param name="restrictedToMinimumLevel">The minimum log event level required in order to write an event to the sink.</param>
        /// <param name="formatProvider">Supplies culture-specific formatting information, or null.</param>
        /// <param name="storageTableName">
        ///     Table name that log entries will be written to. Note: Optional, setting this may impact
        ///     performance
        /// </param>
        /// <param name="writeInBatches">
        ///     Use a periodic batching sink, as opposed to a synchronous one-at-a-time sink; this alters the partition
        ///     key used for the events so is not enabled by default.
        /// </param>
        /// <param name="batchPostingLimit">The maximum number of events to post in a single batch.</param>
        /// <param name="period">The time to wait between checking for event batches.</param>
        /// <param name="keyGenerator">The key generator used to create the PartitionKey and the RowKey for each log entry</param>
        /// <returns>Logger configuration, allowing configuration to continue.</returns>
        /// <exception cref="ArgumentNullException">A required parameter is null.</exception>
        public static LoggerConfiguration AzureTableStorage(this LoggerSinkConfiguration loggerConfiguration,
            string connectionString,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
            IFormatProvider formatProvider = null,
            string storageTableName = null,
            bool writeInBatches = false,
            TimeSpan? period = null,
            int? batchPostingLimit = null,
            IKeyGenerator keyGenerator = null)
        {
            if (loggerConfiguration == null)
            {
                throw new ArgumentNullException(nameof(loggerConfiguration));
            }
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException(nameof(connectionString));
            }

            try
            {
                var storageAccount = CloudStorageAccount.Parse(connectionString);
                return AzureTableStorage(loggerConfiguration, storageAccount, restrictedToMinimumLevel, formatProvider, storageTableName, writeInBatches, period, batchPostingLimit, keyGenerator);
            }
            catch (Exception ex)
            {
                SelfLog.WriteLine("Error configuring AzureTableStorage: {0}", ex);

                ILogEventSink sink = new LoggerConfiguration().CreateLogger();
                return loggerConfiguration.Sink(sink, restrictedToMinimumLevel);
            }
        }
    }
}
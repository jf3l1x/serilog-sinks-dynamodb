using System;

namespace Serilog.Sinks.DynamoDB
{
    public class DynamoDBSinkOptions
    {
        public DynamoDBSinkOptions()
        {
            Period = TimeSpan.FromSeconds(5);
            BatchPostingLimit = 1000;
        }
        /// <summary>
        ///     The DynamoDB Table to persist events
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        ///     The maximum number of events to post in a single batch.
        /// </summary>
        public int BatchPostingLimit { get; set; }

        /// <summary>
        ///     The time to wait between checking for event batches. Defaults to 2 seconds.
        /// </summary>
        public TimeSpan Period { get; set; }

        /// <summary>
        ///     Optional Region to configure, if not informed will use the configuration from appsettings
        /// </summary>
        public string Region { get; set; }

        /// <summary>
        ///     Optional AccessKey to configure, if not informed will use the configuration from appsettings
        /// </summary>
        public string AccessKey { get; set; }

        /// <summary>
        ///     Optional SecretKey to configure, if not informed will use the configuration from appsettings
        /// </summary>
        public string SecretKey { get; set; }
    }
}
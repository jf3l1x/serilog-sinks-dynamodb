using System;
using System.Linq;
using Amazon.DynamoDBv2.DataModel;
using Serilog.Events;

namespace Serilog.Sinks.DynamoDB
{
    /// <summary>
    ///     A Dynamo DB document to repersent a log event
    /// </summary>
    public class LogDocument
    {
        /// <summary>
        ///     Construct a new <see cref="LogDocument" />.
        /// </summary>
        public LogDocument()
        {
        }

        /// <summary>
        ///     Construct a new <see cref="LogDocument" />.
        /// </summary>
        public LogDocument(LogEvent logEvent, string renderedMessage)
        {
            Id = Guid.NewGuid();
            Timestamp = logEvent.Timestamp.UtcDateTime;
            MessageTemplate = logEvent.MessageTemplate.Text;
            Level = logEvent.Level.ToString();
            RenderedMessage = renderedMessage;
            
            
       
        }

        [DynamoDBHashKey]
        public Guid Id { get; set; }

        /// <summary>
        ///     The time at which the event occurred.
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        ///     The template that was used for the log message.
        /// </summary>
        public string MessageTemplate { get; set; }

        /// <summary>
        ///     The level of the log.
        /// </summary>
        public string Level { get; set; }

        /// <summary>
        ///     The rendered log message.
        /// </summary>
        public string RenderedMessage { get; set; }

        /// <summary>
        ///     Properties associated with the event, including those presented in <see cref="Events.LogEvent.MessageTemplate" />.
        /// </summary>
        /// public IDictionary
        /// <string, object> Properties { get; set; }
        public string Properties { get; set; }
    }
}
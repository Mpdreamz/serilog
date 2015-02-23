using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Serilog.Events;

namespace Serilog.Sinks.Elasticsearch.Tests.Domain
{
    /// <summary>
    /// Elasticsearch _bulk follows a specific pattern:
    /// {operation}\n
    /// {operationmetadata}\n
    /// This provides a marker interface for both
    /// </summary>
    interface IBulkObject { }

    class BulkOperation : IBulkObject
    {
        [JsonProperty("index")]
        public IndexAction IndexAction { get; set; }
    }
    class IndexAction
    {
        [JsonProperty("_index")]
        public string Index { get; set; }
        [JsonProperty("_type")]
        public string Type { get; set; }
    }

    public class SerilogElasticsearchMessage : IBulkObject
    {
        [JsonProperty("@timestamp")]
        public DateTime Timestamp { get; set; }
        
        [JsonProperty("level")]
        [JsonConverter(typeof(StringEnumConverter))]
        public LogEventLevel Level { get; set; }
        
        [JsonProperty("messageTemplate")]
        public string MessageTemplate { get; set; }
        
        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("exceptions")]
        public List<SerilogElasticsearchException> Exceptions { get; set; }
    }

    public class SerilogElasticsearchException
    {
        [JsonProperty("Message")]
        public string Message { get; set; }
    }
}

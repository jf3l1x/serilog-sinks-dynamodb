// Copyright 2015 Serilog Contributors
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
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Runtime;
using Serilog.Debugging;
using Serilog.Events;
using Serilog.Sinks.PeriodicBatching;

namespace Serilog.Sinks.DynamoDB
{
    public class DynamoDBSink : PeriodicBatchingSink
    {
        private readonly IFormatProvider _formatProvider;
        private readonly DynamoDBSinkOptions _options;
        private readonly string _tableName;
        private readonly DefaultJsonFormatter _formatter;
        private RegionEndpoint _regionEndpoint;
        private AWSCredentials _credentials;
        public DynamoDBSink(IFormatProvider formatProvider, DynamoDBSinkOptions options)
            : base(options.BatchPostingLimit, options.Period)
        {
            _formatter=new DynamoDBJsonFormatter();
            _formatProvider = formatProvider;
            _options = options;
            _tableName = options.TableName;
            
            if (!string.IsNullOrEmpty(_options.AccessKey))
            {
                _credentials=new BasicAWSCredentials(_options.AccessKey,_options.SecretKey);
            }
            if (!string.IsNullOrEmpty(_options.Region))
            {
                if (_credentials == null)
                {
                    AWSConfigs.AWSRegion = _options.Region;
                }
                else
                {
                    _regionEndpoint = RegionEndpoint.GetBySystemName(_options.Region);
                }
                
            }
            AmazonDynamoDbConfig = new AmazonDynamoDBConfig() ;
            
            OperationConfig = new DynamoDBOperationConfig {OverrideTableName = options.TableName};
        }

        private DynamoDBOperationConfig OperationConfig { get; }

        private AmazonDynamoDBConfig AmazonDynamoDbConfig { get; }

        protected override async Task EmitBatchAsync(IEnumerable<LogEvent> events)
        {
            var records = events.Select(x =>
            {
                var document = new LogDocument(x, x.RenderMessage(_formatProvider));
                var sw = new StringWriter();
                _formatter.Format(x, sw);
                document.Properties = sw.ToString();
                return document;
            });
            AmazonDynamoDBClient client = null;
            try
            {
                
                if (_credentials != null)
                {
                    if (_regionEndpoint != null)
                    {
                        client = new AmazonDynamoDBClient(_credentials, _regionEndpoint);
                    }
                    else
                    {
                        client = new AmazonDynamoDBClient(_credentials, AmazonDynamoDbConfig);
                    }

                }
                else
                {
                    client = new AmazonDynamoDBClient( AmazonDynamoDbConfig);
                }
                using (var context = new DynamoDBContext(client))
                {
                    var batchWrite = context.CreateBatchWrite<LogDocument>(OperationConfig);
                    batchWrite.AddPutItems(records);
                    await batchWrite.ExecuteAsync();
                }

            }
            catch (Exception exception)
            {
                SelfLog.WriteLine("Unable to write events to DynamoDB Sink for {0}: {1}", _tableName, exception);
            }
            finally
            {
                if (client != null)
                {
                    client.Dispose();
                }
            }
        }
    }
}
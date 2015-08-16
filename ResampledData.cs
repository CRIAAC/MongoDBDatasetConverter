using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using FileHelpers;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using MongoDB.Bson.Serialization.Attributes;

namespace MongoDBDatasetConverter
{
    public class ResampledData
    {
        public ResampledData(BsonDocument bsonDocument)
        {
            Timestamp = bsonDocument["timestamp"].ToUniversalTime();
            _sensorsDatas = new List<SensorData>();
            foreach (var key in bsonDocument["content"].ToBsonDocument())
            {
                var bsonObject = key.Value.ToBsonDocument();
                var localSensorData = BsonSerializer.Deserialize<SensorData>(bsonObject);
                _sensorsDatas.Add(localSensorData);
            }
        }
        
        public DateTime Timestamp { get; set; }
        private readonly List<SensorData> _sensorsDatas;

        public override string ToString()
        {
            return Timestamp.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture) + "," + string.Join(",", _sensorsDatas.Select(x => x.ToString()).ToArray());
        }
    }
}
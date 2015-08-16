using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace MongoDBDatasetConverter
{
    public class RawData
    {
        
        public RawData(BsonDocument bsonDocument)
        {
            TimestampSent = bsonDocument["timestamp_sent"].ToUniversalTime();
            bsonDocument.Remove("timestamp_sent");
            TimestampReceived = bsonDocument["timestamp_received"].ToUniversalTime();
            bsonDocument.Remove("timestamp_received");
            _sensorsDatas = BsonSerializer.Deserialize<SensorData>(bsonDocument);
        }

        [BsonElement("timestamp_sent")]
        public DateTime TimestampSent { get; set; }
        [BsonElement("timestamp_received")]
        public DateTime TimestampReceived { get; set; }
        public SensorData _sensorsDatas { get; set; }



        public override string ToString()
        {
            return TimestampSent.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture) +  "," + TimestampReceived.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture) + "," + _sensorsDatas.ToString();
        }
    }
}
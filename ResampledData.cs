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
            SensorsDatas = new List<SensorData>();
            foreach (var key in bsonDocument["content"].ToBsonDocument())
            {
                var bsonObject = key.Value.ToBsonDocument();
                var localSensorData = BsonSerializer.Deserialize<SensorData>(bsonObject);
                SensorsDatas.Add(localSensorData);
            }
        }
        
        public DateTime Timestamp { get; set; }
        public List<SensorData> SensorsDatas { get; }

        public List<double?> getSensorData()
        {
            var properties = from property in typeof(SensorData).GetProperties()
                             let orderAttribute = property.GetCustomAttributes(typeof(OrderAttribute), false).SingleOrDefault() as OrderAttribute
                             orderby orderAttribute.Order
                             select property;            

            List<double?> result = new List<double?>();
            foreach (var sensorsData in SensorsDatas)
            {
                foreach (var property in properties)
                {
                    result.Add(property.GetValue(sensorsData,null) as double?);
                }
            }

            return result;
        }

        public void SetSensorDatas(List<double?> input)
        {
            var properties = from property in typeof(SensorData).GetProperties()
                             let orderAttribute = property.GetCustomAttributes(typeof(OrderAttribute), false).SingleOrDefault() as OrderAttribute
                             orderby orderAttribute.Order
                             select property;

            int index = 0;
            foreach (var sensorsData in SensorsDatas)
            {
                foreach (var property in properties)
                {
                    property.SetValue(sensorsData,input[index++]);
                }
            }
        }

        public override string ToString()
        {
            return Timestamp.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture) + "," + string.Join(",", SensorsDatas.Select(x => x.ToString()).ToArray());
        }

        public string ToStringIgnoreTime()
        {
            return string.Join(",", SensorsDatas.Select(x => x.ToString()).ToArray());
        }
    }
}
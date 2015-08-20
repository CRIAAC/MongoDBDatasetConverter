using System.Reflection;
using System.Text;
using MongoDB.Bson.Serialization.Attributes;

namespace MongoDBDatasetConverter
{
    public class SensorData
    {
        private PropertyInfo[] _PropertyInfos = null;

        [Order]
        [BsonElement("accel_X")]
        public double? AccelX { get; set; }

        [Order]
        [BsonElement("accel_Y")]
        public double? AccelY { get; set; }

        [Order]
        [BsonElement("accel_Z")]
        public double? AccelZ { get; set; }

        [Order]
        [BsonElement("gyro_X")]
        public double? GyroX { get; set; }

        [Order]
        [BsonElement("gyro_Y")]
        public double? GyroY { get; set; }

        [Order]
        [BsonElement("gyro_Z")]
        public double? GyroZ { get; set; }

        [Order]
        [BsonElement("magneto_X")]
        public double? MagnetoX { get; set; }

        [Order]
        [BsonElement("magneto_Y")]
        public double? MagnetoY { get; set; }

        [Order]
        [BsonElement("magneto_Z")]
        public double? MagnetoZ { get; set; }

        [Order]
        [BsonElement("pitch")]
        public double? Pitch { get; set; }

        [Order]
        [BsonElement("roll")]
        public double? Roll { get; set; }

        [Order]
        [BsonElement("yaw")]
        public double? Yaw { get; set; }

        public string ToString()
        {
            if (_PropertyInfos == null)
                _PropertyInfos = this.GetType().GetProperties();

            var sb = new StringBuilder();

            foreach (var info in _PropertyInfos)
            {
                var value = info.GetValue(this, null) ?? "null";
                //sb.AppendLine(info.Name + ": " + value.ToString());
                sb.Append(value.ToString() + ",");
            }

            sb.Length--;

            return sb.ToString();
        }
    }
}
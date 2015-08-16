using System.Reflection;
using System.Text;
using MongoDB.Bson.Serialization.Attributes;

namespace MongoDBDatasetConverter
{
    public class SensorData
    {
        private PropertyInfo[] _PropertyInfos = null;
        
        [BsonElement("accel_X")]
        public double? AccelX { get; set; }

        [BsonElement("accel_Y")]
        public double? AccelY { get; set; }

        [BsonElement("accel_Z")]
        public double? AccelZ { get; set; }

        [BsonElement("gyro_X")]
        public double? GyroX { get; set; }

        [BsonElement("gyro_Y")]
        public double? GyroY { get; set; }

        [BsonElement("gyro_Z")]
        public double? GyroZ { get; set; }

        [BsonElement("magneto_X")]
        public double? MagnetoX { get; set; }

        [BsonElement("magneto_Y")]
        public double? MagnetoY { get; set; }

        [BsonElement("magneto_Z")]
        public double? MagnetoZ { get; set; }

        [BsonElement("pitch")]
        public double? Pitch { get; set; }

        [BsonElement("roll")]
        public double? Roll { get; set; }

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
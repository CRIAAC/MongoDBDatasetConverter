using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Schema;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace MongoDBDatasetConverter
{
    class RawDataContainer : DataContainer
    {
        private readonly Dictionary<string, Dictionary<string, List<RawData>>> _collectionsContent = new Dictionary<string, Dictionary<string, List<RawData>>>();
        public RawDataContainer(string connectionString, string databaseName) : base(connectionString, databaseName)
        {
            CheckDatabaseName(databaseName);
        }

        public RawDataContainer(string databaseName) : base(databaseName)
        {
            CheckDatabaseName(databaseName);
        }

        private void CheckDatabaseName(string databaseName)
        {
            if (!Regex.IsMatch(databaseName, @"^[a-zA-Z0-9_]*$"))
            {
                throw new NotImplementedException("Invalid String Format");
            }
        }

        public override async Task ConvertToCsv(string collectionName, string outputFileName)
        {
            if (!Regex.IsMatch(collectionName, @"^[a-zA-Z0-9_]*$"))
            {
                throw new NotImplementedException("Invalid String Format");
            }

            _collectionsContent.Add(collectionName, new Dictionary<string, List<RawData>>());
            Console.WriteLine(collectionName);

            var collection = Database.GetCollection<BsonDocument>(collectionName);
            var aggregate = collection.Aggregate().Group(new BsonDocument { { "_id", "$mac" }, { "count", new BsonDocument("$sum", 1) } });
            var results = await aggregate.ToListAsync();

            int counter = 0;

            foreach (var result in results)
            {
                var localMacAddress = result["_id"].ToString();

                _collectionsContent[collectionName].Add(localMacAddress, new List<RawData>());

                var filter = Builders<BsonDocument>.Filter.Eq("mac", localMacAddress);
                var projection = Builders<BsonDocument>.Projection.Exclude("_id").Exclude("mac");              
                var documents = await collection.Find(filter).Project(projection).ToListAsync();

                using (StreamWriter file = new StreamWriter(outputFileName +"_" +  counter + ".csv"))
                {
                    foreach (var document in documents)
                    {
                        _collectionsContent[collectionName][localMacAddress].Add(new RawData(document));
                        file.WriteLine(_collectionsContent[collectionName][localMacAddress].Last().ToString());
                    }
                }

                counter++;
            }           
        }

        
        public override async Task ConvertAllCollectionsToCsv(string dir)
        {
            using (var cursor = await Database.ListCollectionsAsync())
            {
                var collections = await cursor.ToListAsync();

                var query = from e in collections.AsQueryable()
                    where e["name"].ToString().Contains("_raw")
                    select e;

                foreach (var collection in query)
                {                    
                    var collectionName = collection["name"].ToString();
                    
                    var localCsvFileName = dir + collectionName;
                    await ConvertToCsv(collectionName, localCsvFileName);
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FileHelpers;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace MongoDBDatasetConverter
{
    class ResampledDataContainer : DataContainer
    {
        private readonly Dictionary<string, List<ResampledData>> _collectionsContent = new Dictionary<string, List<ResampledData>>();

        public ResampledDataContainer(string connectionString, string databaseName) : base(connectionString, databaseName)
        {
            CheckDatabaseName(databaseName);
        }

        public ResampledDataContainer(string databaseName) : base(databaseName)
        {
            CheckDatabaseName(databaseName);
        }

        private void CheckDatabaseName(string databaseName)
        {
            if (!Regex.IsMatch(databaseName, @"^[a-zA-Z0-9]*$"))
            {
                throw new NotImplementedException("Invalid String Format");
            }
        }

        public ResampledData[] Content { get; set; }

        public override async Task ConvertToCsv(string collectionName, string outputFileName)
        {

            if (!Regex.IsMatch(collectionName, @"^[a-zA-Z0-9]*$"))
            {
                throw new NotImplementedException("Invalid String Format");
            }


            _collectionsContent.Add(collectionName, new List<ResampledData>());
            Console.WriteLine(collectionName);

            var collection = Database.GetCollection<BsonDocument>(collectionName);

            var projection = Builders<BsonDocument>.Projection.Exclude("_id");
            var documents = await collection.Find(new BsonDocument()).Project(projection).ToListAsync();

            using (StreamWriter file = new StreamWriter(outputFileName))
            {
                List<List<double?>> matrix = new List<List<double?>>();


                foreach (var document in documents)
                {
                    ResampledData localContent = new ResampledData(document);                    
                    string s = localContent.ToStringIgnoreTime();

                    //avoid null enties at begining
                    if (s.Contains("null") && _collectionsContent[collectionName].Count == 0)
                    {
                        Console.WriteLine("line null at begining");
                    }
                    else
                    {
                        matrix.Add(localContent.getSensorData());
                        _collectionsContent[collectionName].Add(localContent);
                    }
                }

                int nbRows = matrix.Count;
                int nbColumns = matrix[0].Count;

                for (int i = 0; i < nbColumns; i++)
                {
                    List<double?> temp = new List<double?>();
                    for (int j = 0; j < nbRows; j++)
                    {
                        temp.Add(matrix[j][i]);
                    }

                    temp.Smooth();
                    for (int j = 0; j < nbRows; j++)
                    {
                        matrix[j][i] = temp[j];
                    }
                }

                for (int index = 0; index < matrix.Count; index++)
                {
                    var row = matrix[index];
                    _collectionsContent[collectionName][index].SetSensorDatas(row);
                    file.WriteLine(_collectionsContent[collectionName][index].ToStringIgnoreTime());
                }

                Console.WriteLine();
            }
        }

        public override async Task ConvertAllCollectionsToCsv(string dir)
        {
            using (var cursor = await Database.ListCollectionsAsync())
            {
                var collections = await cursor.ToListAsync();

                var query = from e in collections.AsQueryable()
                    where e["name"].ToString().Contains("_raw") == false
                    select e;
                
                foreach (var collection in query)
                {
                    var collectionName = collection["name"].ToString();
                    var localCsvFileName =  dir + collectionName + ".csv";
                    await ConvertToCsv(collectionName, localCsvFileName);
                }
            }
        }

    }
}

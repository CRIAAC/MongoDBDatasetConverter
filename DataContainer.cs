using System.Threading.Tasks;
using MongoDB.Driver;

namespace MongoDBDatasetConverter
{
    internal abstract class DataContainer
    {
        protected DataContainer(string connectionString, string databaseName)
        {
            Client = new MongoClient(connectionString);
            Database = Client.GetDatabase(databaseName);
        }

        protected DataContainer(string databaseName) : this("mongodb://localhost:27017", databaseName)
        {
        }

        protected static IMongoClient Client { get; set; }
        protected static IMongoDatabase Database { get; set; }


        public abstract Task ConvertAllCollectionsToCsv(string dir);
        public abstract Task ConvertToCsv(string collectionName, string outputFileName);
    }
}
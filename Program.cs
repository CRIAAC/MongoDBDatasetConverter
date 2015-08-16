using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MongoDBDatasetConverter
{
    class Program
    {
        static void Main(string[] args)
        {
            ResampledDataContainer test = new ResampledDataContainer("datasets");
            RawDataContainer test2 = new RawDataContainer("datasets");


            //var task = test.ConvertToCsv("testnewrouteur", @"../../test.csv");
            var task = test.ConvertAllCollectionsToCsv(@"../../csv_files/");

            //var task = test2.ConvertToCsv("testdemerde1_raw", @"../../test");
            //var task = test2.ConvertAllCollectionsToCsv(@"../../csv_rawfiles/");

            task.Wait();
        }
    }
}

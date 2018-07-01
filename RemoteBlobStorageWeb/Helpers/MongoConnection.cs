using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using RBS.Models;
using RBS.Utils;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core;


namespace RBS.Helpers
{
    public class MongoConnection
    {
        public static string uri = Config.MongoUrl;
        private static readonly Lazy<MongoClient> lazyMongoClient =
            new Lazy<MongoClient>(() => new MongoClient(uri));

        public static MongoClient mongoClient => lazyMongoClient.Value;
        
        public static IMongoCollection<RBS_Blob> GetReadingsCollection() {
            
            var db = mongoClient.GetDatabase("bjorninge_remote_blob");
            return db.GetCollection<RBS_Blob>("blob1");
        }

       
        public async static Task AsyncInsertReading(RBS_Blob reading){

            await GetReadingsCollection().InsertOneAsync(reading);
        }

        public async static Task<RBS_Blob> GetRemoteReading(string uuid)
        {
 

            var collection = GetReadingsCollection();

            var filter = Builders<RBS_Blob>.Filter.Eq("uuid", uuid);


            return await collection.FindAsync(filter).Result.FirstAsync();
        }

        public async static Task<List<RBS_Blob>> GetPendingReadingsForProcessing()
        {

            var collection = GetReadingsCollection();

            var filter = Builders<RBS_Blob>.Filter.Eq("purpose", "RBS");

            var pending = await collection.FindAsync(filter).Result.ToListAsync();

            // Removes pending entries.
            // Next call to GetPendingReadingsForProcessing will not
            // include these entries
            //var update = Builders<LibreReadingModel>.Update.Set("status", "processing");
            //await collection.UpdateManyAsync(filter, update);

            return pending;
        }



      
        /*public async static Task<bool> AsyncUpdateReading(RBS_Blob reading)
        {
           

            var collection = GetReadingsCollection();

            var updateFilter = Builders<RBS_Blob>.Filter.Eq("uuid", reading.uuid);
            var update = Builders<RBS_Blob>.Update.set



            var res = await collection.UpdateOneAsync(updateFilter, update);

            return res.ModifiedCount == 1;


        }*/



    }



}

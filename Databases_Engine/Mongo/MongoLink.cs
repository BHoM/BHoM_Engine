using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BHoM.Global;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Databases_Engine.Mongo
{
    public class MongoLink
    {
        private IMongoCollection<BsonDocument> collection;

        public MongoLink(string serverLink = "mongodb://localhost:27017", string databaseName = "project", string collectionName = "bhomObjects")
        {
            var mongo = new MongoClient();
            IMongoDatabase database = mongo.GetDatabase(databaseName);
            collection = database.GetCollection<BsonDocument>(collectionName);
        }

        public void SaveObject(BHoMObject obj)
        {
            var document = BsonDocument.Parse(obj.ToJSON());
            collection.InsertOne(document);
        }

        public void SaveObjects(IEnumerable<BHoMObject> objects)
        {
            var documents = objects.Select(x => BsonDocument.Parse(x.ToJSON()));
            collection.InsertMany(documents);
        }

        public void DeleteObjects(string filterString)
        {
            FilterDefinition<BsonDocument> filter = filterString;
            collection.DeleteMany(filter);
        }

        public IEnumerable<BHoMObject> GetObjects(string filterString)
        {
            Project tempProject = new Project();

            FilterDefinition<BsonDocument> filter = filterString;
            var result = collection.Find(filter);
            var ret = result.ToList().Select(x => BHoMObject.FromJSON(x.ToString(), tempProject));
            Console.WriteLine(collection.Count(new BsonDocument()));
            return ret;
        }
    }
}

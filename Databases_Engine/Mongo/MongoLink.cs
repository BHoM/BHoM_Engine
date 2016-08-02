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
        private IMongoCollection<BsonDocument> depCollection;

        public MongoLink(string serverLink = "mongodb://localhost:27017", string databaseName = "project", string collectionName = "bhomObjects")
        {
            var mongo = new MongoClient(serverLink);
            IMongoDatabase database = mongo.GetDatabase(databaseName);
            collection = database.GetCollection<BsonDocument>(collectionName);
            depCollection = database.GetCollection<BsonDocument>(collectionName + "__dep");
        }

        public void SaveObjects(IEnumerable<BHoMObject> objects, string key = "")
        {
            // Create the bulk query for the object to replace/insert
            List<WriteModel<BsonDocument>> bulk = new List<WriteModel<BsonDocument>>();
            bulk.Add(new DeleteManyModel<BsonDocument>(Builders<BsonDocument>.Filter.Eq("Key", key)));
            foreach (BHoMObject obj in objects)
                bulk.Add(new InsertOneModel<BsonDocument>(ToBson(obj, key)));

            // Send that query
            BulkWriteOptions bulkOptions = new BulkWriteOptions();
            bulkOptions.IsOrdered = true;
            collection.BulkWrite(bulk, bulkOptions);

            // Get all the dependencies
            Dictionary<Guid, BHoMObject> dependencies = new Dictionary<Guid, BHoMObject>();
            foreach (BHoMObject bhomObject in objects)
            {
                foreach (KeyValuePair<Guid, BHoMObject> kvp in bhomObject.GetDeepDependencies())
                {
                    if (!dependencies.ContainsKey(kvp.Key))
                        dependencies[kvp.Key] = kvp.Value;
                }
            }

            // Create the bulk query for the dependencies to replace/insert
            List<WriteModel<BsonDocument>> depBulk = new List<WriteModel<BsonDocument>>();
            depBulk.Add(new DeleteManyModel<BsonDocument>(Builders<BsonDocument>.Filter.Eq("Key", key)));
            foreach (BHoMObject obj in dependencies.Values)
                depBulk.Add(new InsertOneModel<BsonDocument>(ToBson(obj, key)));

            // Send that query
            depCollection.BulkWrite(depBulk, bulkOptions);
        }

        public void DeleteObjects(string filterString)
        {
            FilterDefinition<BsonDocument> filter = filterString;
            collection.DeleteMany(filter);
        }

        public void Clear()
        {
            collection.DeleteMany(new BsonDocument());
        }

        public IEnumerable<BHoMObject> GetObjects(string filterString)
        {
            // Add the queried objects to a temp project
            Project tempProject = new Project();
            FilterDefinition<BsonDocument> filter = filterString;
            var result = collection.Find(filter);
            var ret = result.ToList().Select(x => BHoMObject.FromJSON(x.ToString(), tempProject));
            foreach (BHoMObject obj in ret)
                tempProject.AddObject(obj);

            // Sort out the dependencies
            var deps = depCollection.Find<BsonDocument>(Builders<BsonDocument>.Filter.In("Properties.BHoM_Guid", tempProject.GetTaskValues()));
            foreach( BsonDocument doc in deps.ToList())
                tempProject.AddObject(BHoMObject.FromJSON(doc.ToString(), tempProject));
            tempProject.RunTasks();

            return ret;
        }

        private BsonDocument ToBson(BHoMObject obj, string key)
        {
            var document = BsonDocument.Parse(obj.ToJSON());
            if (key != "")
                document["Key"] = key;
            return document;
        }
    }
}

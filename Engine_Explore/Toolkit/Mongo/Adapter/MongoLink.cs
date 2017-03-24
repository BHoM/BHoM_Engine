using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using BHB = Engine_Explore.BHoM.Base;
using BHE = Engine_Explore.Engine;


namespace Engine_Explore.Toolkit.Mongo.Adapter
{
    public class MongoLink : BHB.ILink
    {
        /***************************************************/
        /**** Properties                                ****/
        /***************************************************/

        public string ServerName
        {
            get
            {
                MongoServerAddress server = m_Collection.Database.Client.Settings.Server;
                return "mongodb://" + server.ToString();
            }
        }

        /*******************************************/

        public string DatabaseName
        {
            get { return m_Collection.Database.DatabaseNamespace.DatabaseName; }
        }

        /*******************************************/

        public string CollectionName
        {
            get { return m_Collection.CollectionNamespace.CollectionName; }
        }

        /*******************************************/

        public int HistorySize { get; set; }


        /***************************************************/
        /**** Constructors                              ****/
        /***************************************************/

        public MongoLink(string serverLink = "mongodb://localhost:27017", string databaseName = "project", string collectionName = "bhomObjects")
        {
            if (!serverLink.StartsWith("mongodb://"))
                serverLink = "mongodb://" + serverLink + ":27017";

            m_Client = new MongoClient(serverLink);
            IMongoDatabase database = m_Client.GetDatabase(databaseName);
            m_Collection = database.GetCollection<BsonDocument>(collectionName);

            HistorySize = 20;

            IMongoDatabase hist_Database = m_Client.GetDatabase(databaseName + "_History");
            m_History = hist_Database.GetCollection<BsonDocument>(collectionName);
        }


        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public bool Push(IEnumerable<object> objects, out object result, bool overwrite = true, string key = "")
        {
            // Check that the database is connected
            if (m_Client.Cluster.Description.State == MongoDB.Driver.Core.Clusters.ClusterState.Disconnected)
            {
                result = null;
                return false;
            }
                
            // Create the bulk query for the object to replace/insert
            DateTime timestamp = DateTime.Now;
            List<WriteModel<BsonDocument>> bulk = new List<WriteModel<BsonDocument>>();
            WriteModel<BsonDocument> deletePrevious = new DeleteManyModel<BsonDocument>(Builders<BsonDocument>.Filter.Eq("__Key__", key));
            if (overwrite)
                bulk.Add(deletePrevious);
            foreach (object obj in objects)
                bulk.Add(new InsertOneModel<BsonDocument>(BHE.Convert.Bson.Write(obj, key, timestamp)));

            // Send that query
            BulkWriteOptions bulkOptions = new BulkWriteOptions();
            bulkOptions.IsOrdered = true;
            result = m_Collection.BulkWrite(bulk, bulkOptions);

            // Push in the history database as well
            if (overwrite)
                bulk.Remove(deletePrevious);
            List<object> times = Pull(new List<string> { "{$group: {_id: \"$__Time__\"}}", "{$sort: {_id: -1}}" });
            if (times.Count > HistorySize)
                bulk.Insert(0, new DeleteManyModel<BsonDocument>(Builders<BsonDocument>.Filter.Lte("__Time__", times[HistorySize])));
            m_History.BulkWrite(bulk, bulkOptions);

            return true;
        }

        /*******************************************/

        public bool Push(IEnumerable<object> objects, bool overwrite = true, string key = "")
        {
            object temp;
            return Push(objects, out temp, overwrite, key);
        }

        /*******************************************/

        public List<object> Pull(List<string> queries, string config = "{keepAsString: false}")
        {
            if (m_Client.Cluster.Description.State == MongoDB.Driver.Core.Clusters.ClusterState.Disconnected)
                return new List<object>();

            var pipeline = queries.Select(s => BsonDocument.Parse(s)).ToList();
            var aggregateOptions = new AggregateOptions();
            aggregateOptions.AllowDiskUse = true;
            List<BsonDocument> result = m_Collection.Aggregate<BsonDocument>(pipeline, aggregateOptions).ToList();

            var configDic = BHE.Convert.Json.ReadStringDictionary<string>(config);

            if (configDic != null && configDic.ContainsKey("keepAsString") && configDic["KeepAsString"] != "false")
                return result.Select(x => x.ToString()).ToList<object>();
            else
                return result.Select(x => BHE.Convert.Bson.ReadObject(x)).ToList<object>();
        }

        /*******************************************/

        public bool Delete(string filter = "{}", string config = "")
        {
            if (m_Client.Cluster.Description.State == MongoDB.Driver.Core.Clusters.ClusterState.Disconnected)
                return false;

            FilterDefinition<BsonDocument> filterDef = filter;
            m_Collection.DeleteMany(filterDef);
            return true;
        }

        /*******************************************/

        public bool Execute(string command, string config = "")
        {
            return false;
        }

        /*******************************************/
        /****  Private Fields                   ****/
        /*******************************************/

        private MongoClient m_Client;
        private IMongoCollection<BsonDocument> m_Collection;
        private IMongoCollection<BsonDocument> m_History;
    }
}

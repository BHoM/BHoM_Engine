using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using BHB = Engine_Explore.BHoM.Base;
using BHE = Engine_Explore.Engine;
using System.Collections;

namespace Engine_Explore.Adapter
{
    public class MongoAdapter : BHB.IAdapter
    {
        /***************************************************/
        /**** Properties                                ****/
        /***************************************************/

        public string ServerName { get { return m_Link.ServerName; } }

        public string DatabaseName { get { return m_Link.DatabaseName; } }

        public string CollectionName { get { return m_Link.CollectionName; } }

        public int HistorySize { get { return m_Link.HistorySize; } set { m_Link.HistorySize = value; } }

        public List<string> ErrorLog { get; set; } = new List<string>();


        /***************************************************/
        /**** Constructors                              ****/
        /***************************************************/

        public MongoAdapter(string serverLink = "mongodb://localhost:27017", string databaseName = "project", string collectionName = "bhomObjects")
        {
            m_Link = new Link.MongoLink(serverLink, databaseName, collectionName);
        }


        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public bool Push(IEnumerable<object> objects, out object result, bool overwrite = true, string key = "")
        {
            result = null;
            DateTime timestamp = DateTime.Now;
            IEnumerable<BsonDocument> documents = objects.Select(x => BHE.Convert.Bson.Write(x, key, timestamp));
            return m_Link.Push(documents, overwrite, key);           
        }

        /*******************************************/

        public bool Push(IEnumerable<object> objects, bool overwrite = true, string key = "")
        {
            DateTime timestamp = DateTime.Now;
            IEnumerable<BsonDocument> documents = objects.Select(x => BHE.Convert.Bson.Write(x, key, timestamp));
            return m_Link.Push(documents, overwrite, key);
        }

        /*******************************************/

        public IList Pull(string queries, string config = "{keepAsJson: false}")
        {
            List<BsonDocument> result = m_Link.Pull(BHE.Convert.Json.ReadStringArray(queries));
       
            var configDic = BHE.Convert.Json.ReadStringDictionary<string>(config);

            if (configDic != null && configDic.ContainsKey("keepAsJson") && configDic["KeepAsJson"] != "false")
                return result.Select(x => x.ToString()).ToList();
            else
                return result.Select(x => BHE.Convert.Bson.ReadObject(x)).ToList();
        }

        /*******************************************/

        public bool Delete(string filter = "{}", string config = "")
        {
            return m_Link.Delete(filter);
        }

        /*******************************************/

        public bool Execute(string command, string config = "")
        {
            return false;
        }


        /*******************************************/
        /****  Private Fields                   ****/
        /*******************************************/

        private Link.MongoLink m_Link;
    }
}

/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2018, the respective contributors. All rights reserved.
 *
 * Each contributor holds copyright over their respective contributions.
 * The project versioning (Git) records all such contribution source information.
 *                                           
 *                                                                              
 * The BHoM is free software: you can redistribute it and/or modify         
 * it under the terms of the GNU Lesser General Public License as published by  
 * the Free Software Foundation, either version 3.0 of the License, or          
 * (at your option) any later version.                                          
 *                                                                              
 * The BHoM is distributed in the hope that it will be useful,              
 * but WITHOUT ANY WARRANTY; without even the implied warranty of               
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the                 
 * GNU Lesser General Public License for more details.                          
 *                                                                            
 * You should have received a copy of the GNU Lesser General Public License     
 * along with this code. If not, see <https://www.gnu.org/licenses/lgpl-3.0.html>.      
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;


namespace Engine_Explore.Adapter.Link
{
    public class MongoLink
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

        public bool Push(IEnumerable<BsonDocument> objects, bool overwrite = true, string key = "")
        {
            // Check that the database is connected
            if (m_Client.Cluster.Description.State == MongoDB.Driver.Core.Clusters.ClusterState.Disconnected)
                return false;

            // Create the bulk query for the object to replace/insert
            List<WriteModel<BsonDocument>> bulk = new List<WriteModel<BsonDocument>>();
            WriteModel<BsonDocument> deletePrevious = new DeleteManyModel<BsonDocument>(Builders<BsonDocument>.Filter.Eq("__Key__", key));
            if (overwrite)
                bulk.Add(deletePrevious);
            foreach (BsonDocument obj in objects)
                bulk.Add(new InsertOneModel<BsonDocument>(obj));

            // Send that query
            BulkWriteOptions bulkOptions = new BulkWriteOptions();
            bulkOptions.IsOrdered = true;
            m_Collection.BulkWrite(bulk, bulkOptions);

            // Push in the history database as well
            if (overwrite)
                bulk.Remove(deletePrevious);
            List<BsonDocument> times = Pull(new List<string> { "{$group: {_id: \"$__Time__\"}}", "{$sort: {_id: -1}}" });
            if (times.Count > HistorySize)
                bulk.Insert(0, new DeleteManyModel<BsonDocument>(Builders<BsonDocument>.Filter.Lte("__Time__", times[HistorySize])));
            m_History.BulkWrite(bulk, bulkOptions);

            return true;
        }

        /*******************************************/

        public List<BsonDocument> Pull(List<string> queries)
        {
            if (m_Client.Cluster.Description.State == MongoDB.Driver.Core.Clusters.ClusterState.Disconnected)
                return new List<BsonDocument>();

            var pipeline = queries.Select(s => BsonDocument.Parse(s)).ToList();
            var aggregateOptions = new AggregateOptions();
            aggregateOptions.AllowDiskUse = true;

            return  m_Collection.Aggregate<BsonDocument>(pipeline, aggregateOptions).ToList();
        }

        /*******************************************/

        public bool Delete(string filter = "{}")
        {
            if (m_Client.Cluster.Description.State == MongoDB.Driver.Core.Clusters.ClusterState.Disconnected)
                return false;

            m_Collection.DeleteMany(filter);
            return true;
        }


        /*******************************************/
        /****  Private Fields                   ****/
        /*******************************************/

        private MongoClient m_Client;
        private IMongoCollection<BsonDocument> m_Collection;
        private IMongoCollection<BsonDocument> m_History;
    }
}

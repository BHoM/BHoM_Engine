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

        public bool Push(IEnumerable<object> objects, string tag = "", string config = "")
        {
            DateTime timestamp = DateTime.Now;
            IEnumerable<BsonDocument> documents = objects.Select(x => BHE.Convert.Bson.Write(x, tag, timestamp));
            return m_Link.Push(documents, true, tag);
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

        public bool Execute(string command, List<string> parameters = null, string config = "")
        {
            return false;
        }


        /*******************************************/
        /****  Private Fields                   ****/
        /*******************************************/

        private Link.MongoLink m_Link;
    }
}

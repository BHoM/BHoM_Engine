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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine_Explore.BHoM.Base
{
    public class BHoMAdapter : IAdapter
    {
        public delegate IList PullFunction(string query = "", string config = "");
        public delegate bool PushFunction(IEnumerable data, string tag = "", string config = "");
        public delegate bool ExecuteFunction(string command, List<string> parameters = null, string config = "");
        public delegate bool DeleteFunction(string filter, string config = "");


        /***************************************************/
        /**** Properties                                ****/
        /***************************************************/

        public Dictionary<string, PullFunction> PullFunctions { get; set; } = new Dictionary<string, PullFunction>();

        public Dictionary<string, PushFunction> PushFunctions { get; set; } = new Dictionary<string, PushFunction>();

        public Dictionary<string, ExecuteFunction> ExecuteFunctions { get; set; } = new Dictionary<string, ExecuteFunction>();

        public Dictionary<string, DeleteFunction> DeleteFunctions { get; set; } = new Dictionary<string, DeleteFunction>();

        public List<string> ErrorLog { get; set; } = new List<string>();


        /***************************************************/
        /**** Constructors                              ****/
        /***************************************************/

        public BHoMAdapter() {}


        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public virtual IList Pull(string query, string config = "")
        {
            if (PullFunctions.ContainsKey(query))
                return PullFunctions[query](query, config);
            else
                return null;
        }

        /***************************************************/

        public virtual bool Push(IEnumerable<object> data, string tag = "", string config = "")
        {
            bool ok = true;

            foreach (IGrouping<Type, object> group in ((IEnumerable<object>)data).GroupBy(x => x.GetType()))
            {
                string typeName = group.Key.Name;
                if (PushFunctions.ContainsKey(typeName))
                    ok &= PushFunctions[typeName](group, tag, config);
                else
                    ok = false;
            }

            return ok;
        }

        /***************************************************/

        public virtual bool Delete(string filter = "", string config = "")
        {
            if (DeleteFunctions.ContainsKey(filter))
                return DeleteFunctions[filter](filter, config);
            else
                return false;
        }

        /***************************************************/

        public virtual bool Execute(string command, List<string> parameters = null, string config = "")
        {
            if (ExecuteFunctions.ContainsKey(command))
                return ExecuteFunctions[command](command, parameters, config);
            else
                return false;
        }

    }
}

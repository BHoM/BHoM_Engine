/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2019, the respective contributors. All rights reserved.
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
using System.Data;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using BH.oM.Reflection.Attributes;
using BH.Engine.Reflection;
using BH.oM.Base;
using BH.oM.Data.Collections;

namespace BH.Engine.Data
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Table Table(string name, List<IBHoMObject> bhomObjects, bool ignoreName = false, bool ignoreGuid = true, bool ignoreTags = true)
        {
            IBHoMObject first = bhomObjects.First();

            Dictionary<string, object> properties = first.PropertyDictionary();

            List<DataColumn> columns = new List<DataColumn>();

            properties.CleanUnwantedProperties(ignoreName, ignoreGuid, ignoreTags);

            foreach (var kvp in properties)
            {
                
                columns.Add(new DataColumn(kvp.Key, kvp.Value.GetType()));
            }

            DataTable table = new DataTable();

            table.Columns.AddRange(columns.ToArray());

            foreach (IBHoMObject obj in bhomObjects)
            {
                Dictionary<string, object> props = obj.PropertyDictionary();
                props.CleanUnwantedProperties(ignoreName, ignoreGuid, ignoreTags);

                DataRow row = table.NewRow();

                foreach (var kvp in props)
                {
                    row[kvp.Key] = kvp.Value;
                }
                table.Rows.Add(row);
            }

            return new Table { Data = table, Name = name };
        }

        /***************************************************/

        private static void CleanUnwantedProperties(this Dictionary<string, object> dict, bool ignoreName, bool ignoreGuid, bool ignoreTags)
        {
            if (ignoreName && dict.ContainsKey("Name"))
                dict.Remove("Name");
            if (ignoreGuid && dict.ContainsKey("BHoM_Guid"))
                dict.Remove("BHoM_Guid");
            if (ignoreTags && dict.ContainsKey("Tags"))
                dict.Remove("Tags");

        }

        /***************************************************/

        [Description("")]
        [Input("", "")]
        [Output("", "")]
        public static Table Table<T>(string name, string axis1Name, List<object> axis1Values, string axis2Name, List<object> axis2Values, List<List<T>> values, string valuesName = "Values")
        {

            DataTable table = new DataTable();

            DataColumn[] cols =
                {
                new DataColumn(axis1Name, axis1Values.First().GetType()),
                new DataColumn(axis2Name, axis2Values.First().GetType()),
                new DataColumn(valuesName, typeof(T))
            };


            table.Columns.AddRange(cols);


            if (axis2Values.Count * axis1Values.Count != values.SelectMany(x => x).Count())
            {
                Engine.Reflection.Compute.RecordWarning("Values and header count does not match. Some values might be missing from the table");
            }

            for (int i = 0; i < Math.Min(axis1Values.Count, values.Count); i++)
            {
                for (int j = 0; j < Math.Min(axis2Values.Count, values[i].Count); j++)
                {
                    DataRow row = table.NewRow();
                    row[axis1Name] = axis1Values[i];
                    row[axis2Name] = axis2Values[j];
                    row[valuesName] = values[i][j];
                    table.Rows.Add(row);
                }
            }

            return new Table() { Data = table, Name = name };
            

        }

        /***************************************************/


    }
}

/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2024, the respective contributors. All rights reserved.
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
using BH.oM.Base.Attributes;
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

        [Description("Generates a Table from a list of BHoMObjects. To generate a custom table, use this in conjunction with CustomObjects")]
        [Input("name", "Name of the table")]
        [Input("bhomObjects", "The list of BHoMObjects to turn into a Table")]
        [Input("ignoreName", "Toggles whether to skip the name property or not, defaults to false")]
        [Input("ignoreGuid", "Toggles whether to skip the name Guid or not, defaults to false")]
        [Input("ignoreTags", "Toggles whether to skip the name Tags or not, defaults to false")]
        [Output("Table", "The created Table class")]
        public static Table Table(string name, List<IBHoMObject> bhomObjects, bool ignoreName = false, bool ignoreGuid = true, bool ignoreTags = true)
        {
            IBHoMObject first = bhomObjects.First();

            Dictionary<string, Type> propertyTypes = first.PropertyTypeDictionary();

            List<DataColumn> columns = new List<DataColumn>();

            propertyTypes.CleanUnwantedProperties(ignoreName, ignoreGuid, ignoreTags);

            foreach (var kvp in propertyTypes)
            {
                columns.Add(new DataColumn(kvp.Key, kvp.Value));
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

        [Description("Generates a 2d tables based on the two main axis values in two directions and matrix of internal values.")]
        [Input("name", "Name of the table")]
        [Input("axis1Name", "Name of the first axis")]
        [Input("axis1Values", "Values of the first axis")]
        [Input("axis2Name", "Name of the second axis")]
        [Input("axis2Values", "Values of the second axis")]
        [Input("values", "Main values of the table")]
        [Input("valuesName", "Optional name of the values of the table")]
        [Output("table", "The generated table")]
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
                Engine.Base.Compute.RecordWarning("Values and header count does not match. Some values might be missing from the table");
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
        /**** Private Methods                           ****/
        /***************************************************/

        private static void CleanUnwantedProperties<T>(this Dictionary<string, T> dict, bool ignoreName, bool ignoreGuid, bool ignoreTags)
        {
            if (ignoreName && dict.ContainsKey("Name"))
                dict.Remove("Name");
            if (ignoreGuid && dict.ContainsKey("BHoM_Guid"))
                dict.Remove("BHoM_Guid");
            if (ignoreTags && dict.ContainsKey("Tags"))
                dict.Remove("Tags");
        }

        /***************************************************/

        private static Dictionary<string, Type> PropertyTypeDictionary(this IBHoMObject obj)
        {
            if (obj is CustomObject)
                return PropertyTypeDictionary(obj as CustomObject);

            Dictionary<string, Type> dic = new Dictionary<string, Type>();

            foreach (var prop in obj.GetType().GetProperties())
            {
                if (!prop.CanRead || prop.GetMethod.GetParameters().Count() > 0) continue;

                dic[prop.Name] = prop.PropertyType;
            }
            return dic;
        }

        /***************************************************/

        private static Dictionary<string, Type> PropertyTypeDictionary(this CustomObject obj)
        {
            Dictionary<string, Type> dic = new Dictionary<string, Type>();

            foreach (KeyValuePair<string,object> kvp in obj.CustomData)
            {
                dic[kvp.Key] = kvp.Value.GetType();
            }

            if (!dic.ContainsKey("Name"))
                dic["Name"] = typeof(string);

            if (!dic.ContainsKey("BHoM_Guid"))
                dic["BHoM_Guid"] = typeof(Guid);

            if (!dic.ContainsKey("Tags"))
                dic["Tags"] = obj.Tags.GetType();

            return dic;
        }

        /***************************************************/

    }
}






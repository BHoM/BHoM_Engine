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
using System.ComponentModel;
using BH.oM.Base.Attributes;
using BH.oM.Base;
using BH.oM.Data.Collections;

namespace BH.Engine.Data
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Gets all table rows with an exact match to the values provided. Values returned as CustomObjects")]
        [Input("table", "The table to extract values from")]
        [Input("axes", "The axis of the table to match values for")]
        [Input("values", "The value of the axis to match with")]
        [Output("Data", "The data matching the provided axes and values as CustomObjects.")]
        public static List<CustomObject> ValuesAt(this Table table, List<string> axes, List<object> values)
        {
            if (table == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the values at from a null table.");
                return new List<CustomObject>();
            }

            if (!table.AxisExists(axes))
                return new List<CustomObject>();

            string expression = "";

            for (int i = 0; i < axes.Count; i++)
            {
                expression += axes[i] + " = " + values[i];

                if(i != axes.Count -1)
                    expression+= " AND ";
            }

            return ValuesAt(table, expression);
        }

        /***************************************************/

        [Description("Gets all table rows matching the expression string. Values returned as CustomObjects")]
        [Input("table", "The table to extract values from")]
        [Input("expression", "Expression string for extracting the values from the table.")]
        [Output("Data", "The data matching the provided axes and values as CustomObjects.")]
        public static List<CustomObject> ValuesAt(this Table table, string expression)
        {
            if (table == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the values at from a null table.");
                return new List<CustomObject>();
            }

            return AsCustomObjects(table.Data.Select(expression), table.Data.Columns);
        }

        /***************************************************/

        [Description("Gets all table rows matching the expression string sorted by a specified axis. Values returned as CustomObjects")]
        [Input("table", "The table to extract values from")]
        [Input("expression", "Expression string for extracting the values from the table.")]
        [Input("sortOrder", "The axis the values should be sorted by.")]
        [Output("Data", "The data matching the provided axes and values as CustomObjects.")]
        public static List<CustomObject> ValuesAt(this Table table, string expression, string sortOrder)
        {
            if (table == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the values at from a null table.");
                return new List<CustomObject>();
            }

            return AsCustomObjects(table.Data.Select(expression, sortOrder), table.Data.Columns);
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static List<CustomObject> AsCustomObjects(DataRow[] rows, DataColumnCollection columns)
        {
            List<CustomObject> result = new List<CustomObject>();

            foreach (DataRow row in rows)
            {
                result.Add(AsCustomObject(row, columns));
            }
            return result;
        }

        /***************************************************/

        private static CustomObject AsCustomObject(DataRow row, DataColumnCollection columns)
        {
            CustomObject obj = new CustomObject();
            foreach (DataColumn col in columns)
            {
                if (col.ColumnName == "Name")
                    obj.Name = row[col].ToString();
                else if (col.ColumnName == "Tags")
                    obj.Tags = row[col] as HashSet<string> ?? new HashSet<string>();
                else if (col.ColumnName == "BHoM_Guid")
                    obj.BHoM_Guid = (Guid)row[col];
                else
                    obj.CustomData[col.ColumnName] = row[col];
            }
            return obj;
        }

        /***************************************************/

      
    }
}






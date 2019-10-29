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
using System.ComponentModel;
using BH.oM.Reflection.Attributes;
using BH.oM.Base;
using BH.oM.Data.Collections;

namespace BH.Engine.Data
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("")]
        [Input("", "")]
        [Output("", "")]
        public static List<CustomObject> ValuesAt(this Table table, List<string> axes, List<IComparable> values)
        {
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

        public static List<CustomObject> ValuesAt(this Table table, string expression)
        {
            return AsCustomObjects(table.Data.Select(expression), table.Data.Columns);
        }

        /***************************************************/

        public static List<CustomObject> ValuesAt(this Table table, string expression, string sortOrder)
        {
            return AsCustomObjects(table.Data.Select(expression, sortOrder), table.Data.Columns);
        }

        /***************************************************/

        public static CustomObject FirstValueAt(this Table table, string expression, string sortOrder)
        {
            return AsCustomObject(table.Data.Select(expression, sortOrder).First(), table.Data.Columns);
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
                obj.CustomData[col.ColumnName] = row[col];
            }
            return obj;
        }

        /***************************************************/

      
    }
}

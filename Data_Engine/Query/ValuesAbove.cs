/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2025, the respective contributors. All rights reserved.
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
        /****               Public Methods              ****/
        /***************************************************/

        [Description("Gets all table rows with values exceeding the values provided. Values returned as CustomObjects.")]
        [Input("table", "The table to extract values from.")]
        [Input("axes", "The axis of the table to match values for.")]
        [Input("values", "The value of the axis to match with.")]
        [Input("allowEqual", "Sets whether exact matching values are allowed or not.")]
        [Output("Data", "The data matching the provided axes and values as CustomObjects.")]
        public static List<CustomObject> ValuesAbove(this Table table, List<string> axes, List<object> values, bool allowEqual = true)
        {
            if (table == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the values above from a null table.");
                return new List<CustomObject>();
            }

            if (!table.AxisExists(axes))
                return new List<CustomObject>();
            return ValuesAt(table, AboveExpressionString(axes, values, allowEqual));
        }

        /***************************************************/

        [Description("Gets all table rows with values exeeding the values provided sorted by the sortAxis. Values returned as CustomObjects.")]
        [Input("table", "The table to extract values from.")]
        [Input("axes", "The axis of the table to match values for.")]
        [Input("values", "The value of the axis to match with.")]
        [Input("sortAxis", "The axis the values should be sorted by.")]
        [Input("allowEqual", "Sets whether exact matching values are allowed or not.")]
        [Output("Data", "The data matching the provided axes and values as CustomObjects.")]
        public static List<CustomObject> ValuesAbove(this Table table, List<string> axes, List<object> values, string sortAxis, bool allowEqual = true)
        {
            if (table == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the values above from a null table.");
                return new List<CustomObject>();
            }

            if (!table.AxisExists(axes.Concat(new string[]{sortAxis}).ToList()))
                return new List<CustomObject>();
            return ValuesAt(table, AboveExpressionString(axes, values, allowEqual), sortAxis);
        }

        /***************************************************/
        /****              Private Methods              ****/
        /***************************************************/

        private static string AboveExpressionString(List<string> axes, List<object> values, bool allowEqual)
        {
            string expression = "";
            string operatorSymbol = allowEqual ? " >= " : " > ";
            for (int i = 0; i < axes.Count; i++)
            {
                expression += axes[i] + operatorSymbol + values[i];
                if (i != axes.Count - 1)
                    expression += " AND ";
            }

            return expression;
        }

        /***************************************************/
    }
}
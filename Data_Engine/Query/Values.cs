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
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using BH.oM.Base.Attributes;
using BH.oM.Base;
using BH.oM.Data.Collections;
using BH.Engine.Data;

namespace BH.Engine.Data
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Gets all table rows. Values returned as CustomObjects")]
        [Input("table", "The table to extract values from")]
        [Output("Data", "All data in the table as CustomObjects.")]
        public static List<CustomObject> Values(this Table table)
        {
            if (table == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the values from a null table.");
                return new List<CustomObject>();
            }

            return AsCustomObjects(table.Data.Select(), table.Data.Columns);
        }

        /***************************************************/

        [Description("Gets the data values contained in this node.")]
        [Input("node", "The node to query for its data values.")]
        [Output("data", "Data values contained in this node.")]
        public static IEnumerable<T> IValues<T>(this INode<T> node)
        {
            return Values(node as dynamic) ?? new List<T>();
        }

        /***************************************************/

        [Description("Gets the data values contained in this node.")]
        [Input("node", "The node to query for its data values.")]
        [Output("data", "Data values contained in this node.")]
        public static List<T> Values<T>(this DomainTree<T> node)
        {
            return node?.Values ?? new List<T>();
        }


        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static IEnumerable<T> Values<T>(this INode<T> node)
        {
            Base.Compute.RecordError("The method Values is not implemented for " + node.GetType().Name);
            return null;
        }

        /***************************************************/
    }
}






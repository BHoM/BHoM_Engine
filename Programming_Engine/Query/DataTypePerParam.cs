/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2023, the respective contributors. All rights reserved.
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

using BH.oM.Programming;
using BH.oM.Base.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Programming
{
    public static partial class Query
    {
        /***************************************************/
        /**** Interface Methods                         ****/
        /***************************************************/

        [Description("Collect the data type for each parameter in the input syntax node.")]
        [Input("node", "syntax node to extract data types from")]
        [Output("Dictionary with parameter ids as keys and their type as value")]
        public static Dictionary<Guid, Type> IDataTypePerParam(this INode node)
        {
            return DataTypePerParam(node as dynamic);
        }


        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Collect the data type for each parameter of each syntax node inside the cluster.")]
        [Input("content", "cluster content to extract data types from")]
        [Output("Dictionary with parameter ids as keys and their type as value")]
        public static Dictionary<Guid, Type> DataTypePerParam(this ClusterContent content)
        {
            Dictionary<Guid, Type> types = new Dictionary<Guid, Type>();
            if (content == null)
                return types;

            content.InternalNodes = content.InternalNodes.Select(x => x.IPopulateTypes()).ToList();

            foreach (DataParam input in content.Inputs)
                types[input.BHoM_Guid] = input.DataType;

            foreach (ReceiverParam output in content.Outputs)
                types[output.BHoM_Guid] = output.DataType;

            foreach (INode child in content.InternalNodes)
            {
                Dictionary<Guid, Type> childTypes = child.IDataTypePerParam();
                foreach (KeyValuePair<Guid, Type> kvp in childTypes)
                    types[kvp.Key] = kvp.Value;
            }

            return types;
        }


        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static Dictionary<Guid, Type> DataTypePerParam(this INode node)
        {
            Dictionary<Guid, Type> types = new Dictionary<Guid, Type>();

            foreach (ReceiverParam input in node.Inputs)
                types[input.BHoM_Guid] = input.DataType;

            foreach (DataParam output in node.Outputs)
                types[output.BHoM_Guid] = output.DataType;

            return types;
        }

        /***************************************************/
    }
}




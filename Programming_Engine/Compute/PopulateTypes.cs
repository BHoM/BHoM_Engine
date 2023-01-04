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
    public static partial class Compute
    {
        /***************************************************/
        /**** Interface Methods                         ****/
        /***************************************************/

        [Description("Automatically set the DataType and default value of all parameters of the input syntax node.")]
        [Input("node", "syntax node to modify")]
        [Output("Modified node")]
        public static INode IPopulateTypes(this INode node)
        {
            return PopulateTypes(node as dynamic);
        }


        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Automatically set the DataType and default value of all parameters of the syntax nodes inside the cluster.")]
        [Input("content", "cluster content to modify")]
        [Output("Modified node")]
        public static ClusterContent PopulateTypes(this ClusterContent content)
        {
            if (content == null)
                return content;

            content.InternalNodes = content.InternalNodes.Select(x => x.IPopulateTypes()).ToList();

            // Collect the types
            Dictionary<Guid, Type> nodeTypes = content.DataTypePerParam();

            // Assign the types to the inputs
            foreach (DataParam input in content.Inputs)
            {
                if (input.TargetIds.Count > 0 && nodeTypes.ContainsKey(input.TargetIds[0]))
                    input.DataType = nodeTypes[input.TargetIds[0]];
            }

            // Assign the types to the outputs
            foreach (ReceiverParam output in content.Outputs)
            {
                if (nodeTypes.ContainsKey(output.SourceId))
                    output.DataType = nodeTypes[output.SourceId];
            }

            return content;
        }

        /***************************************************/

        [Description("Automatically set the DataType and default value of all parameters of the input method node.")]
        [Input("node", "method node to modify")]
        [Output("Modified node")]
        public static MethodNode PopulateTypes(this MethodNode node)
        {
            if (node == null || node.Method == null)
                return node;

            ParameterInfo[] methodParams = node.Method.GetParameters();
            if (node.Inputs.Count != methodParams.Length)
                return node;

            for (int i = 0; i < methodParams.Length; i++)
            {
                node.Inputs[i].DataType = methodParams[i].ParameterType;
                if (methodParams[i].HasDefaultValue)
                    node.Inputs[i].DefaultValue = methodParams[i].DefaultValue;
            }
                

            if (node.Outputs.Count == 1)
                node.Outputs[0].DataType = node.Method.ReturnType;
            else if (node.Outputs.Count > 1 && node.Method.ReturnType is oM.Base.IOutput)
            {
                Type[] types = node.Method.ReturnType.GetGenericArguments();
                if (types.Length == node.Outputs.Count)
                {
                    for (int i = 0; i < types.Length; i++)
                        node.Outputs[i].DataType = types[i];
                }
            }

            return node;
        }

        /***************************************************/

        [Description("Automatically set the DataType and default value of all parameters of the input constructor node.")]
        [Input("node", "constructor node to modify")]
        [Output("Modified node")]
        public static ConstructorNode PopulateTypes(this ConstructorNode node)
        {
            if (node == null || node.Constructor == null)
                return node;

            ParameterInfo[] methodParams = node.Constructor.GetParameters();
            if (node.Inputs.Count != methodParams.Length)
                return node;

            for (int i = 0; i < methodParams.Length; i++)
            {
                node.Inputs[i].DataType = methodParams[i].ParameterType;
                if (methodParams[i].HasDefaultValue)
                    node.Inputs[i].DefaultValue = methodParams[i].DefaultValue;
            }


            if (node.Outputs.Count > 0)
                node.Outputs[0].DataType = node.Constructor.DeclaringType;

            return node;
        }

        /***************************************************/

        [Description("Automatically set the DataType and default value of all parameters of the input initialiser node.")]
        [Input("node", "initialiser node to modify")]
        [Output("Modified node")]
        public static InitialiserNode PopulateTypes(this InitialiserNode node)
        {
            if (node == null || node.ObjectType == null)
                return node;

            PropertyInfo[] properties = node.ObjectType.GetProperties();
            object instance = Activator.CreateInstance(node.ObjectType);

            foreach (ReceiverParam input in node.Inputs)
            {
                PropertyInfo match = properties.FirstOrDefault(x => x.Name == input.Name);
                if (match != null)
                {
                    input.DataType = match.PropertyType;
                    input.DefaultValue = match.GetValue(instance);
                }
            }

            if (node.Outputs.Count > 0)
                node.Outputs[0].DataType = node.ObjectType;

            return node;
        }

        /***************************************************/

        [Description("Automatically set the DataType and default value of all parameters of the input type node.")]
        [Input("node", "type node to modify")]
        [Output("Modified node")]
        public static TypeNode PopulateTypes(this TypeNode node)
        {
            if (node == null || node.Type == null)
                return node;

            foreach (DataParam param in node.Outputs)
                param.DataType = typeof(Type);

            return node;
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static INode PopulateTypes(this INode node)
        {
            return node;
        }

        /***************************************************/
    }
}




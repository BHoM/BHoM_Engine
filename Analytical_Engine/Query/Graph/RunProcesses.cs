/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2020, the respective contributors. All rights reserved.
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

using BH.Engine.Base;
using BH.Engine.Reflection;
using BH.oM.Analytical.Elements;
using BH.oM.Base;
using BH.oM.Reflection;
using BH.oM.Reflection.Attributes;
using BH.oM.Reflection.Interface;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Analytical
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns the collection of ProcessResults from all IProcess objects on the Graph relations.")]
        [Input("graph", "The Graph to process.")]
        [Output("process results", "The collection of ProcessResults.")]

        public static List<ProcessResult> RunProcesses(this Graph graph)
        {
            List<ProcessResult> processResults = new List<ProcessResult>();
            foreach (IRelation relation in graph.Relations)
            {
                IBHoMObject source = graph.Entities[relation.Source];
                IBHoMObject target = graph.Entities[relation.Target];
                processResults.AddRange(relation.RunProcesses(source, target));
            }
            return processResults;
        }

        /***************************************************/

        public static List<ProcessResult> RunProcesses(this IRelation relation, IBHoMObject source, IBHoMObject target)   
        {
            List<ProcessResult> processResults = new List<ProcessResult>();

            foreach (Process process in relation.Processes)
            {
                int count = 0;
                foreach (ProcessData data in process.Data)
                {
                    List<object> result = new List<object>();
                    List<string> errors = new List<string>();

                    ParameterInfo[] parameters = process.Method.GetParameters();

                    if(process.SourceParameterIndex > parameters.Length || process.TargetParameterIndex > parameters.Length)
                    {
                        errors.Add("The index of source or target is greater than the parameters required.");
                    }

                    object[] inputs = new object[parameters.Length];

                    int input = 0;

                    for(int i = 0; i < parameters.Length; i++)
                    {
                        if (i == process.SourceParameterIndex)
                            AddToParameters(source, parameters, i, ref errors, ref inputs);

                        else if (i == process.TargetParameterIndex)
                            AddToParameters(target, parameters, i, ref errors, ref inputs);

                        else
                        {
                            //take from the provided inputs
                            if(input < data.Inputs.Count)
                                inputs[i] = data.Inputs[input];
                            else
                                errors.Add("Insufficient inputs were provided.");

                            input++;
                        }
                            
                    }
                    if(!inputs.Any(x => x == null))
                    {
                        object resultObject = process.Method.Invoke(null, inputs);

                        IOutput output = result as IOutput;

                        if (output == null)
                            result.Add(resultObject);

                        else
                        {
                            for (int i = 0; i < output.OutputCount(); i++)
                            {
                                result.Add(output.IItem(i));
                            }
                        }
                    }
                    
                    processResults.Add(new ProcessResult(relation.BHoM_Guid, process.Method.Name + " " + count, -1, result, errors));
                    count++;
                }
                    
            }
            return processResults;
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static void AddToParameters(IBHoMObject obj, ParameterInfo[] parameters, int index, ref List<string> errors, ref object[] inputs)
        {
            if (parameters[index].ParameterType.IsAssignableFrom(obj.GetType()))
            {
                inputs[index] = CastToType(obj, parameters[index].ParameterType);
            }
            else
            {
                errors.Add("The target could not be cast to the corresponding parameter type.");
            }
        }

        /***************************************************/

        private static object CastToType(object item, Type type)
        {
            try
            {
                if (type.IsGenericType && item is IEnumerable)
                {
                    dynamic list;
                    if (type.IsInterface)
                        list = Activator.CreateInstance(typeof(List<>).MakeGenericType(type.GenericTypeArguments[0]));
                    else
                        list = Activator.CreateInstance(type);
                    foreach (dynamic val in (IEnumerable)item)
                    {
                        if (val is IEnumerable)
                            list.Add(CastToType(val, type.GenericTypeArguments[0]));
                        else
                            list.Add(val);
                    }
                    return list;
                }
                else
                {
                    return item;
                }
            }
            catch
            {
                return item;
            }
        }

        /***************************************************/
    }
}

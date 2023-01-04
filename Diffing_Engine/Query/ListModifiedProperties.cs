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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using BH.oM.Base.Attributes;
using BH.oM.Diffing;
using BH.oM.Base;
using System.Reflection;
 

namespace BH.Engine.Diffing
{
    public static partial class Query
    {
        [MultiOutput(0, "identifier", "Identifier of the objects which have some modified properties.\nWhen using Revisions, this is the Hash of the objects. When Diffing using CustomData, this is the specified Id.")]
        [MultiOutput(1, "propNames", "List of properties changed per each object.")]
        [MultiOutput(2, "value_current", "List of current values of the properties.")]
        [MultiOutput(3, "value_past", "List of past values of the properties.")]
        [ToBeRemoved("5.0", "Deprecated due to the new Diff object structure.")]
        public static Output<List<List<string>>, List<List<string>>, List<List<object>>, List<List<object>>> ListModifiedProperties(this Dictionary<string, Dictionary<string, Tuple<object, object>>> modProps, List<string> filterNames = null)
        {
            var output = new Output<List<List<string>>, List<List<string>>, List<List<object>>, List<List<object>>>();

            List<List<string>> objsHashTree = new List<List<string>>();
            List<List<string>> propNameTree = new List<List<string>>();
            List<List<object>> propValue_CurrentTree = new List<List<object>>();
            List<List<object>> propValue_ReadTree = new List<List<object>>();

            // These first empty assignments are needed to avoid UI to throw error "Object not set to an instance of an object" when input is null.
            output.Item1 = objsHashTree;
            output.Item2 = propNameTree;
            output.Item3 = propValue_CurrentTree;
            output.Item4 = propValue_ReadTree;

            if (modProps == null)
                return output;

            foreach (var item in modProps)
            {
                List<string> propNameList = new List<string>();
                List<object> propValue_CurrentList = new List<object>();
                List<object> propValue_ReadList = new List<object>();

                if (item.Value != null)
                    foreach (var propItem in item.Value)
                    {
                        string propName = propItem.Key.Replace(':', '.'); // removes the workaround imposed in DifferentProperties.cs. Allows to have the Explode working while maintaining the correct representation. See BHoM/BHoM_UI#241


                        if ((filterNames == null || filterNames.Count == 0) || filterNames.Any(n => propName.Contains(n)))
                        {
                            propNameList.Add(propName);
                            propValue_CurrentList.Add(propItem.Value.Item1);
                            propValue_ReadList.Add(propItem.Value.Item2);
                        }
                    }

                if (propNameList.Any()) 
{
                    objsHashTree.Add(new List<string>() { item.Key });
                    propNameTree.Add(propNameList);
                    propValue_CurrentTree.Add(propValue_CurrentList);
                    propValue_ReadTree.Add(propValue_ReadList);
                }
            }

            output.Item1 = objsHashTree;
            output.Item2 = propNameTree;
            output.Item3 = propValue_CurrentTree;
            output.Item4 = propValue_ReadTree;

            return output;
        }
    }
}





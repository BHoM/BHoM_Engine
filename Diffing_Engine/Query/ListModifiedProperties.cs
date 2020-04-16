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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using BH.oM.Reflection.Attributes;
using BH.oM.Diffing;
using BH.oM.Base;
using System.Reflection;
using BH.oM.Reflection;

namespace BH.Engine.Diffing
{
    public static partial class Query
    {
        
        [MultiOutput(0, "identifier", "Identifier of the objects which have some modified properties.\nWhen using Revisions, this is the Hash of the objects. When Diffing using CustomData, this is the specified Id.")]
        [MultiOutput(1, "propNames", "List of properties changed per each object.")]
        [MultiOutput(2, "value_Current", "List of current values of the properties.")]
        [MultiOutput(3, "value_Past", "List of past values of the properties.")]
        public static Output<List<List<string>>, List<List<string>>, List<List<object>>, List<List<object>>> ListModifiedProperties(Dictionary<string, Dictionary<string, Tuple<object, object>>> modProps)
        {
            List<List<string>> objsHashTree = new List<List<string>>();
            List<List<string>> propNameTree = new List<List<string>>();
            List<List<object>> propValue_CurrentTree = new List<List<object>>();
            List<List<object>> propValue_ReadTree = new List<List<object>>();

            foreach (var item in modProps)
            {
                objsHashTree.Add(new List<string>() { item.Key });

                List<string> propNameList = new List<string>();
                List<object> propValue_CurrentList = new List<object>();
                List<object> propValue_ReadList = new List<object>();

                foreach (var propItem in item.Value)
                {
                    propNameList.Add(propItem.Key.Replace(':', '.')); // removes the workaround imposed in DifferentProperties.cs. Allows to have the Explode working while maintaining the correct representation. See BHoM/BHoM_UI#241
                    propValue_CurrentList.Add(propItem.Value.Item1);
                    propValue_ReadList.Add(propItem.Value.Item2);
                }

                propNameTree.Add(propNameList);
                propValue_CurrentTree.Add(propValue_CurrentList);
                propValue_ReadTree.Add(propValue_ReadList);
            }


            var output = new Output<List<List<string>>, List<List<string>>, List<List<object>>, List<List<object>>>
            {
                Item1 = objsHashTree,
                Item2 = propNameTree,
                Item3 = propValue_CurrentTree,
                Item4 = propValue_ReadTree
            };

            return output;
        }
    }
}


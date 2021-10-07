/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2021, the respective contributors. All rights reserved.
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;


namespace BH.Engine.Diffing
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<MethodBase> AdaptersDiffingMethods()
        {
            if (m_AdaptersDiffingMethods != null)
                return m_AdaptersDiffingMethods;

            List<MethodBase> adaptersDiffingMethods = new List<MethodBase>();

            List<MethodBase> diffingMethods = BH.Engine.Reflection.Query.AllMethodList()
                .Where(x => x.Name.EndsWith("Diffing"))
                .Where(mi => mi.DeclaringType.Module.Name != typeof(BH.Engine.Diffing.Query).Module.Name)
                .ToList();


            // Checks on the parameters.
            foreach (var mi in diffingMethods)
            {
                ParameterInfo[] inputParams = mi.GetParameters();

                // Check the number of input paramters. A valid Diffing method needs at least 3 input parameters: two lists of objects to compare and a DiffConfig.
                if (inputParams.Length < 3)
                    continue; 

                // Check that the first two input parameters are IEnumerables.
                if (!(typeof(IEnumerable).IsAssignableFrom(inputParams[0].ParameterType) && typeof(IEnumerable).IsAssignableFrom(inputParams[1].ParameterType)))
                    continue;

                // Check that there is exactly one DiffingConfig parameter.
                try
                {
                    inputParams.Single(pi => typeof(BH.oM.Diffing.DiffingConfig).IsAssignableFrom(pi.ParameterType));
                }
                catch (Exception e)
                {
                    // There is either no DiffingConfig input parameter or more than one.
                    continue;
                }

                adaptersDiffingMethods.Add(mi);
            }

            m_AdaptersDiffingMethods = adaptersDiffingMethods;

            return adaptersDiffingMethods;
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        internal static Dictionary<string, MethodBase> AdaptersDiffingMethods_perNamespace()
        {
            List<MethodBase> adaptersDiffingMethods = AdaptersDiffingMethods();

            var AdaptersDiffingMethods_GroupedPerNamespace = adaptersDiffingMethods.GroupBy(m => m.DeclaringType.Namespace);

            foreach (var g in AdaptersDiffingMethods_GroupedPerNamespace)
            {
                if (g.Count() > 1) {
                    BH.Engine.Reflection.Compute.RecordError($"{g.Count()} Diffing methods found in namespace {g.Key}. Only one is allowed. Returning only the first one.");
                }
            }

            return AdaptersDiffingMethods_GroupedPerNamespace.ToDictionary(g => g.Key, g => g.FirstOrDefault());
        }


        /***************************************************/
        /**** Private Fields                            ****/
        /***************************************************/

        private static List<MethodBase> m_AdaptersDiffingMethods = null;
    }
}



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

using BH.oM.Base.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;


namespace BH.Engine.Diffing
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Return all the public Diffing methods found in Toolkits (i.e. not in the BHoM_Engine)." +
            "\nA Diffing method is a public method whose name ends with 'Diffing' and that has these parameters:" +
            "an IEnumerable<objects> for the past set; an IEnumerable<objects> for the following set;" +
            "any number of other optional parameters; and one DiffingConfig parameter.")]
        [Output("Diffing methods found in all Toolkits (i.e. not in the BHoM_Engine).")]
        public static List<MethodBase> AdaptersDiffingMethods()
        {
            if (m_AdaptersDiffingMethods != null)
                return m_AdaptersDiffingMethods;

            List<MethodBase> adaptersDiffingMethods = new List<MethodBase>();

            List<MethodBase> diffingMethods = BH.Engine.Base.Query.AllMethodList()
                .Where(x => x.IsPublic)
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
        /**** Private Fields                            ****/
        /***************************************************/

        private static List<MethodBase> m_AdaptersDiffingMethods = null;
    }
}





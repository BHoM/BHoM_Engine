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
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using BH.oM.Reflection.Attributes;
using BH.oM.Physical.Elements;

using BH.Engine.Base;
using BH.oM.Physical.FramingProperties;
using BH.oM.Physical.Materials;

namespace BH.Engine.Physical
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static double Density(this Material material, Type type = null)
        {
            if (type == null)
                type = typeof(IMaterialProperties);

            List<double> densities = new List<double>();

            foreach (IMaterialProperties mat in material.Properties.Where(x => type.IsAssignableFrom(x.GetType())))
            {
                try
                {
                    densities.Add((double)Engine.Reflection.Query.PropertyValue(mat, "Density"));
                }
                catch
                { }
            }

            if (densities.Count == 0)
            {
                Reflection.Compute.RecordWarning("no density on any of the fragments of " + material.Name + " by type " + type.Name);
                return 0;
            }
            if (densities.Count > 1)
                Reflection.Compute.RecordWarning("Average of multiple Fragments taken from " + material.Name);

            return densities.Average();
        }

        /***************************************************/

    }
}

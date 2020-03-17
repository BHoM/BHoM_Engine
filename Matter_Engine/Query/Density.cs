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
using BH.oM.Reflection;
using BH.oM.Quantities.Attributes;

namespace BH.Engine.Matter
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Gets the density of a Material by querying each of its IMaterialProperties for theirs." +
                     "The density is gotten from a property with that name." +
                     "If the density is found on diffrent IMaterialProperties, not result will be returned.")]
        [Input("material", "The material to get the density of")]
        [Input("type", "The kind of IMaterialProperties to cull the result by, use this if multiple IMaterialProperties have densities")]
        [Output("density", "The density of the material, further info on how the value was accuired is recorded in the warning", typeof(Density))]
        public static double Density(this Material material, Type type = null, double tolerance = 0.001)
        {
            if (type == null)
                type = typeof(IMaterialProperties);

            List<double> densities = new List<double>();
            List<string> notes = new List<string>() { "Density report for the Material " + material.Name + ":" };

            foreach (IMaterialProperties mat in material.Properties.Where(x => type.IsAssignableFrom(x.GetType())))
            {
                object density = Reflection.Query.PropertyValue(mat, "Density");
                if (density != null)
                {
                    densities.Add((double)density);
                    notes.Add("The value of the density for the MaterialFragment: " + mat.Name + " was acquired through its properties");
                }
            }

            if (densities.Count == 0)
            {
                Reflection.Compute.RecordWarning("no density on any of the fragments of " + material.Name + " by type " + type.Name);
                return 0;
            }
            if (densities.Count > 1 && !CheckRange(densities, tolerance))
            {
                Reflection.Compute.RecordWarning("The density for " + material.Name + " is found on multiple IMaterialProperties, please specify one type for a result");
                return double.NaN;
            }
            if (densities.Count > 1)
                notes.Add("");

            Reflection.Compute.RecordNote(string.Join(System.Environment.NewLine, notes.ToArray()));

            return densities.First();
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static bool CheckRange(IEnumerable<double> numbers, double relativeTolerance, double zeroTolerance = 1e-6)
        {
            double min = numbers.Min();
            double max = numbers.Max();

            if (max < zeroTolerance)
                return true;

            double mean = (min + max) / 2;
            
            return (max - min) / mean < relativeTolerance;
        }

        /***************************************************/

    }
}

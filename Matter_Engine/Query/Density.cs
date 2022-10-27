/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2022, the respective contributors. All rights reserved.
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
using BH.oM.Physical.Elements;

using BH.Engine.Base;
using BH.oM.Physical.FramingProperties;
using BH.oM.Physical.Materials;
 
using BH.oM.Quantities.Attributes;

namespace BH.Engine.Matter
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns the density of a Material though querying each of the individual IMaterialProperties that implement IDensityProvider." +
                     "\nIf inconsistent density values are found on multiple different IMaterialProperties, no result will be returned.")]
        [Input("material", "The material to query density from.")]
        [Input("type", "A specific type of IDensityProvider to limit the search to. Set a preferred type here if multiple IMaterialProperties are implementing IDensityProvider.")]
        [Input("tolerance", "The ratio tolerance for considering the value of the densities as equal." +
                            "\nCompares to the differance between min and max over their mean.", typeof(Ratio))]
        [Output("density", "The density of the material. Additional info on how the value has been acquired is recorded in the warning", typeof(Density))]
        public static double Density(this Material material, Type type = null, double tolerance = 0.001)
        {
            if(material == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the density of a null material.");
                return 0;
            }

            if (type == null)
                type = typeof(IDensityProvider);
            else if (!typeof(IDensityProvider).IsAssignableFrom(type))
            {
                Base.Compute.RecordError($"Type provided for density extraction need to be a type implementing {nameof(IDensityProvider)}. Please provide a valid type or null to allow all types.");
                return double.NaN;
            }

            List<double> densities = new List<double>();
            List<string> notes = new List<string>() { "Density report for the Material " + material.Name + ":" };

            foreach (IDensityProvider mat in material.Properties.Where(x => type.IsAssignableFrom(x.GetType())).Cast<IDensityProvider>())
            {
                densities.Add(mat.Density);
                notes.Add("The value of the density for the MaterialFragment: " + mat.Name + " was acquired through its properties");
            }

            if (densities.Count == 0)
            {
                Base.Compute.RecordWarning("no density on any of the fragments of " + material.Name + " by type " + type.Name);
                return 0;
            }
            if (densities.Count > 1 && !CheckRange(densities, tolerance))
            {
                Base.Compute.RecordWarning("Multiple unique values for density found across multiple IMaterialProperties for " + material.Name + ". Please either ensure consistency of values or provide a specific material property type to define a valid density.");
                return double.NaN;
            }
            if (densities.Count > 1)
                notes.Add("");

            Base.Compute.RecordNote(string.Join(System.Environment.NewLine, notes.ToArray()));

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



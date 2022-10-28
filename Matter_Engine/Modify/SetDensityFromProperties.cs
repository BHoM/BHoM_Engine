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

using BH.oM.Base;
using BH.oM.Base.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using BH.oM.Physical.Materials;
using BH.oM.Physical.Materials.Options;
using BH.Engine.Base;
using BH.oM.Quantities.Attributes;

namespace BH.Engine.Matter
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Assignes the density to the material based on its stored IMaterialProperties.")]
        [Input("material", "The material to modify the density of.")]
        [Input("options", "Options controling how the density should be extracted from the Proeprties of the Material.")]
        [Output("material", "The material with density updated to match the proeprties depending on the options input.")]
        public static void SetDensityFromProperties(this Material material, DensityExtractionOptions options)
        {
            if (material == null)
            {
                Base.Compute.RecordError("Assign the density of a null material.");
                return;
            }

            //Gets all properties on the material able to store density
            List<IDensityProvider> allDensityProviders = material.Properties.OfType<IDensityProvider>().ToList();

            if (allDensityProviders.Count == 0) //Nothing available -> return
            {
                Base.Compute.RecordWarning($"Material {material.Name} does not contain any properties able to store density. Density not modified.");
                return;
            }

            options = options ?? new DensityExtractionOptions();    //Set up default options

            //Handle type filtering
            Type type = options.Type;
            List<IDensityProvider> densityProviders;
            if (type == null)
                densityProviders = allDensityProviders;
            else if (typeof(IDensityProvider).IsAssignableFrom(type))   //Check type filter valid
            {
                //Filter by the provided type
                densityProviders = allDensityProviders.Where(x => type.IsAssignableFrom(x.GetType())).ToList();
                if (densityProviders.Count == 0)    //No items found matching type filter
                {
                    if (options.AllowFallbackIfNoType)  //If allowing fallback, use all providers rather than filtered out ones
                    {
                        Base.Compute.RecordWarning($"No MaterialProperty of type {type.Name}. Falling back to extracting density from other properties with density.");
                        densityProviders = allDensityProviders;
                    }
                    else    //Else raise warning and return
                    {
                        Base.Compute.RecordWarning($"No density on any of the properties of {material.Name} of type {type.Name}. To allow falling back to other avilable properties set {nameof(options.AllowFallbackIfNoType)} to true.");
                        return;
                    }
                }
            }
            else
            {
                Base.Compute.RecordError($"Type provided for density extraction need to be a type implementing {nameof(IDensityProvider)}. Please provide a valid type or null to allow all types.");
                return;
            }


            //Get all density values
            List<double> densities = densityProviders.Select(x => x.Density).ToList();

            if (options.IgnoreZeroValues)   //If true, density values smaller than the 0 tolerance should be ignored
            {
                if (densities.All(x => x <= options.ZeroTolerance)) //If all densities are below the 0 threshold, do not filter, as that means removing all
                {
                    Base.Compute.RecordWarning($"All density values are below the {nameof(options.ZeroTolerance)} threshold. No Densities filtered out.");
                }
                else
                {
                    densities = densities.Where(x => x > options.ZeroTolerance).ToList();
                }
            }

            if (densities.Count == 1)   //Single value - simply return
            {
                material.Density = densities[0];
                return;
            }

            //More than single value, extract depending on ExtractionType setting
            
            switch (options.ExtractionType)
            {
                case DensityExtractionType.Average:
                    material.Density = densities.Average();
                    return;
                case DensityExtractionType.Maximum:
                    material.Density = densities.Max();
                    return;
                case DensityExtractionType.Minimum:
                    material.Density = densities.Min();
                    return;
                default:
                case DensityExtractionType.AllMatching:
                    if (CheckRange(densities, options.EqualTolerance, options.ZeroTolerance))
                    {
                        material.Density = densities.Average();
                        return;
                    }
                    else
                    {
                        Base.Compute.RecordWarning($"Multiple unique values for density found across multiple IMaterialProperties for {material.Name} outside the allowable range set by {nameof(options.EqualTolerance)}. Please either ensure consistency of values or provide a specific material property type to define a valid density or change the {nameof(DensityExtractionType)}.");
                        return;
                    }

            }
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


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
using BH.oM.Physical.Materials.Options;
 
using BH.oM.Quantities.Attributes;

namespace BH.Engine.Matter
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [PreviousVersion("6.0", "BH.Engine.Matter.Query.Density(BH.oM.Physical.Materials.Material, System.Type, System.Double)")]
        [Description("Returns the density of a Material.")]
        [Input("material", "The material to query density from.")]
        [Output("density", "The density of the material.", typeof(Density))]
        public static double Density(this Material material)
        {
            //Method kept for versioning reasons
            return material.Density;
        }

        /***************************************************/

        public static double Density(this IEnumerable<IDensityProvider> densityProviders, DensityExtractionOptions options, bool raiseWarnings = true)
        {
            if (densityProviders == null)
            {
                if (raiseWarnings)
                    BH.Engine.Base.Compute.RecordError($"Cannot extract Density from a null list of {nameof(IDensityProvider)}s.");
                return double.NaN;
            }
            //Gets all properties on the material able to store density
            List<IDensityProvider> allDensityProviders = densityProviders.ToList();

            if (allDensityProviders.Count == 0) //Nothing available -> return
            {
                if(raiseWarnings)
                    Base.Compute.RecordWarning($"Material does not contain any properties able to store density. Density not modified.");
                return double.NaN;
            }

            options = options ?? new DensityExtractionOptions();    //Set up default options

            //Handle type filtering
            Type type = options.Type;
            List<IDensityProvider> filteredDensityProviders;
            if (type == null)
                filteredDensityProviders = allDensityProviders;
            else if (typeof(IDensityProvider).IsAssignableFrom(type))   //Check type filter valid
            {
                //Filter by the provided type
                filteredDensityProviders = allDensityProviders.Where(x => type.IsAssignableFrom(x.GetType())).ToList();
                if (filteredDensityProviders.Count == 0)    //No items found matching type filter
                {
                    if (options.AllowFallbackIfNoType)  //If allowing fallback, use all providers rather than filtered out ones
                    {
                        if(raiseWarnings)
                            Base.Compute.RecordWarning($"No {nameof(IDensityProvider)} of type {type.Name}. Falling back to extracting density from other properties with density.");
                        filteredDensityProviders = allDensityProviders;
                    }
                    else    //Else raise warning and return
                    {
                        if(raiseWarnings)
                            Base.Compute.RecordWarning($"No {nameof(IDensityProvider)} of type {type.Name} found. To allow falling back to other avilable properties set {nameof(options.AllowFallbackIfNoType)} to true.");
                        return double.NaN;
                    }
                }
            }
            else
            {
                if(raiseWarnings)
                    Base.Compute.RecordError($"Type provided for density extraction need to be a type implementing {nameof(IDensityProvider)}. Please provide a valid type or null to allow all types.");
                return double.NaN;
            }


            //Get all density values
            List<double> densities = filteredDensityProviders.Select(x => x.Density).ToList();

            if (options.IgnoreZeroValues)   //If true, density values smaller than the 0 tolerance should be ignored
            {
                if (densities.All(x => x <= options.ZeroTolerance)) //If all densities are below the 0 threshold, do not filter, as that means removing all
                {
                    if(raiseWarnings)
                        Base.Compute.RecordWarning($"All density values are below the {nameof(options.ZeroTolerance)} threshold. No Densities filtered out.");
                }
                else
                {
                    densities = densities.Where(x => x > options.ZeroTolerance).ToList();
                }
            }

            if (densities.Count == 1)   //Single value - simply return
            {
                return densities[0];
            }

            //More than single value, extract depending on ExtractionType setting
            switch (options.ExtractionType)
            {
                case DensityExtractionType.Average:
                    return densities.Average();
                case DensityExtractionType.Maximum:
                    return densities.Max();
                case DensityExtractionType.Minimum:
                    return densities.Min();
                default:
                case DensityExtractionType.AllMatching:
                    if (CheckRange(densities, options.EqualTolerance, options.ZeroTolerance))
                    {
                        return densities.Average();
                    }
                    else
                    {
                        if(raiseWarnings)
                            Base.Compute.RecordWarning($"Multiple unique values for density found across multiple {nameof(IDensityProvider)} outside the allowable range set by {nameof(options.EqualTolerance)}. Please either ensure consistency of values or provide a specific material property type to define a valid density or change the {nameof(DensityExtractionType)}.");
                        return double.NaN;
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



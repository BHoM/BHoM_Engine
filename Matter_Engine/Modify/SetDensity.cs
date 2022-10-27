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
using System.Reflection;
using BH.oM.Physical.Materials;
using BH.Engine.Base;
using BH.oM.Quantities.Attributes;

namespace BH.Engine.Matter
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Sets the density to the IMaterialProperties of the physical Material.")]
        [Input("material", "The material to set the density to.")]
        [Input("density", "Density to set to the material.", typeof(Density))]
        [Input("typeFilter", "Optional filter to only set the density to properties of a specific type.")]
        [Input("templateAddType", "Optional template type to use if no appropriate IMaterialProperty types exists on the Material. Needs to be a Type that implementes the IDensityProvider interface. This input is ignored if the material contains any properties able to store density.")]
        [Output("material", "Material with density set according to the options provided.")]
        public static Material SetDensity(this Material material, double density, Type typeFilter = null, Type templateAddType = null)
        {
            if (material == null)
                return null;

            Material clone = material.DeepClone();

            List<IDensityProvider> densityProperties = clone.Properties.OfType<IDensityProvider>().ToList();

            if (densityProperties.Count != 0)
            {
                if (typeFilter != null)
                    densityProperties = densityProperties.Where(x => typeFilter.IsAssignableFrom(x.GetType())).ToList();

                if(densityProperties.Count == 0)
                    Base.Compute.RecordWarning($"Material does not contain any properties of type {typeFilter.Name} able to store density.");

                foreach (IDensityProvider matProp in densityProperties)
                {
                    matProp.Density = density;
                }
            }
            else if (templateAddType != null)
            {
                if (typeof(IDensityProvider).IsAssignableFrom(templateAddType) && !templateAddType.IsInterface && !templateAddType.IsAbstract)
                {
                    IDensityProvider densityProvider = Activator.CreateInstance(templateAddType) as IDensityProvider;
                    densityProvider.Density = density;
                    clone.Properties.Add(densityProvider);
                }
                else
                    Base.Compute.RecordError($"{nameof(templateAddType)} need to be a non-abstract, non-interface type implementing IDensityProvider to be able to add it to the Properties of the Material.");
            }
            else
                Base.Compute.RecordWarning("Material does not contain any properties able to store density.");

            return clone;
        }

        /***************************************************/
    }
}

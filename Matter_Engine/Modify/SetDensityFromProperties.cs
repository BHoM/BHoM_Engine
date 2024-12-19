/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2025, the respective contributors. All rights reserved.
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
using BH.oM.Matter.Options;
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
        public static Material SetDensityFromProperties(this Material material, DensityExtractionOptions options = null)
        {
            if (material == null)
            {
                Base.Compute.RecordError("Assign the density of a null material.");
                return null;
            }

            Material clone = material.ShallowClone();

            double density = material.Properties.OfType<IDensityProvider>().Density(options, true);
            if (!double.IsNaN(density))
                clone.Density = density;

            return clone;
        }

        /***************************************************/

    }
}





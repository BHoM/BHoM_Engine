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

namespace BH.Engine.Matter
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Checks the density on all IDensityProviders on the material has the same density as the density on the Material within given tolerance.")]
        [Input("material", "The material to check.")]
        [Input("tolerance", "The allowable difference for two densities to be deemed the same.")]
        [Output("equal", "Returns true if all densities on the material and its proerties are matching.")]
        public static bool AllDensitiesEqual(this Material material, double tolerance = 1e-6)
        {
            if (material == null)
                return false;

            foreach (IDensityProvider densityProvider in material.Properties.OfType<IDensityProvider>())
            {
                if (Math.Abs(densityProvider.Density - material.Density) > tolerance)
                    return false;
            }
            return true;
        }

        /***************************************************/
    }
}

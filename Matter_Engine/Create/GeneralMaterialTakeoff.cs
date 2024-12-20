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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.oM.Physical.Constructions;
using BH.oM.Physical.Materials;

using BH.oM.Base.Attributes;
using System.ComponentModel;
using BH.oM.Physical.Elements;
using BH.oM.Geometry;
using BH.oM.Dimensional;
using BH.oM.Quantities.Attributes;
using BH.Engine.Base;
using BH.oM.Base;

namespace BH.Engine.Matter
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates a GeneralMaterialTakeoff based on the VolumetricMaterialTakeoff. Quantities beyound Volume and Mass of the returned GeneralMaterialTakeoff will be unset.")]
        [Input("volumetricMaterialTakeoff", "The VolumetricMaterialTakeoff to be used to create the GeneralMaterialTakeoff. Materials from the VolumetricMaterialTakeoff will be used with corresponing volumes as well as densities to populate mass and volume values of the general material takeoff.")]
        [Output("generalMaterialTakeoff", "A GeneralMaterialTakeoff composed of the Materials in the provided VolumetricMaterialTakeoff and volumes as well as mass set.")]
        public static GeneralMaterialTakeoff GeneralMaterialTakeoff(VolumetricMaterialTakeoff volumetricMaterialTakeoff)
        {
            if (volumetricMaterialTakeoff == null)
            {
                Base.Compute.RecordError($"Cannot create a {nameof(GeneralMaterialTakeoff)} from a null {nameof(VolumetricMaterialTakeoff)}.");
                return null;
            }

            return (GeneralMaterialTakeoff)volumetricMaterialTakeoff;
        }

        /***************************************************/

    }
}




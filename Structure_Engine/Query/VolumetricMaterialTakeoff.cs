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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using BH.oM.Dimensional;
using BH.Engine.Geometry;
using BH.Engine.Spatial;
using BH.oM.Base.Attributes;
using BH.oM.Physical.Materials;
using BH.oM.Structure.Elements;
using BH.oM.Structure.MaterialFragments;

namespace BH.Engine.Structure
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Gets the volumetric material takeoff from the PileFoundation object. The takeoff will contain materials and volumes from the PileCap and Piles.")]
        [Input("pileFoundation", "The PileFoundation object to extract the volumetric material takeoff from.")]
        [Output("volTakeoff", "The volumetric material takeoff based on buildup of the PileFoundation object.")]
        public static VolumetricMaterialTakeoff VolumetricMaterialTakeoff(this PileFoundation pileFoundation)
        {
            if (pileFoundation.IsNull())
                return null;

            if (pileFoundation.PileCap.Property.IsNull() || pileFoundation.PileCap.Property.Material.IsNull())
                return null;

            if (pileFoundation.Piles.Any(x => x.Section.IsNull()) || pileFoundation.Piles.Any(x => x.Section.Material.IsNull()))
                return null;

            List<VolumetricMaterialTakeoff> takeoffs = new List<VolumetricMaterialTakeoff>();

            takeoffs.Add(Matter.Query.IVolumetricMaterialTakeoff(pileFoundation.PileCap));
            foreach (Pile pile in pileFoundation.Piles)
            {
                takeoffs.Add(VolumetricMaterialTakeoff(pile));
            }

            return Matter.Compute.AggregateVolumetricMaterialTakeoff(takeoffs);
        }

        /***************************************************/

        [Description("Gets the volumetric material takeoff from the Pile object.")]
        [Input("pile", "The PileFoundation object to extract the volumetric material takeoff from.")]
        [Output("volTakeoff", "The volumetric material takeoff based on buildup of the Pile object.")]
        public static VolumetricMaterialTakeoff VolumetricMaterialTakeoff(this Pile pile)
        {
            if (pile.IsNull())
                return null;

            if (pile.Section.IsNull() || pile.Section.Material.IsNull())
                return null;

            IMaterialFragment structMaterial = pile.Section.Material;

            Material physMaterial = new Material() { Density = structMaterial.Density, Name = structMaterial.Name };

            VolumetricMaterialTakeoff takeOff = Matter.Create.VolumetricMaterialTakeoff(new List<Material>() { physMaterial }, new List<double>() { pile.Section.Area * pile.Length() });

            return takeOff;
        }

        /***************************************************/

        [Description("Gets the volumetric material takeoff from the PadFoundation object.")]
        [Input("padFoundation", "The PadFoundation object to extract the volumetric material takeoff from.")]
        [Output("volTakeoff", "The volumetric material takeoff based on buildup of the PadFoundation object.")]
        public static VolumetricMaterialTakeoff VolumetricMaterialTakeoff(this PadFoundation padFoundation)
        {
            if (padFoundation.IsNull())
                return null;

            if (padFoundation.Property.IsNull() || padFoundation.Property.Material.IsNull())
                return null;

            IMaterialFragment structMaterial = padFoundation.Property.Material;

            Material physMaterial = new Material() { Density = structMaterial.Density, Name = structMaterial.Name };

            VolumetricMaterialTakeoff takeOff = Matter.Create.VolumetricMaterialTakeoff(new List<Material>() { physMaterial }, new List<double>() { padFoundation.SolidVolume() });

            return takeOff;
        }

        /***************************************************/

    }
}

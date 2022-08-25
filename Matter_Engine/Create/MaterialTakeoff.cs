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

        [Description("Creates a MaterialTakeoff for a collection of Materials given their volumes.")]
        [Input("materials", "The discrete Materials the MaterialTakeoff is comprised of.")]
        [Input("volumes", "The ratios of each material based on their relative volumes. The number of ratios must match the number of materials.", typeof(Volume))]
        [Output("materialTakeoff", "A MaterialTakeoff composed of the provided materials and volumes.")]
        public static MaterialTakeoff MaterialTakeoff(IEnumerable<Material> materials, IEnumerable<double> volumes)
        {
            if (materials.IsNullOrEmpty() || volumes.IsNullOrEmpty())
                return null;

            if (materials.Count() != volumes.Count())
            {
                Base.Compute.RecordError("Requires the same number of materials as ratios to create a MaterialComposition.");
                return null;
            }


            return new MaterialTakeoff(materials, volumes);
        }

        /***************************************************/

        [Description("Creates a MaterialTakeoff based on the materials in the provided MaterialComposition and ratios scaloed with the provided total volume.")]
        [Input("materialComposition", "The MaterialComposition to be used to create the MaterialTakeoff. Materials of the MaterialComposition will be assigned to the takeoff and rations scaled with the provided totalVolume to give the volumes for each material.")]
        [Input("totalVolume", "Total volume of the MaterialTakeoff. Ratios of the MaterialComposition are scaled with this value to give the volume for each material part.", typeof(Volume))]
        [Output("materialTakeoff", "A MaterialTakeoff composed of the Materials in the provided MaterialComposition and volumes as its ratios scaled with the provided totalVolume.")]
        public static MaterialTakeoff MaterialTakeoff(MaterialComposition materialComposition, double totalVolume)
        {
            if (materialComposition == null)
            {
                Base.Compute.RecordError("Cannot create a MaterialTakeoff from a null MaterialComposition.");
                return null;
            }

            return new MaterialTakeoff(materialComposition.Materials, materialComposition.Ratios.Select(x => x*totalVolume));
        }

        /***************************************************/
    }
}

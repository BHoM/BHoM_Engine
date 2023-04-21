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

using BH.oM.Base;
using BH.oM.Base.Attributes;
using BH.oM.Quantities.Attributes;
using BH.oM.Structure.MaterialFragments;
using BH.oM.Structure.SurfaceProperties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [PreviousVersion("6.2", "BH.Engine.Structure.Create.ElogatedCircularVoidHollowCore(System.Double, System.Double, System.Double, System.Double, BH.oM.Structure.MaterialFragments.Concrete, System.String)")]
        [Description("Creates a HollowCore surface property with elongated circular voids.")]
        [InputFromProperty("thickness")]
        [Input("holeHeight", "Height of the voided elongated circular holes.", typeof(Length))]
        [Input("holeWidth", "Width of the voided elongated circular holes. This also corresponds to the diamter of the circular ends of the voids.", typeof(Length))]
        [Input("holeSpacing", "Centre-Centre spacing of the voided holes.", typeof(Length))]
        [Input("material", "Concrete material of the hollow core. Default concrete material will be assigned if nothing is provided.")]
        [InputFromProperty("direction")]
        [InputFromProperty("name")]
        [Output("hollowCore", "The created hollow core property.")]
        public static HollowCore ElogatedCircularVoidHollowCore(double thickness, double holeHeight, double holeWidth, double holeSpacing, Concrete material = null, PanelDirection direction = PanelDirection.X, string name = "")
        {
            if (thickness < holeHeight)
            {
                Base.Compute.RecordError("Thickness needs to be larger than the hole diameter.");
                return null;
            }

            if (holeSpacing < holeWidth)
            {
                Base.Compute.RecordError("Spacing needs to be larger than the hole diameter.");
                return null;
            }

            if (holeHeight < holeWidth)
            {
                Base.Compute.RecordError("Height needs to be larger than the hole diameter.");
                return null;
            }

            if (material == null)
                material = Query.Default(MaterialType.Concrete) as Concrete;

            return new HollowCore
            {
                Thickness = thickness,
                Material = material,
                Name = name,
                Direction = direction,
                Openings = new ElongatedCircularHollowCoreOpeningProfiles { Height = holeHeight, Width = holeWidth, Spacing = holeSpacing }
            };
        }

        /***************************************************/
    }
}

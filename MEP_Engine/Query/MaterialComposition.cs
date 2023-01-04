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
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using BH.oM.Base.Attributes;
using BH.oM.MEP.System;
using BH.oM.Physical.Materials;
using BH.oM.Quantities.Attributes;

namespace BH.Engine.MEP
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Gets all the Materials a Duct is composed of and in which ratios")]
        [Input("duct", "The Duct to get the MaterialComposition from")]
        [Output("materialComposition", "The kind of matter the Duct is composed of and in which ratios", typeof(Ratio))]
        public static MaterialComposition MaterialComposition(this Duct duct)
        {
            if(duct == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the material composition of a null duct.");
                return null;
            }

            if (duct.SectionProperty == null)
            {
                Engine.Base.Compute.RecordError("The Duct MaterialComposition could not be calculated as no SectionProperty has been assigned.");
                return null;
            }

            List<Material> materials = null;
            List<double> ratios = null;

            materials = new List<Material>() { duct.SectionProperty.DuctMaterial, duct.SectionProperty.LiningMaterial, duct.SectionProperty.InsulationMaterial };
            ratios = new List<double>() { duct.SectionProperty.ElementSolidArea, duct.SectionProperty.LiningSolidArea, duct.SectionProperty.InsulationSolidArea };

            return Matter.Create.MaterialComposition(materials, ratios);
        }

        /***************************************************/

        [Description("Gets all the Materials a pipe is composed of and in which ratios")]
        [Input("pipe", "The pipe to get the MaterialComposition from")]
        [Output("materialComposition", "The kind of matter the Duct is composed of and in which ratios", typeof(Ratio))]
        public static MaterialComposition MaterialComposition(this Pipe pipe)
        {
            if(pipe == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the material composition of a null pipe.");
                return null;
            }

            if (pipe.SectionProperty == null)
            {
                Engine.Base.Compute.RecordError("The Duct MaterialComposition could not be calculated as no SectionProperty has been assigned.");
                return null;
            }

            List<Material> materials = null;
            List<double> ratios = null;

            materials = new List<Material>() { pipe.SectionProperty.PipeMaterial, pipe.SectionProperty.InsulationMaterial };
            ratios = new List<double>() { pipe.SectionProperty.ElementSolidArea, pipe.SectionProperty.InsulationSolidArea };

            return Matter.Create.MaterialComposition(materials, ratios);
        }

        /***************************************************/

        [Description("Gets all the Materials a wire is composed of and in which ratios")]
        [Input("wire", "The wire to get the MaterialComposition from")]
        [Output("materialComposition", "The kind of matter the Duct is composed of and in which ratios", typeof(Ratio))]
        public static MaterialComposition MaterialComposition(this WireSegment wire)
        {
            if(wire == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the material composition of a null wire segment.");
                return null;
            }

            if (wire.SectionProperty == null)
            {
                Engine.Base.Compute.RecordError("The Duct MaterialComposition could not be calculated as no SectionProperty has been assigned.");
                return null;
            }

            List<Material> materials = null;
            List<double> ratios = null;

            materials = new List<Material>() { wire.SectionProperty.ConductiveMaterial, wire.SectionProperty.InsulationMaterial };
            ratios = new List<double>() { wire.SectionProperty.ElementSolidArea, wire.SectionProperty.InsulationSolidArea };

            return Matter.Create.MaterialComposition(materials, ratios);
        }

        /***************************************************/

        [Description("Gets all the Materials a cableTray is composed of and in which ratios")]
        [Input("cableTray", "The cableTray to get the MaterialComposition from")]
        [Output("materialComposition", "The kind of matter the Duct is composed of and in which ratios", typeof(Ratio))]
        public static MaterialComposition MaterialComposition(this CableTray cableTray)
        {
            if(cableTray == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the material composition of a null cable tray.");
                return null;
            }

            if (cableTray.SectionProperty == null)
            {
                Engine.Base.Compute.RecordError("The Duct MaterialComposition could not be calculated as no SectionProperty has been assigned.");
                return null;
            }

            List<Material> materials = null;
            List<double> ratios = null;

            materials = new List<Material>() { cableTray.SectionProperty.Material };
            ratios = new List<double>() { cableTray.SectionProperty.ElementSolidArea };

            return Matter.Create.MaterialComposition(materials, ratios);
        }

        /***************************************************/
    }
}



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

using System.ComponentModel;
using BH.oM.Base.Attributes;
using BH.oM.MEP.System.MaterialFragments;
using BH.oM.MEP.System.SectionProperties;
using BH.Engine.Spatial;
using BH.Engine.Geometry;
using BH.oM.Physical.Materials;

namespace BH.Engine.MEP
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/
        [Description("Creates a composite Wire sectionProfile including interior lining and exterior insulation.")]
        [Input("sectionProfile", "A base ShapeProfile upon which to base the composite section.")]
        [Input("conductiveMaterial", "Material properties for the Wire object.")]
        [Input("insulationMaterial", "Material properties for the insulation material, or material that wraps the exterior of the Wire object.")]
        [Output("wireSectionProperty", "Wire Section property used to provide accurate wire assembly and capacities.")]
        public static WireSectionProperty WireSectionProperty(SectionProfile sectionProfile, Material conductiveMaterial = null, Material insulationMaterial = null, string name = "")
        {
            if (sectionProfile == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot create a wire section property with a null section profile.");
                return null;
            }

            double elementSolidArea = sectionProfile.ElementProfile.Area();
            double elementVoidArea = sectionProfile.ElementProfile.VoidArea();

            double liningSolidArea = 0;
            double liningVoidArea = double.NaN;

            double insulationSolidArea = 0;
            double insulationVoidArea = double.NaN;

            if (sectionProfile.LiningProfile != null)
            {
                liningSolidArea = sectionProfile.LiningProfile.Area();
                liningVoidArea = sectionProfile.LiningProfile.VoidArea();
            }

            if (sectionProfile.InsulationProfile != null)
            {
                insulationSolidArea = sectionProfile.InsulationProfile.Area();
                insulationVoidArea = sectionProfile.InsulationProfile.VoidArea();
            }

            WireSectionProperty property = new WireSectionProperty(sectionProfile, elementSolidArea, liningSolidArea, insulationSolidArea, elementVoidArea, liningVoidArea, insulationVoidArea);
            property.ConductiveMaterial = conductiveMaterial;
            property.InsulationMaterial = insulationMaterial;
            property.Name = name;

            if (property == null)
            {
                BH.Engine.Base.Compute.RecordError("Insufficient information to create a WireSectionProperty. Please ensure you have all required inputs.");
            }
            return property;
        }
        /***************************************************/
    }
}






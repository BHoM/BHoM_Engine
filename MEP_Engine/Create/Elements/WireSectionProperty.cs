/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2020, the respective contributors. All rights reserved.
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.oM.Reflection.Attributes;
using BH.oM.MEP.MaterialFragments;
using BH.oM.MEP.SectionProperties;
using BH.Engine.Spatial;
using BH.Engine.Geometry;

namespace BH.Engine.MEP
{
    public static partial class Create
    {
        [Description("Creates a composite Wire sectionProfile including interior lining and exterior insulation.")]
        [Input("sectionProfile", "A base ShapeProfile upon which to base the composite section.")]
        [Input("ductMaterial", "Material properties for the Wire object.")]
        [Input("insulationMaterial", "Material properties for the insulation material, or material that wraps the exterior of the Wire object.")]
        [Input("liningMaterial", "Material properties for the lining material that wraps the inside surface of the Wire object. This is the layer that is in direct contact with interior flowing material.")]
        public static WireSectionProperty WireSectionProperty(
            SectionProfile sectionProfile,
            IMEPMaterial conductiveMaterial = null,
            IMEPMaterial insulationMaterial = null,
            string name = "")
        {
            //Solid Areas
            double elementSolidArea = sectionProfile.ElementProfile.Area();
            double liningSolidArea = sectionProfile.LiningProfile.Area() - sectionProfile.ElementProfile.Area();
            double insulationSolidArea = sectionProfile.InsulationProfile.Area();

            //Void Areas
            double elementVoidArea = sectionProfile.ElementProfile.VoidArea();
            double liningVoidArea = sectionProfile.LiningProfile.VoidArea();
            double insulationVoidArea = sectionProfile.InsulationProfile.VoidArea();

            WireSectionProperty property = new WireSectionProperty(sectionProfile, elementSolidArea, liningSolidArea, insulationSolidArea, elementVoidArea, liningVoidArea, insulationVoidArea);
            property.ConductiveMaterial = conductiveMaterial;
            property.InsulationMaterial = insulationMaterial;
            property.Name = name;

            if (property == null)
            {
                BH.Engine.Reflection.Compute.RecordError("Insufficient information to create a WireSectionProperty. Please ensure you have all required inputs.");
            }
            return property;
        }
    }
}

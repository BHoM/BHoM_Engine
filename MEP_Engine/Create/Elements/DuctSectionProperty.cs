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

using BH.oM.MEP.MaterialFragments;
using BH.oM.MEP.System.SectionProperties;
using BH.Engine.Spatial;
using BH.Engine.Geometry;
using BH.oM.Reflection.Attributes;

namespace BH.Engine.MEP
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/
        [Description("Creates a composite Duct sectionProfile including interior lining and exterior insulation.")]
        [Input("sectionProfile", "A base ShapeProfile upon which to base the composite section.")]
        [Input("ductMaterial", "Material properties for the Duct object.")]
        [Input("insulationMaterial", "Material properties for the insulation material, or material that wraps the exterior of the Duct object.")]
        [Input("liningMaterial", "Material properties for the lining material that wraps the inside surface of the Duct object. This is the layer that is in direct contact with interior flowing material.")]
        [Output("ductSectionProperty", "Duct Section property used to provide accurate duct assembly and capacities.")]
        public static DuctSectionProperty DuctSectionProperty(
            SectionProfile sectionProfile,
            IMEPMaterial ductMaterial = null,
            IMEPMaterial insulationMaterial = null,
            IMEPMaterial liningMaterial = null,
            string name = "")
        {
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

            if(sectionProfile.InsulationProfile != null)
            {
                insulationSolidArea = sectionProfile.InsulationProfile.Area();
                insulationVoidArea = sectionProfile.InsulationProfile.VoidArea();
            }

            //Duct specific properties
            double circularEquivalent = sectionProfile.ElementProfile.ICircularEquivalentDiameter();
            double hydraulicDiameter = sectionProfile.ElementProfile.HydraulicDiameter(liningVoidArea);

            DuctSectionProperty property = new DuctSectionProperty(sectionProfile, elementSolidArea, liningSolidArea, insulationSolidArea, elementVoidArea, liningVoidArea, insulationVoidArea, hydraulicDiameter, circularEquivalent);
            property.DuctMaterial = ductMaterial;
            property.InsulationMaterial = insulationMaterial;
            property.LiningMaterial = liningMaterial;
            property.Name = name;

            if (property == null)
            {
                BH.Engine.Reflection.Compute.RecordError("Insufficient information to create a DuctSectionProperty. Please ensure you have all required inputs.");
            }
            return property;
        }
        /***************************************************/
    }
}

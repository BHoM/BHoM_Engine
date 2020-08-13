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

using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.oM.Reflection.Attributes;
using BH.oM.Geometry.ShapeProfiles;
using BH.oM.MEP.SectionProperties;

namespace BH.Engine.MEP
{
    public static partial class Create
    {
        [Description("Creates a composite section profile from the desired ShapeProfile. The composition is inclusive of interior Lining and exterior Insulation thicknesses.")]
        [Input("shapeProfile", "A base ShapeProfile to base the composite SectionProfile. Currently only BoxProfiles and TubeProfiles are supported.")]
        [Input("liningThickness", "Thickness for the interior duct lining to be added to the overall section profile.")]
        [Input("insulationThickness", "Thickness for the exterior duct insulation to be added to the overall section profile.")]

        public static SectionProfile SectionProfile(BoxProfile boxProfile, double liningThickness, double insulationThickness)
        {
            //Internal offset of original ShapeProfile
            IProfile liningProfile = BH.Engine.Geometry.Create.BoxProfile((boxProfile.Height - (boxProfile.Thickness * 2)), (boxProfile.Width - (boxProfile.Thickness * 2)), liningThickness, boxProfile.OuterRadius, boxProfile.InnerRadius);

            //External offset of original ShapeProfile
            IProfile insulationProfile = BH.Engine.Geometry.Create.BoxProfile((boxProfile.Height + (insulationThickness * 2)), (boxProfile.Width + (insulationThickness * 2)), insulationThickness, boxProfile.InnerRadius, boxProfile.OuterRadius);

            return new oM.MEP.SectionProperties.SectionProfile
            {
                ElementProfile = boxProfile,
                LiningProfile = liningProfile,
                InsulationProfile = insulationProfile,
            };
        }

        [Description("Creates a composite section profile from the desired ShapeProfile. The composition is inclusive of interior Lining and exterior Insulation thicknesses.")]
        [Input("shapeProfile", "A base ShapeProfile to base the composite SectionProfile. Currently only BoxProfiles and TubeProfiles are supported.")]
        [Input("liningThickness", "Thickness for the interior duct lining to be added to the overall section profile.")]
        [Input("insulationThickness", "Thickness for the exterior duct insulation to be added to the overall section profile.")]

        public static SectionProfile SectionProfile(TubeProfile tubeProfile, double liningThickness, double insulationThickness)
        {
            //Internal offset of original ShapeProfile
            IProfile liningProfile = BH.Engine.Geometry.Create.TubeProfile(tubeProfile.Diameter, tubeProfile.Thickness + liningThickness);

            //External offset of original ShapeProfile
            IProfile insulationProfile = BH.Engine.Geometry.Create.TubeProfile(tubeProfile.Diameter, (insulationThickness * -1));

            return new oM.MEP.SectionProperties.SectionProfile
            {
                ElementProfile = tubeProfile,
                LiningProfile = liningProfile,
                InsulationProfile = insulationProfile,
            };
        }
    }
}
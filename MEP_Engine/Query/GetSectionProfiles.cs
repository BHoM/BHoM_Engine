/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2021, the respective contributors. All rights reserved.
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
using BH.oM.Reflection.Attributes;
using BH.oM.MEP.System;
using BH.oM.Spatial.ShapeProfiles;
using System.Collections.Generic;
using BH.Engine.Spatial;
using BH.oM.MEP.System.MaterialFragments;

namespace BH.Engine.MEP
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Queries the object's SectionProfiles.")]
        [Input("obj", "The object to query the SectionProfiles.")]
        [Output("sectionProfiles", "The Element's SectionProfiles.")]

        public static List<IProfile> GetSectionProfiles(this IFlow obj)
        {
            SystemMaterialFragment systemMaterialFragment = obj.SectionProperty.Materials;
            IProfile elementProfile = obj.SectionProperty.Profile;
            List<IProfile> profiles = new List<IProfile>();

            if (obj.SectionProperty.Materials.LiningThickness > 0)
            {
                IProfile liningProfile = null;
                if (obj.SectionProperty.Profile.Shape == ShapeType.Box)
                {
                    double height = ((BoxProfile)obj.SectionProperty.Profile).Height;
                    double width = ((BoxProfile)obj.SectionProperty.Profile).Width;
                    double elementThickness = ((BoxProfile)obj.SectionProperty.Profile).Thickness;
                    double liningThickness = obj.SectionProperty.Materials.LiningThickness;
                    double outerRad = ((BoxProfile)obj.SectionProperty.Profile).OuterRadius;
                    double innerRad = ((BoxProfile)obj.SectionProperty.Profile).InnerRadius;

                    liningProfile = Spatial.Create.BoxProfile((height - (elementThickness * 2)), (width - (elementThickness * 2)), liningThickness, outerRad, innerRad);
                    profiles.Add(liningProfile);
                }
                if (obj.SectionProperty.Profile.Shape == ShapeType.Tube)
                {
                    double diameter = ((TubeProfile)obj.SectionProperty.Profile).Diameter;
                    double elementThickness = ((TubeProfile)obj.SectionProperty.Profile).Thickness;
                    double liningThickness = obj.SectionProperty.LiningThickness;

                    liningProfile = Spatial.Create.TubeProfile((((diameter / 2) - elementThickness) * 2), liningThickness);
                    profiles.Add(liningProfile);
                }
            }

            if (obj.SectionProperty.Materials.InsulationThickness > 0)
            {
                IProfile insulationProfile = null;
                if (obj.SectionProperty.Profile.Shape == ShapeType.Box)
                {
                    double height = ((BoxProfile)obj.SectionProperty.Profile).Height;
                    double width = ((BoxProfile)obj.SectionProperty.Profile).Width;
                    double elementThickness = ((BoxProfile)obj.SectionProperty.Profile).Thickness;
                    double insulationThickness = obj.SectionProperty.Materials.InsulationThickness;
                    double outerRad = ((BoxProfile)obj.SectionProperty.Profile).OuterRadius;
                    double innerRad = ((BoxProfile)obj.SectionProperty.Profile).InnerRadius;

                    insulationProfile = Spatial.Create.BoxProfile((height + (insulationThickness * 2)), (width + (insulationThickness * 2)), insulationThickness, innerRad, outerRad);
                    profiles.Add(insulationProfile);
                }
                if (obj.SectionProperty.Profile.Shape == ShapeType.Tube)
                {
                    double diameter = ((TubeProfile)obj.SectionProperty.Profile).Diameter;
                    double elementThickness = ((TubeProfile)obj.SectionProperty.Profile).Thickness;
                    double insulationThickness = obj.SectionProperty.LiningThickness;

                    insulationProfile = Spatial.Create.TubeProfile((((diameter / 2) - elementThickness) * 2), insulationThickness);
                    profiles.Add(insulationProfile);
                }
            }
            return profiles;
        }

        /***************************************************/
    }
}


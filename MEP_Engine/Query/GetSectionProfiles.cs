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
using System.Linq;
using BH.oM.MEP.Enums;
using BH.oM.MEP.System.SectionProperties;

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
            ShapeType elementShape = obj.ElementSize.Shape;

            List<IProfile> profiles = new List<IProfile>();

            // Add element profile
            if (obj.SectionProfile.Where(x => x.Type == ProfileType.Element).Count() > 0)
            {
                IProfile elementProfile = null;

                if (elementShape != ShapeType.Box && elementShape != ShapeType.Tube && elementShape != ShapeType.Channel)
                {
                    BH.Engine.Reflection.Compute.RecordError("Only Box, Tube, and Channel profiles are supported. You can request additional profiles by raising and issue within the BHoM Github repository.");
                    return null;
                }

                if (elementShape == ShapeType.Box)
                {
                    double height = obj.ElementSize.Height;
                    double width = obj.ElementSize.Width;
                    double elementThickness = obj.SectionProfile.Where(x => x.Type == ProfileType.Element).First().Layer.Select(x => x.Thickness).Sum();
                    double outerRad = obj.ElementSize.OuterRadius;
                    double innerRad = obj.ElementSize.InnerRadius;

                    elementProfile = Spatial.Create.BoxProfile(height, width, elementThickness, outerRad, innerRad);
                    profiles.Add(elementProfile);
                }
                if (elementShape == ShapeType.Tube)
                {
                    double diameter = obj.ElementSize.Diameter;
                    double elementThickness = obj.SectionProfile.Where(x => x.Type == ProfileType.Element).First().Layer.Select(x => x.Thickness).Sum();

                    elementProfile = Spatial.Create.TubeProfile(diameter, elementThickness);
                    profiles.Add(elementProfile);
                }
                if (elementShape == ShapeType.Channel)
                {
                    // add channel support
                }
            }
            // Add Lining Profile
            if (obj.SectionProfile.Where(x => x.Type == ProfileType.Lining).Count() > 0)
            {
                IProfile liningProfile = null;
                if (elementShape == ShapeType.Box)
                {
                    double height = obj.ElementSize.Height;
                    double width = obj.ElementSize.Width;
                    double elementThickness = obj.SectionProfile.Where(x => x.Type == ProfileType.Element).First().Layer.Select(x => x.Thickness).Sum();
                    double liningThickness = obj.SectionProfile.Where(x => x.Type == ProfileType.Lining).First().Layer.Select(x => x.Thickness).Sum();
                    double outerRad = obj.ElementSize.OuterRadius;
                    double innerRad = obj.ElementSize.InnerRadius;

                    liningProfile = Spatial.Create.BoxProfile((height - (elementThickness * 2)), (width - (elementThickness * 2)), liningThickness, outerRad, innerRad);
                    profiles.Add(liningProfile);
                }
                if (elementShape == ShapeType.Tube)
                {
                    double diameter = obj.ElementSize.Diameter;
                    double elementThickness = obj.SectionProfile.Where(x => x.Type == ProfileType.Element).First().Layer.Select(x => x.Thickness).Sum();
                    double liningThickness = obj.SectionProfile.Where(x => x.Type == ProfileType.Lining).First().Layer.Select(x => x.Thickness).Sum();

                    liningProfile = Spatial.Create.TubeProfile((((diameter / 2) - elementThickness) * 2), liningThickness);
                    profiles.Add(liningProfile);
                }
                if (elementShape == ShapeType.Channel)
                {
                    // add channel support
                }
            }
            // Add Insulation Profile 
            if (obj.SectionProfile.Where(x => x.Type == ProfileType.Insulation).Count() > 0)
            {
                IProfile insulationProfile = null;
                if (elementShape == ShapeType.Box)
                {
                    double height = obj.ElementSize.Height;
                    double width = obj.ElementSize.Width;
                    double elementThickness = obj.SectionProfile.Where(x => x.Type == ProfileType.Element).First().Layer.Select(x => x.Thickness).Sum();
                    double insulationThickness = obj.SectionProfile.Where(x => x.Type == ProfileType.Insulation).First().Layer.Select(x => x.Thickness).Sum();
                    double outerRad = obj.ElementSize.OuterRadius;
                    double innerRad = obj.ElementSize.InnerRadius;

                    insulationProfile = Spatial.Create.BoxProfile((height + (insulationThickness * 2)), (width + (insulationThickness * 2)), insulationThickness, innerRad, outerRad);
                    profiles.Add(insulationProfile);
                }
                if (elementShape == ShapeType.Tube)
                {
                    double diameter = obj.ElementSize.Diameter;
                    double elementThickness = obj.SectionProfile.Where(x => x.Type == ProfileType.Element).First().Layer.Select(x => x.Thickness).Sum();
                    double insulationThickness = obj.SectionProfile.Where(x => x.Type == ProfileType.Insulation).First().Layer.Select(x => x.Thickness).Sum(); // I changed this ProfileType.Insulation from Lining if there are errors. 

                    insulationProfile = Spatial.Create.TubeProfile((((diameter / 2) - elementThickness) * 2), insulationThickness);
                    profiles.Add(insulationProfile);
                }
            }
            return profiles;
        }

        /***************************************************/
    }
}


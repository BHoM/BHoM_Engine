/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2019, the respective contributors. All rights reserved.
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

using BH.oM.Geometry;
using BH.Engine.Geometry;
using BH.oM.Architecture.Theatron;
using System.Collections.Generic;
using System;
using BH.Engine.Base;
using System.ComponentModel;
using BH.oM.Reflection.Attributes;

namespace BH.Engine.Architecture.Theatron
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/
        [Description("Create a full profile from one or more ProfileParameters")]
        [Input("parameters", "List of ProfileParameters")]
        public static TheatronFullProfile TheatronFullProfile(List<ProfileParameters> parameters)
        {
            //this assumes no relation with the plan geometry setting out is from the origin
            TheatronFullProfile fullProfile = new TheatronFullProfile();
            double minDist = parameters[0].StartX - parameters[0].EyePositionParameters.EyePositionX;
            Point origin = Geometry.Create.Point(minDist, 0, parameters[0].StartZ - parameters[0].EyePositionParameters.EyePositionZ);
            Vector direction = Vector.XAxis;
            ProfileOrigin sectionOrigin = ProfileOrigin(origin, direction);
            GenerateMapProfiles(ref fullProfile, parameters.DeepClone(), minDist, sectionOrigin);
            return fullProfile;
        }

        /***************************************************/
        [Description("Create a full profile from one or more ProfileParameters and a TheatronPlan geometry. The worst case section will be found and used to define the profile geometry")]
        [Input("parameters", "List of ProfileParameters")]
        [Input("planGeometry", "A TheatronPlan")]
        public static TheatronFullProfile TheatronFullProfile(List<ProfileParameters> parameters, TheatronPlan planGeometry)
        {
            
            TheatronFullProfile fullProfile = new TheatronFullProfile();
            
            GenerateMapProfiles(ref fullProfile, parameters.DeepClone(), planGeometry.MinDistToFocalCurve, planGeometry.SectionClosestToFocalCurve);
            
            return fullProfile;
        }
        /***************************************************/
        [Description("Create a full profile from one or more ProfileParameters and a focal point and ProfileOrigin")]
        [Input("parameters", "List of ProfileParameters")]
        [Input("planGeometry", "A TheatronPlan")]
        public static TheatronFullProfile TheatronFullProfile(List<ProfileParameters> parameters, Point focalPoint, ProfileOrigin sectionOrigin)
        {
            //this assumes no relation with the plan geometry setting out is from the origin
            TheatronFullProfile fullProfile = new TheatronFullProfile();
            Point lastpoint = new Point();
            //fullProfile.FocalPoint = focalPoint;
            Vector focalToStart = sectionOrigin.Origin - focalPoint;
            focalToStart.Z = 0;
            GenerateMapProfiles(ref fullProfile, parameters.DeepClone(), focalToStart.Length(), sectionOrigin);
            
            return fullProfile;
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static void GenerateMapProfiles(ref TheatronFullProfile fullProfile, List<ProfileParameters> parameters,double distToFocalCurve, ProfileOrigin sectionOrigin)
        {
            Point lastpoint = new Point();

            for (int i = 0; i < parameters.Count; i++)
            {
                if (i == 0)
                {
                    parameters[i].StartX = distToFocalCurve + parameters[i].RowWidth - parameters[i].EyePositionParameters.EyePositionX;
                }
                TierProfile tierSection = TierProfile(parameters[i], lastpoint);
                fullProfile.BaseTierProfiles.Add(tierSection);
                
                if (i == 0)
                {
                    fullProfile.FullProfileOrigin = fullProfile.BaseTierProfiles[0].SectionOrigin;
                    fullProfile.FullProfileOrigin.Origin.Z = 0;
                }
                Point source = fullProfile.FullProfileOrigin.Origin;
                Point target = sectionOrigin.Origin;
                double angle = Math.Atan2(sectionOrigin.Direction.Y, sectionOrigin.Direction.X);
                Vector scaleVector = SetScaleVector(tierSection.SectionOrigin.Direction, tierSection.SectionOrigin, tierSection.SectionOrigin);
                fullProfile.MappedProfiles.Add(TransformProfile(tierSection, scaleVector,source, target, angle));
                lastpoint = tierSection.FloorPoints[tierSection.FloorPoints.Count - 1];

            }
            
        }

        /***************************************************/

        
    }
}

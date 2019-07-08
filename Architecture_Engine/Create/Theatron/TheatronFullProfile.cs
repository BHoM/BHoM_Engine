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

namespace BH.Engine.Architecture.Theatron
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static TheatronFullProfile TheatronFullProfile(List<ProfileParameters> parameters)
        {
            //this assumes no relation with the plan geometry setting out is from the origin
            TheatronFullProfile fullProfile = new TheatronFullProfile();
            generateProfiles(ref fullProfile, parameters);
            return fullProfile;
        }

        /***************************************************/

        public static TheatronFullProfile TheatronFullProfile(List<ProfileParameters> parameters, TheatronPlan planGeometry)
        {
            
            TheatronFullProfile fullProfile = new TheatronFullProfile();
            Point lastpoint = new Point();
            fullProfile.FocalPoint = planGeometry.CValueFocalPoint;
            generateMapProfiles(ref fullProfile, parameters, planGeometry.MinDistToFocalCurve, planGeometry.SectionClosestToFocalCurve);
            
            return fullProfile;
        }
        /***************************************************/

        public static TheatronFullProfile TheatronFullProfile(List<ProfileParameters> parameters, Point focalPoint, ProfileOrigin sectionOrigin)
        {
            //this assumes no relation with the plan geometry setting out is from the origin
            TheatronFullProfile fullProfile = new TheatronFullProfile();
            Point lastpoint = new Point();
            fullProfile.FocalPoint = focalPoint;
            Vector focalToStart = sectionOrigin.Origin - focalPoint;
            focalToStart.Z = 0;
            generateMapProfiles(ref fullProfile, parameters, focalToStart.Length(), sectionOrigin);
            
            return fullProfile;
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static void generateProfiles(ref TheatronFullProfile fullProfile, List<ProfileParameters> parameters)
        {
            Point lastpoint = new Point();

            for (int i = 0; i < parameters.Count; i++)
            {
                TierProfile tierSection = Create.TierProfile(parameters[i], lastpoint);
                fullProfile.BaseTierProfiles.Add(tierSection);
                lastpoint = tierSection.FloorPoints[tierSection.FloorPoints.Count - 1];

            }
            fullProfile.FullProfileOrigin = fullProfile.BaseTierProfiles[0].SectionOrigin;
        }

        /***************************************************/

        private static void generateMapProfiles(ref TheatronFullProfile fullProfile, List<ProfileParameters> parameters,double distToFocalCurve, ProfileOrigin sectionOrigin)
        {
            Point lastpoint = new Point();

            for (int i = 0; i < parameters.Count; i++)
            {
                if (i == 0)
                {
                    parameters[i].StartX = distToFocalCurve + parameters[i].RowWidth - parameters[i].EyePositionX;
                }
                TierProfile tierSection = Create.TierProfile(parameters[i], lastpoint);
                fullProfile.BaseTierProfiles.Add(tierSection);
                Point target = sectionOrigin.Origin;
                
                double angle = Math.Atan2(sectionOrigin.Direction.Y, sectionOrigin.Direction.X);

                fullProfile.MappedProfiles.Add(mapTierToPlane(tierSection, 1, (target), angle));
                lastpoint = tierSection.FloorPoints[tierSection.FloorPoints.Count - 1];

            }
            fullProfile.FullProfileOrigin = fullProfile.BaseTierProfiles[0].SectionOrigin;
        }

        /***************************************************/

        
    }
}

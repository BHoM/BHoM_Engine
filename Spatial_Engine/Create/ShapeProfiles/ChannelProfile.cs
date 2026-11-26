/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2026, the respective contributors. All rights reserved.
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

using BH.oM.Base.Attributes;
using BH.oM.Geometry;
using BH.oM.Spatial.ShapeProfiles;
using System.Collections.Generic;
using System.ComponentModel;

namespace BH.Engine.Spatial
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates a C-shaped profile based on input dimensions. Method generates edge curves based on the inputs.")]
        [InputFromProperty("height")]
        [InputFromProperty("flangeWidth")]
        [InputFromProperty("webThickness")]
        [InputFromProperty("flangeThickness")]
        [InputFromProperty("rootRadius")]
        [InputFromProperty("toeRadius")]
        [InputFromProperty("mirrorAboutLocalZ")]
        [Output("channel", "The created ChannelProfile.")]
        public static ChannelProfile ChannelProfile(double height, double flangeWidth, double webThickness, double flangeThickness, double rootRadius = 0, double toeRadius = 0, bool mirrorAboutLocalZ = false)
        {
            if (!AreFinite(height, flangeWidth, webThickness, flangeThickness, rootRadius, toeRadius))
                return null;

            if (!ArePositive(height, flangeWidth, webThickness, flangeThickness) || !AreNonNegative(rootRadius, toeRadius))
                return null;

            if (height < flangeThickness * 2 + rootRadius * 2 || height <= flangeThickness * 2)
            {
                InvalidRatioError(nameof(height), $"{nameof(flangeThickness)} and {nameof(rootRadius)}");
                return null;
            }

            if (flangeWidth < webThickness + rootRadius + toeRadius)
            {
                InvalidRatioError(nameof(flangeWidth), $"{nameof(webThickness)}, {nameof(toeRadius)} and {nameof(rootRadius)}");
                return null;
            }

            if (flangeThickness < toeRadius)
            {
                InvalidRatioError(nameof(flangeThickness), nameof(toeRadius));
                return null;
            }

            List<ICurve> curves = ChannelProfileCurves(height, flangeWidth, webThickness, flangeThickness, rootRadius, toeRadius, mirrorAboutLocalZ);
            return new ChannelProfile(height, flangeWidth, webThickness, flangeThickness, rootRadius, toeRadius, mirrorAboutLocalZ, curves);
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static List<ICurve> ChannelProfileCurves(double height, double width, double webthickness, double flangeThickness, double rootRadius, double toeRadius, bool mirrorAboutLocalZ)
        {
            //Web along the y-axis and flanges pointing in positive x, with the mid height at the origin
            double y = height / 2;
            double yf = y - flangeThickness;
            List<ProfileCorner> corners = new List<ProfileCorner>
            {
                new ProfileCorner(0, -y, 0), new ProfileCorner(width, -y, 0), new ProfileCorner(width, -yf, toeRadius), new ProfileCorner(webthickness, -yf, rootRadius),
                new ProfileCorner(webthickness, yf, rootRadius), new ProfileCorner(width, yf, toeRadius), new ProfileCorner(width, y, 0), new ProfileCorner(0, y, 0)
            };

            return ProfileCurves(true, corners.MirrorCorners(mirrorAboutLocalZ, false));
        }

        /***************************************************/

    }
}







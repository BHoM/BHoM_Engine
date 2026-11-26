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

using System.Collections.Generic;
using BH.oM.Spatial.ShapeProfiles;
using BH.oM.Geometry;
using BH.oM.Base.Attributes;
using System.ComponentModel;

namespace BH.Engine.Spatial
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates a L-shaped profile based on input dimensions. Method generates edge curves based on the inputs.")]
        [InputFromProperty("height")]
        [InputFromProperty("width")]
        [InputFromProperty("webThickness")]
        [InputFromProperty("flangeThickness")]
        [InputFromProperty("rootRadius")]
        [InputFromProperty("toeRadius")]
        [InputFromProperty("mirrorAboutLocalZ")]
        [InputFromProperty("mirrorAboutLocalY")]
        [Output("angle", "The created AngleProfile.")]
        public static AngleProfile AngleProfile(double height, double width, double webThickness, double flangeThickness, double rootRadius = 0, double toeRadius = 0, bool mirrorAboutLocalZ = false, bool mirrorAboutLocalY = false)
        {
            if (!AreFinite(height, width, webThickness, flangeThickness, rootRadius, toeRadius))
                return null;

            if (!ArePositive(height, width, webThickness, flangeThickness) || !AreNonNegative(rootRadius, toeRadius))
                return null;

            if (height < flangeThickness + rootRadius + toeRadius)
            {
                InvalidRatioError(nameof(height), $"{nameof(flangeThickness)}, {nameof(rootRadius)} and {nameof(toeRadius)}");
                return null;
            }

            if (width < webThickness + rootRadius + toeRadius)
            {
                InvalidRatioError(nameof(width), $"{nameof(webThickness)}, {nameof(rootRadius)} and {nameof(toeRadius)}");
                return null;
            }

            if (flangeThickness < toeRadius)
            {
                InvalidRatioError(nameof(flangeThickness), nameof(toeRadius));
                return null;
            }

            if (webThickness < toeRadius)
            {
                InvalidRatioError(nameof(webThickness), nameof(toeRadius));
                return null;
            }

            List<ICurve> curves = AngleProfileCurves(width, height, flangeThickness, webThickness, rootRadius, toeRadius, mirrorAboutLocalZ, mirrorAboutLocalY);
            return new AngleProfile(height, width, webThickness, flangeThickness, rootRadius, toeRadius, mirrorAboutLocalZ, mirrorAboutLocalY, curves);
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static List<ICurve> AngleProfileCurves(double width, double depth, double flangeThickness, double webThickness, double innerRadius, double toeRadius, bool mirrorAboutLocalZ, bool mirrorAboutLocalY)
        {
            //Heel at the origin, horizontal leg along positive x and vertical leg along positive y
            List<ProfileCorner> corners = new List<ProfileCorner>
            {
                new ProfileCorner(0, 0, 0), new ProfileCorner(width, 0, 0), new ProfileCorner(width, flangeThickness, toeRadius),
                new ProfileCorner(webThickness, flangeThickness, innerRadius), new ProfileCorner(webThickness, depth, toeRadius), new ProfileCorner(0, depth, 0)
            };

            return ProfileCurves(true, corners.MirrorCorners(mirrorAboutLocalZ, mirrorAboutLocalY));
        }

        /***************************************************/
    }
}







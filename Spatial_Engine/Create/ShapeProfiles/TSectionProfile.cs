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

        [Description("Creates a T-shaped profile based on input dimensions. Method generates edge curves based on the inputs.")]
        [InputFromProperty("height")]
        [InputFromProperty("width")]
        [InputFromProperty("webThickness")]
        [InputFromProperty("flangeThickness")]
        [InputFromProperty("rootRadius")]
        [InputFromProperty("toeRadius")]
        [InputFromProperty("mirrorAboutLocalY")]
        [Output("T", "The created TSectionProfile.")]
        public static TSectionProfile TSectionProfile(double height, double width, double webThickness, double flangeThickness, double rootRadius = 0, double toeRadius = 0, bool mirrorAboutLocalY = false)
        {
            if (!AreFinite(height, width, webThickness, flangeThickness, rootRadius, toeRadius))
                return null;

            if (!ArePositive(height, width, webThickness, flangeThickness) || !AreNonNegative(rootRadius, toeRadius))
                return null;

            if (height < flangeThickness + rootRadius)
            {
                InvalidRatioError(nameof(height), $"{nameof(flangeThickness)} and {nameof(rootRadius)}");
                return null;
            }

            if (width < webThickness + 2 * rootRadius + 2 * toeRadius)
            {
                InvalidRatioError(nameof(width), $"{nameof(webThickness)}, {nameof(rootRadius)} and {nameof(toeRadius)}");
                return null;
            }

            if (toeRadius > flangeThickness)
            {
                InvalidRatioError(nameof(toeRadius), nameof(flangeThickness));
                return null;
            }

            List<ProfileCorner> corners = TeeCorners(flangeThickness, width, webThickness, height - flangeThickness, rootRadius, toeRadius, 0);
            List<ICurve> curves = ProfileCurves(true, corners.MirrorCorners(false, mirrorAboutLocalY));
            return new TSectionProfile(height, width, webThickness, flangeThickness, rootRadius, toeRadius, mirrorAboutLocalY, curves);
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        [Description("Counter-clockwise corners of a T-shape centred on the y-axis, with the bottom of the web at y = y0 and the flange on top.")]
        private static List<ProfileCorner> TeeCorners(double tft, double tfw, double wt, double wd, double r1, double r2, double y0)
        {
            double wx = wt / 2;
            double fx = tfw / 2;
            double yf = y0 + wd;    //Underside of flange
            double yt = yf + tft;   //Top
            return new List<ProfileCorner>
            {
                new ProfileCorner(-wx, y0, 0), new ProfileCorner(wx, y0, 0), new ProfileCorner(wx, yf, r1), new ProfileCorner(fx, yf, r2),
                new ProfileCorner(fx, yt, 0), new ProfileCorner(-fx, yt, 0), new ProfileCorner(-fx, yf, r2), new ProfileCorner(-wx, yf, r1)
            };
        }

        /***************************************************/

    }
}







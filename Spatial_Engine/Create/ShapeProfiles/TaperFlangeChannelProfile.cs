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
using System;
using BH.oM.Base.Attributes;
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
        [InputFromProperty("width")]
        [InputFromProperty("webThickness")]
        [InputFromProperty("flangeThickness")]
        [InputFromProperty("flangeSlope")]
        [InputFromProperty("rootRadius")]
        [InputFromProperty("toeRadius")]
        [InputFromProperty("mirrorAboutLocalZ")]
        [Output("channel", "The created ChannelProfile.")]
        public static TaperFlangeChannelProfile TaperFlangeChannelProfile(double height, double width, double webThickness, double flangeThickness, double flangeSlope, double rootRadius = 0, double toeRadius = 0, bool mirrorAboutLocalZ = false)
        {
            if (!AreFinite(height, width, webThickness, flangeThickness, flangeSlope, rootRadius, toeRadius))
                return null;

            if (!ArePositive(height, width, webThickness, flangeThickness) || !AreNonNegative(flangeSlope, rootRadius, toeRadius))
                return null;

            if (height < 2 * ( flangeThickness + flangeSlope * (width/2 - webThickness))) // Assume no fillets
            {
                InvalidRatioError(nameof(height), $"{nameof(flangeThickness)}, {nameof(width)} and {nameof(webThickness)}");
                return null;
            }

            if (width < webThickness) // Assume no fillets
            {
                InvalidRatioError(nameof(width), nameof(webThickness));
                return null;
            }

            if (flangeSlope > flangeThickness / (width/2)) // Assume no fillets
            {
                InvalidRatioError(nameof(width), $"{nameof(flangeThickness)} and {nameof(flangeSlope)}");
                return null;
            }

            // check that the root radius doesn't take up the whole web
            if (height < 2 * (flangeThickness + flangeSlope * (width / 2 - webThickness - rootRadius) + Math.Sqrt(Math.Pow(rootRadius, 2) + Math.Pow(flangeSlope * rootRadius, 2))))
            {
                InvalidRatioError(nameof(rootRadius), $"{nameof(flangeThickness)}, {nameof(flangeSlope)}, {nameof(width)}, {nameof(webThickness)} and {nameof(height)}");
                return null;
            }

            // check that the toe radius doesn't eliminate the face of the toe
            if (Math.Sqrt(Math.Pow(toeRadius, 2) + Math.Pow(flangeSlope * toeRadius, 2)) > flangeThickness - flangeSlope * (width / 2 - toeRadius))
            {
                InvalidRatioError(nameof(toeRadius), $"{nameof(flangeThickness)}, {nameof(flangeSlope)} and {nameof(width)}");
                return null;
            }

            // check that the toe and root radii don't eliminate the inner flange face
            if (width < webThickness + (1 - flangeSlope) * (rootRadius + toeRadius))
            {
                InvalidRatioError(nameof(width), $"{nameof(webThickness)}, {nameof(rootRadius)} and {nameof(toeRadius)}");
                return null;
            }

            List<ICurve> curves = TaperFlangeChannelProfileCurves(height, width, webThickness, flangeThickness, flangeSlope, rootRadius, toeRadius, mirrorAboutLocalZ);
            return new TaperFlangeChannelProfile(height, width, webThickness, flangeThickness, flangeSlope, rootRadius, toeRadius, mirrorAboutLocalZ, curves);
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static List<ICurve> TaperFlangeChannelProfileCurves(double height, double width, double wt, double ft, double slope, double r1, double r2, bool mirrorAboutLocalZ)
        {
            //Web along the y-axis and flanges pointing in positive x, with the mid height at the origin.
            //The flange thickness is measured at the middle of the flange width.
            double y = height / 2;
            double yToe = y - (ft - width / 2 * slope);         //Inner face of flange at the toe
            double yRoot = y - (ft + (width / 2 - wt) * slope); //Inner face of flange at the web
            List<ProfileCorner> corners = new List<ProfileCorner>
            {
                new ProfileCorner(0, -y, 0), new ProfileCorner(width, -y, 0), new ProfileCorner(width, -yToe, r2), new ProfileCorner(wt, -yRoot, r1),
                new ProfileCorner(wt, yRoot, r1), new ProfileCorner(width, yToe, r2), new ProfileCorner(width, y, 0), new ProfileCorner(0, y, 0)
            };

            return ProfileCurves(true, corners.MirrorCorners(mirrorAboutLocalZ, false));
        }

        /***************************************************/

    }
}







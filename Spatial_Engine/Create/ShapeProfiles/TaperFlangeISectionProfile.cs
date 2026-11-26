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
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace BH.Engine.Spatial
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates a I-shaped profile based on input dimensions. Method generates edge curves based on the inputs.")]
        [InputFromProperty("height")]
        [InputFromProperty("width")]
        [InputFromProperty("webThickness")]
        [InputFromProperty("flangeThickness")]
        [InputFromProperty("flangeSlope")]
        [InputFromProperty("rootRadius")]
        [InputFromProperty("toeRadius")]
        [Output("I", "The created ISectionProfile.")]
        public static TaperFlangeISectionProfile TaperFlangeISectionProfile(double height, double width, double webThickness, double flangeThickness, double flangeSlope, double rootRadius, double toeRadius)
        {
            if (!AreFinite(height, width, webThickness, flangeThickness, flangeSlope, rootRadius, toeRadius))
                return null;

            if (!ArePositive(height, width, webThickness, flangeThickness) || !AreNonNegative(flangeSlope, rootRadius, toeRadius))
                return null;

            if (height < 2 * (flangeThickness + flangeSlope * (width / 4 - webThickness / 2))) // assume no fillets
            {
                InvalidRatioError(nameof(height), $"{nameof(flangeThickness)}, {nameof(flangeSlope)} and {nameof(webThickness)}");
                return null;
            }

            if (width < webThickness) // Assume no fillets
            {
                InvalidRatioError(nameof(width), nameof(webThickness));
                return null;
            }

            if (flangeSlope > flangeThickness / (width / 4)) // Assume no fillets
            {
                InvalidRatioError(nameof(width), $"{nameof(flangeThickness)} and {nameof(flangeSlope)}");
                return null;
            }

            // check that the root radius doesn't take up the whole web
            if (height < 2 * (flangeThickness + flangeSlope * (width / 4 - webThickness / 2 - rootRadius) + Math.Sqrt(Math.Pow(rootRadius, 2) + Math.Pow(flangeSlope * rootRadius, 2))))
            {
                InvalidRatioError(nameof(rootRadius), $"{nameof(flangeThickness)}, {nameof(flangeSlope)}, {nameof(width)}, {nameof(webThickness)} and {nameof(height)}");
                return null;
            }

            // check that the toe radius doesn't eliminate the face of the toe
            if (Math.Sqrt(Math.Pow(toeRadius, 2) + Math.Pow(flangeSlope * toeRadius, 2)) > flangeThickness - flangeSlope * (width / 4 - toeRadius))
            {
                InvalidRatioError(nameof(toeRadius), $"{nameof(flangeThickness)}, {nameof(flangeSlope)} and {nameof(width)}");
                return null;
            }

            // check that the toe and root radii don't eliminate the inner flange face
            if (width < webThickness + 2 * (1 - flangeSlope) * (rootRadius + toeRadius))
            {
                InvalidRatioError(nameof(width), $"{nameof(webThickness)}, {nameof(rootRadius)} and {nameof(toeRadius)}");
                return null;
            }

            List<ICurve> curves = TaperFlangeIProfileCurves(flangeThickness, width, flangeThickness, width, flangeSlope, webThickness, height, rootRadius, toeRadius);
            return new TaperFlangeISectionProfile(height, width, webThickness, flangeThickness, flangeSlope, rootRadius, toeRadius, curves);
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static List<ICurve> TaperFlangeIProfileCurves(double tft, double tfw, double bft, double bfw, double slope, double wt, double height, double r1, double r2)
        {
            //Built with the mid height at the origin. Only profiles with differing flanges need to compute the centre of area.
            //The flange thicknesses are measured at a quarter of the flange width from the tip.
            double yb = -height / 2;
            double yt = height / 2;
            double bx = bfw / 2;
            double tx = tfw / 2;
            double wx = wt / 2;
            double ybToe = yb + bft - bfw / 4 * slope;          //Inner face of bottom flange at the toe
            double ybRoot = yb + bft + (bfw / 4 - wx) * slope;  //Inner face of bottom flange at the web
            double ytToe = yt - (tft - tfw / 4 * slope);        //Inner face of top flange at the toe
            double ytRoot = yt - (tft + (tfw / 4 - wx) * slope);//Inner face of top flange at the web

            List<ProfileCorner> corners = new List<ProfileCorner>
            {
                new ProfileCorner(-bx, yb, 0), new ProfileCorner(bx, yb, 0), new ProfileCorner(bx, ybToe, r2), new ProfileCorner(wx, ybRoot, r1),
                new ProfileCorner(wx, ytRoot, r1), new ProfileCorner(tx, ytToe, r2), new ProfileCorner(tx, yt, 0), new ProfileCorner(-tx, yt, 0),
                new ProfileCorner(-tx, ytToe, r2), new ProfileCorner(-wx, ytRoot, r1), new ProfileCorner(-wx, ybRoot, r1), new ProfileCorner(-bx, ybToe, r2)
            };

            return ProfileCurves(tft != bft || tfw != bfw, corners);
        }

        /***************************************************/
    }
}





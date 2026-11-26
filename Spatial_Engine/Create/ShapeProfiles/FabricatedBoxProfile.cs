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

        [Description("Creates a rectangular hollow profile based on input dimensions. Method generates edge curves based on the inputs.")]
        [InputFromProperty("height")]
        [InputFromProperty("width")]
        [InputFromProperty("webThickness")]
        [InputFromProperty("topFlangeThickness")]
        [InputFromProperty("botFlangeThickness")]
        [InputFromProperty("weldSize")]
        [Output("fabBox", "The created BoxProfile.")]
        public static FabricatedBoxProfile FabricatedBoxProfile(double height, double width, double webThickness, double topFlangeThickness, double botFlangeThickness, double weldSize = 0)
        {
            if (!AreFinite(height, width, webThickness, topFlangeThickness, botFlangeThickness, weldSize))
                return null;

            if (!ArePositive(height, width, webThickness, topFlangeThickness, botFlangeThickness) || !AreNonNegative(weldSize))
                return null;

            if (height < topFlangeThickness + botFlangeThickness + 2 * Math.Sqrt(2) * weldSize || height <= topFlangeThickness + botFlangeThickness)
            {
                InvalidRatioError(nameof(height), $"{nameof(topFlangeThickness)}, {nameof(botFlangeThickness)} and {nameof(weldSize)}");
                return null;
            }

            if (width < webThickness * 2 + 2 * Math.Sqrt(2) * weldSize || width <= webThickness * 2)
            {
                InvalidRatioError(nameof(width), $"{nameof(webThickness)} and {nameof(weldSize)}");
                return null;
            }

            List<ICurve> curves = FabricatedBoxProfileCurves(width, height, webThickness, topFlangeThickness, botFlangeThickness, weldSize);
            return new FabricatedBoxProfile(height, width, webThickness, topFlangeThickness, botFlangeThickness, weldSize, curves);
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static List<ICurve> FabricatedBoxProfileCurves(double width, double height, double webThickness, double topFlangeThickness, double botFlangeThickness, double weldSize)
        {
            //Inner edges of webs and flanges, relative to the centre of the outer rectangle
            double x = width / 2 - webThickness;
            double yt = height / 2 - topFlangeThickness;
            double yb = -height / 2 + botFlangeThickness;

            //Inner loop clockwise, with the welds as chamfers in the corners
            List<ProfileCorner> inner;
            if (weldSize > 0)
            {
                double wl = weldSize * Math.Sqrt(2);
                inner = new List<ProfileCorner>
                {
                    new ProfileCorner(-x, yb + wl, 0), new ProfileCorner(-x, yt - wl, 0), new ProfileCorner(-x + wl, yt, 0), new ProfileCorner(x - wl, yt, 0),
                    new ProfileCorner(x, yt - wl, 0), new ProfileCorner(x, yb + wl, 0), new ProfileCorner(x - wl, yb, 0), new ProfileCorner(-x + wl, yb, 0)
                };
            }
            else
                inner = new List<ProfileCorner> { new ProfileCorner(-x, yb, 0), new ProfileCorner(-x, yt, 0), new ProfileCorner(x, yt, 0), new ProfileCorner(x, yb, 0) };

            //Only differing flanges move the centre of area away from the centre of the outer rectangle
            return ProfileCurves(topFlangeThickness != botFlangeThickness, RectangleCorners(width, height, 0), inner);
        }

        /***************************************************/

    }
}







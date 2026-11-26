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
using BH.oM.Quantities.Attributes;
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

        [Description("Creates a rectangular hollow profile with outstands based on input dimensions. Method generates edge curves based on the inputs.")]
        [InputFromProperty("height")]
        [InputFromProperty("width")]
        [InputFromProperty("webThickness")]
        [InputFromProperty("topFlangeThickness")]
        [InputFromProperty("botFlangeThickness")]
        [Input("topCorbelWidth", "Width of the outstands of each side at the top of the profile.", typeof(Length))]
        [Input("botCorbelWidth", "Width of the outstands of each side at the bottom of the profile.", typeof(Length))]
        [Output("fabBox", "The created GeneralisedFabricatedBoxProfile.")]
        public static GeneralisedFabricatedBoxProfile GeneralisedFabricatedBoxProfile(double height, double width, double webThickness, double topFlangeThickness = 0.0, double botFlangeThickness = 0.0, double topCorbelWidth = 0.0, double botCorbelWidth = 0.0)
        {
            if (!AreFinite(height, width, webThickness, topFlangeThickness, botFlangeThickness, topCorbelWidth, botCorbelWidth))
                return null;

            if (!ArePositive(height, width, webThickness, topFlangeThickness, botFlangeThickness) || !AreNonNegative(topCorbelWidth, botCorbelWidth))
                return null;

            if (webThickness >= width / 2)
            {
                InvalidRatioError(nameof(webThickness), nameof(width));
                return null;
            }

            if (height <= topFlangeThickness + botFlangeThickness)
            {
                InvalidRatioError(nameof(height), $"{nameof(topFlangeThickness)} and {nameof(botFlangeThickness)}");
                return null;
            }

            List<ICurve> curves = GeneralisedFabricatedBoxProfileCurves(height, width, webThickness, topFlangeThickness, botFlangeThickness, topCorbelWidth, topCorbelWidth, botCorbelWidth, botCorbelWidth);
            return new GeneralisedFabricatedBoxProfile(height, width, webThickness, topFlangeThickness, botFlangeThickness, topCorbelWidth, topCorbelWidth, botCorbelWidth, botCorbelWidth, curves);
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static List<ICurve> GeneralisedFabricatedBoxProfileCurves(double height, double width, double webThickness, double topFlangeThickness, double botFlangeThickness, double topLeftCorbelWidth, double topRightCorbelWidth, double botLeftCorbelWidth, double botRightCorbelWidth)
        {
            //Box centred on the y-axis with its bottom at the origin
            double x = width / 2;
            double xi = x - webThickness;
            double yb = botFlangeThickness;
            double yt = height - topFlangeThickness;

            //Outer loop counter-clockwise, starting at the bottom left. Corners of corbels of zero width are left out, as they would coincide with the corners of the box.
            List<ProfileCorner> outer = new List<ProfileCorner>(12);
            outer.Add(new ProfileCorner(-x - botLeftCorbelWidth, 0));
            outer.Add(new ProfileCorner(x + botRightCorbelWidth, 0));
            if (botRightCorbelWidth > 0)
            {
                outer.Add(new ProfileCorner(x + botRightCorbelWidth, yb));
                outer.Add(new ProfileCorner(x, yb));
            }
            if (topRightCorbelWidth > 0)
            {
                outer.Add(new ProfileCorner(x, yt));
                outer.Add(new ProfileCorner(x + topRightCorbelWidth, yt));
            }
            outer.Add(new ProfileCorner(x + topRightCorbelWidth, height));
            outer.Add(new ProfileCorner(-x - topLeftCorbelWidth, height));
            if (topLeftCorbelWidth > 0)
            {
                outer.Add(new ProfileCorner(-x - topLeftCorbelWidth, yt));
                outer.Add(new ProfileCorner(-x, yt));
            }
            if (botLeftCorbelWidth > 0)
            {
                outer.Add(new ProfileCorner(-x, yb));
                outer.Add(new ProfileCorner(-x - botLeftCorbelWidth, yb));
            }

            //Inner loop clockwise
            List<ProfileCorner> inner = new List<ProfileCorner> { new ProfileCorner(-xi, yb, 0), new ProfileCorner(-xi, yt, 0), new ProfileCorner(xi, yt, 0), new ProfileCorner(xi, yb, 0) };

            return ProfileCurves(true, outer, inner);
        }

        /***************************************************/

    }
}







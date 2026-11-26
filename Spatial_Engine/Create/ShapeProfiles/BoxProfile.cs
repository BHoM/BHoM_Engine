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
        [InputFromProperty("thickness")]
        [InputFromProperty("outerRadius")]
        [InputFromProperty("innerRadius")]
        [Output("box", "The created BoxProfile.")]
        public static BoxProfile BoxProfile(double height, double width, double thickness, double outerRadius = 0, double innerRadius = 0)
        {
            if (!AreFinite(height, width, thickness, outerRadius, innerRadius))
                return null;

            if (!ArePositive(height, width, thickness) || !AreNonNegative(outerRadius, innerRadius))
                return null;

            if (thickness >= height / 2)
            {
                InvalidRatioError(nameof(thickness), nameof(height));
                return null;
            }

            if (thickness >= width / 2)
            {
                InvalidRatioError(nameof(thickness), nameof(width));
                return null;
            }

            if (outerRadius > height / 2)
            {
                InvalidRatioError(nameof(outerRadius), nameof(height));
                return null;
            }

            if (outerRadius > width / 2)
            {
                InvalidRatioError(nameof(outerRadius), nameof(width));
                return null;
            }

            if (innerRadius * 2 > width - thickness * 2)
            {
                InvalidRatioError(nameof(innerRadius), $"{nameof(width)} and {nameof(thickness)}");
                return null;
            }

            if (innerRadius * 2 > height - thickness * 2)
            {
                InvalidRatioError(nameof(innerRadius), $"{nameof(height)} and {nameof(thickness)}");
                return null;
            }

            if (Math.Sqrt(2) * thickness <= Math.Sqrt(2) * outerRadius - outerRadius - Math.Sqrt(2) * innerRadius + innerRadius)
            {
                InvalidRatioError(nameof(thickness), $"{nameof(outerRadius)} and {nameof(innerRadius)}");
                return null;
            }

            List<ICurve> curves = BoxProfileCurves(width, height, thickness, thickness, innerRadius, outerRadius);
            return new BoxProfile(height, width, thickness, outerRadius, innerRadius, curves);
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static List<ICurve> BoxProfileCurves(double width, double height, double webThickness, double flangeThickness, double innerRadius, double outerRadius)
        {
            //Outer loop counter-clockwise, inner loop clockwise
            return ProfileCurves(false, RectangleCorners(width, height, outerRadius), RectangleCorners(width - 2 * webThickness, height - 2 * flangeThickness, innerRadius, true));
        }

        /***************************************************/
    }
}







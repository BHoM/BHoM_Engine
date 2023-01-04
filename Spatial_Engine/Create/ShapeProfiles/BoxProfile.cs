/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2023, the respective contributors. All rights reserved.
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
using System.Linq;
using System.Collections.ObjectModel;
using BH.oM.Spatial.ShapeProfiles;
using BH.oM.Geometry;
using System;
using BH.Engine.Reflection;
using BH.oM.Base.Attributes;
using BH.Engine.Geometry;
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
            if (thickness >= height / 2)
            {
                InvalidRatioError("thickness", "height");
                return null;
            }

            if (thickness >= width / 2)
            {
                InvalidRatioError("thickness", "width");
                return null;
            }

            if (outerRadius > height / 2)
            {
                InvalidRatioError("outerRadius", "height");
                return null;
            }

            if (outerRadius > width / 2)
            {
                InvalidRatioError("outerRadius", "width");
                return null;
            }

            if (innerRadius * 2 > width - thickness * 2)
            {
                InvalidRatioError("innerRadius", "width and thickness");
                return null;
            }

            if (innerRadius * 2 > height - thickness * 2)
            {
                InvalidRatioError("innerRadius", "height and thickness");
                return null;
            }

            if (Math.Sqrt(2) * thickness <= Math.Sqrt(2) * outerRadius - outerRadius - Math.Sqrt(2) * innerRadius + innerRadius)
            {
                InvalidRatioError("thickness", "outerRadius and innerRadius");
                return null;
            }

            if (height <= 0 || width <= 0 || thickness <= 0 || outerRadius < 0 || innerRadius < 0)
            {
                Engine.Base.Compute.RecordError("Input length less or equal to 0");
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
            List<ICurve> box = RectangleProfileCurves(width, height, outerRadius);
            box.AddRange(RectangleProfileCurves(width - 2 * webThickness, height - 2 * flangeThickness, innerRadius));
            return box;
        }

        /***************************************************/
    }
}




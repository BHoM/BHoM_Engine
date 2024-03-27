/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2024, the respective contributors. All rights reserved.
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

        [Description("Creates a rectangular solid profile based on input dimensions. Method generates edge curves based on the inputs.")]
        [InputFromProperty("height")]
        [InputFromProperty("width")]
        [InputFromProperty("cornerRadius")]
        [Output("rectangle", "The created RectangleProfile.")]
        public static RectangleProfile RectangleProfile(double height, double width, double cornerRadius = 0)
        {
            if (cornerRadius > height / 2)
            {
                InvalidRatioError("cornerRadius", "height");
                return null;
            }

            if (cornerRadius > width / 2)
            {
                InvalidRatioError("cornerRadius", "width");
                return null;
            }

            if (height <= 0 || width <= 0 || cornerRadius < 0)
            {
                Engine.Base.Compute.RecordError("Input length less or equal to 0");
                return null;
            }
            List<ICurve> curves = RectangleProfileCurves(width, height, cornerRadius);
            return new RectangleProfile(height, width, cornerRadius, curves);
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static List<ICurve> RectangleProfileCurves(double width, double height, double radius)
        {
            Vector xAxis = oM.Geometry.Vector.XAxis;
            Vector yAxis = oM.Geometry.Vector.YAxis;
            Point origin = oM.Geometry.Point.Origin;

            List<ICurve> perimeter = new List<ICurve>();
            Point p = new Point { X = -width / 2, Y = height / 2 - radius, Z = 0 };
            perimeter.Add(new Line { Start = p, End = p = p - yAxis * (height - 2 * radius) });
            if (radius > 0) perimeter.Add(BH.Engine.Geometry.Create.ArcByCentre(p + xAxis * radius, p, p = p + new Vector { X = radius, Y = -radius, Z = 0 }));
            perimeter.Add(new Line { Start = p, End = p = p + xAxis * (width - 2 * radius) });
            if (radius > 0) perimeter.Add(BH.Engine.Geometry.Create.ArcByCentre(p + yAxis * radius, p, p = p + new Vector { X = radius, Y = radius, Z = 0 }));
            perimeter.Add(new Line { Start = p, End = p = p + yAxis * (height - 2 * radius) });
            if (radius > 0) perimeter.Add(BH.Engine.Geometry.Create.ArcByCentre(p - xAxis * radius, p, p = p + new Vector { X = -radius, Y = radius, Z = 0 }));
            perimeter.Add(new Line { Start = p, End = p = p - xAxis * (width - 2 * radius) });
            if (radius > 0) perimeter.Add(BH.Engine.Geometry.Create.ArcByCentre(p - yAxis * radius, p, p = p + new Vector { X = -radius, Y = -radius, Z = 0 }));
            return perimeter;
        }

        /***************************************************/
    }
}





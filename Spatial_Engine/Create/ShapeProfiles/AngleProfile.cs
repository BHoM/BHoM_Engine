/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2025, the respective contributors. All rights reserved.
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
            if (height < flangeThickness + rootRadius + toeRadius)
            {
                InvalidRatioError("height", "flangeThickness, rootRadius and toeRadius");
                return null;
            }

            if (width < webThickness + rootRadius + toeRadius)
            {
                InvalidRatioError("width", "webthickness, rootRadius and toeRadius");
                return null;
            }

            if (flangeThickness < toeRadius)
            {
                InvalidRatioError("flangeThickness", "toeRadius");
                return null;
            }

            if (webThickness < toeRadius)
            {
                InvalidRatioError("webthickness", "toeRadius");
                return null;
            }

            if (height <= 0 || width <= 0 || webThickness <= 0 || flangeThickness <= 0 || rootRadius < 0 || toeRadius < 0)
            {
                Engine.Base.Compute.RecordError("Input length less or equal to 0");
                return null;
            }

            List<ICurve> curves = AngleProfileCurves(width, height, flangeThickness, webThickness, rootRadius, toeRadius);

            if (mirrorAboutLocalZ)
                curves = curves.MirrorAboutLocalZ();
            if (mirrorAboutLocalY)
                curves = curves.MirrorAboutLocalY();

            return new AngleProfile(height, width, webThickness, flangeThickness, rootRadius, toeRadius, mirrorAboutLocalZ, mirrorAboutLocalY, curves);
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static List<ICurve> AngleProfileCurves(double width, double depth, double flangeThickness, double webThickness, double innerRadius, double toeRadius)
        {
            List<ICurve> perimeter = new List<ICurve>();

            Vector xAxis = oM.Geometry.Vector.XAxis;
            Vector yAxis = oM.Geometry.Vector.YAxis;
            Point origin = oM.Geometry.Point.Origin;

            Point p = new Point { X = 0, Y = 0, Z = 0 };
            perimeter.Add(new Line { Start = p, End = p = p + xAxis * (width) });
            perimeter.Add(new Line { Start = p, End = p = p + yAxis * (flangeThickness - toeRadius) });
            if (toeRadius > 0) perimeter.Add(BH.Engine.Geometry.Create.ArcByCentre(p - xAxis * (toeRadius), p, p = p + new Vector { X = -toeRadius, Y = toeRadius, Z = 0 }));
            perimeter.Add(new Line { Start = p, End = p = p - xAxis * (width - webThickness - innerRadius - toeRadius) });
            if (innerRadius > 0) perimeter.Add(BH.Engine.Geometry.Create.ArcByCentre(p + yAxis * (innerRadius), p, p = p + new Vector { X = -innerRadius, Y = innerRadius, Z = 0 }));
            perimeter.Add(new Line { Start = p, End = p = p + yAxis * (depth - flangeThickness - innerRadius - toeRadius) });
            if (toeRadius > 0) perimeter.Add(BH.Engine.Geometry.Create.ArcByCentre(p - xAxis * (toeRadius), p, p = p + new Vector { X = -toeRadius, Y = toeRadius, Z = 0 }));
            perimeter.Add(new Line { Start = p, End = p = p - xAxis * (webThickness - toeRadius) });
            perimeter.Add(new Line { Start = p, End = p = p - yAxis * (depth) });

            Point centroid = perimeter.IJoin().Centroid();
            Vector translation = Point.Origin - centroid;
            return perimeter.Select(x => x.ITranslate(translation)).ToList();
        }

        /***************************************************/
    }
}






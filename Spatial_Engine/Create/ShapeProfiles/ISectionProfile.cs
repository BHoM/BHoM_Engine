﻿/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2020, the respective contributors. All rights reserved.
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
using BH.oM.Reflection.Attributes;
using BH.Engine.Geometry;
using System.ComponentModel;

namespace BH.Engine.Spatial
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [PreviousVersion("4.0", "BH.Engine.Geometry.Create.ISectionProfile(System.Double, System.Double, System.Double, System.Double, System.Double, System.Double)")]
        [PreviousVersion("4.0", "BH.Engine.Structure.Create.ISectionProfile(System.Double, System.Double, System.Double, System.Double, System.Double, System.Double)")]
        [Description("Creates a I-shaped profile based on input dimensions. Method generates edge curves based on the inputs.")]
        [InputFromProperty("height")]
        [InputFromProperty("width")]
        [InputFromProperty("webThickness")]
        [InputFromProperty("flangeThickness")]
        [InputFromProperty("rootRadius")]
        [InputFromProperty("toeRadius")]
        [Output("I", "The created ISectionProfile.")]
        public static ISectionProfile ISectionProfile(double height, double width, double webThickness, double flangeThickness, double rootRadius, double toeRadius)
        {
            if (height < flangeThickness * 2 + rootRadius * 2 || height <= flangeThickness * 2)
            {
                InvalidRatioError("height", "flangeThickness and rootRadius");
                return null;
            }

            if (width < webThickness + rootRadius * 2 + toeRadius * 2)
            {
                InvalidRatioError("width", "webthickness, rootRadius and toeRadius");
                return null;
            }

            if (toeRadius > flangeThickness)
            {
                InvalidRatioError("toeRadius", "flangeThickness");
                return null;
            }

            if (height <= 0 || width <= 0 || webThickness <= 0 || flangeThickness <= 0 || rootRadius < 0 || toeRadius < 0)
            {
                Engine.Reflection.Compute.RecordError("Input length less or equal to 0");
                return null;
            }

            List<ICurve> curves = IProfileCurves(flangeThickness, width, flangeThickness, width, webThickness, height - 2 * flangeThickness, rootRadius, toeRadius, 0);
            return new ISectionProfile(height, width, webThickness, flangeThickness, rootRadius, toeRadius, curves);
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static List<ICurve> IProfileCurves(double tft, double tfw, double bft, double bfw, double wt, double wd, double r1, double r2, double weldSize)
        {
            List<ICurve> perimeter = new List<ICurve>();
            Point p = new Point { X = bfw / 2, Y = 0, Z = 0 };

            Vector xAxis = oM.Geometry.Vector.XAxis;
            Vector yAxis = oM.Geometry.Vector.YAxis;
            Point origin = oM.Geometry.Point.Origin;
            double weldLength = weldSize * 2 / Math.Sqrt(2);

            perimeter.Add(new Line { Start = p, End = p = p + yAxis * (bft - r2) });
            if (r2 > 0) perimeter.Add(BH.Engine.Geometry.Create.ArcByCentre(p - xAxis * r2, p, p = p + new Vector { X = -r2, Y = r2, Z = 0 }));
            perimeter.Add(new Line { Start = p, End = p = p - xAxis * (bfw / 2 - wt / 2 - r1 - r2 - weldLength) });
            if (r1 > 0) perimeter.Add(BH.Engine.Geometry.Create.ArcByCentre(p + yAxis * r1, p, p = p + new Vector { X = -r1, Y = r1, Z = 0 }));
            if (weldSize > 0) perimeter.Add(new Line { Start = p, End = p = p + new Vector { X = -weldLength, Y = weldLength, Z = 0 } });
            perimeter.Add(new Line { Start = p, End = p = p + yAxis * (wd - 2 * r1 - 2 * weldLength) });
            if (weldSize > 0) perimeter.Add(new Line { Start = p, End = p = p + new Vector { X = weldLength, Y = weldLength, Z = 0 } });
            if (r1 > 0) perimeter.Add(BH.Engine.Geometry.Create.ArcByCentre(p + xAxis * r1, p, p = p + new Vector { X = r1, Y = r1, Z = 0 }));
            perimeter.Add(new Line { Start = p, End = p = p + xAxis * (tfw / 2 - wt / 2 - r1 - r2 - weldLength) });
            if (r2 > 0) perimeter.Add(BH.Engine.Geometry.Create.ArcByCentre(p + yAxis * r2, p, p = p + new Vector { X = r2, Y = r2, Z = 0 }));
            perimeter.Add(new Line { Start = p, End = p = p + yAxis * (tft - r2) });

            int count = perimeter.Count;
            for (int i = 0; i < count; i++)
            {
                perimeter.Add(perimeter[i].IMirror(new Plane { Origin = origin, Normal = xAxis }));
            }
            perimeter.Add(new Line { Start = p, End = p - xAxis * (tfw) });
            perimeter.Add(new Line { Start = origin + xAxis * (-bfw / 2), End = origin + xAxis * (bfw / 2) });

            return perimeter;
        }

        /***************************************************/

    }
}

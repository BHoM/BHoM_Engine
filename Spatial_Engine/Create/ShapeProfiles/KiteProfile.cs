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

        public static KiteProfile KiteProfile(double width1, double angle1, double thickness)
        {
            if ((width1 * Math.Sin(angle1 / 2) / Math.Sqrt(2)) / (Math.Sin(Math.PI * 0.75 - (angle1 / 2))) <= thickness)
            {
                InvalidRatioError("thickness", "width and angle1");
                return null;
            }

            if (width1 <= 0 || angle1 <= 0 || thickness <= 0)
            {
                Engine.Base.Compute.RecordError("Input length less or equal to 0");
                return null;
            }

            List<ICurve> curves = KiteProfileCurves(width1, angle1, thickness);
            return new KiteProfile(width1, angle1, thickness, curves);
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static List<ICurve> KiteProfileCurves(double width1, double angle1, double thickness)
        {
            Vector xAxis = oM.Geometry.Vector.XAxis;
            Vector yAxis = oM.Geometry.Vector.YAxis;
            Vector zAxis = oM.Geometry.Vector.ZAxis;
            Point origin = oM.Geometry.Point.Origin;

            List<ICurve> externalEdges = new List<ICurve>();
            List<ICurve> internalEdges = new List<ICurve>();
            List<ICurve> group = new List<ICurve>();

            double width2 = width1 * Math.Tan(angle1 / 2);
            double angle2 = Math.PI - angle1;

            double tolerance = 1e-3;

            if (angle2 < tolerance || angle2 > Math.PI - tolerance)
            {
                Base.Compute.RecordError("Angles must be well between 0 and Pi");
                return null;
            }

            Point p1 = new Point { X = 0, Y = 0, Z = 0 };
            Point p2 = p1 + xAxis * Math.Abs(thickness / Math.Sin(angle1 / 2));

            Vector dirVec1 = xAxis.Rotate(angle1 / 2, zAxis);
            Vector dirVec2 = dirVec1.Rotate(-(Math.PI / 2), zAxis);

            externalEdges.Add(new Line { Start = p1, End = p1 = p1 + dirVec1 * width1 });
            externalEdges.Add(new Line { Start = p1, End = p1 + dirVec2 * (width1 * Math.Tan(angle1 / 2)) });

            int extCount = externalEdges.Count;
            for (int i = 0; i < extCount; i++)
            {
                externalEdges.Add(externalEdges[i].IMirror(new Plane { Origin = origin, Normal = yAxis }));
            }

            internalEdges.Add(new Line { Start = p2, End = p2 = p2 + dirVec1 * (width1 - thickness - (thickness * Math.Cos(angle1 / 2)) / Math.Sin(angle1 / 2)) });
            internalEdges.Add(new Line { Start = p2, End = p2 + dirVec2 * (width2 - thickness - (thickness * Math.Cos(angle2 / 2)) / Math.Sin(angle2 / 2)) });

            int intCount = internalEdges.Count;
            for (int i = 0; i < intCount; i++)
            {
                internalEdges.Add(internalEdges[i].IMirror(new Plane { Origin = origin, Normal = yAxis }));
            }

            Point centroid = externalEdges.IJoin().Centroid(internalEdges.IJoin());
            Vector translation = Point.Origin - centroid;

            group.AddRange(externalEdges);
            group.AddRange(internalEdges);

            return group.Select(x => x.ITranslate(translation)).ToList();
        }

        /***************************************************/
    }
}




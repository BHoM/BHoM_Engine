/*
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

using BH.oM.Geometry;
using BH.oM.Structure.Elements;
using BH.oM.Structure.Loads;
using BH.Engine.Geometry;

using System.Collections.Generic;
using System.Linq;
using System;

namespace BH.Engine.Structure
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<Point> PointGrid(this Panel panel)
        {
            List<ICurve> curves = panel.ExternalEdgeCurves();

            List<PolyCurve> joined = BH.Engine.Geometry.Compute.IJoin(curves);
            List<PolyCurve> joinedOpeningCurves = BH.Engine.Geometry.Compute.IJoin(panel.InternalEdgeCurves());

            Plane plane = joined.First().FitPlane();

            Vector z = Vector.ZAxis;

            double angle = plane.Normal.Angle(z);

            Vector axis = plane.Normal.CrossProduct(z);

            TransformMatrix matrix = Engine.Geometry.Create.RotationMatrix(Point.Origin, axis, angle);

            List<PolyCurve> rotated = BH.Engine.Geometry.Compute.IJoin(curves.Select(x => x.IRotate(Point.Origin, axis,angle)).ToList());

            BoundingBox bounds = rotated.First().Bounds();

            for (int i = 1; i < rotated.Count; i++)
            { 
                bounds += rotated[i].Bounds();
            }

            double xMin = bounds.Min.X;
            double yMin = bounds.Min.Y;
            double zVal = bounds.Min.Z;

            int steps = 9;

            double xStep = (bounds.Max.X - xMin) / steps;
            double yStep = (bounds.Max.Y - yMin) / steps;

            List<Point> pts = new List<Point>();
            TransformMatrix transpose = matrix.Transpose();

            for (int i = 0; i < steps; i++)
            {
                double x = xMin + xStep * i;
                for (int j = 0; j < steps; j++)
                {
                    Point pt = new Point { X = x, Y = yMin + yStep * j, Z = zVal };
                    bool isInside = false;

                    pt = pt.Transform(transpose);

                    foreach (PolyCurve crv in joined)
                    {
                        List<Point> list = new List<Point> { pt };
                        if (crv.IsContaining(list, true, 1E-3))
                        {
                            if (!joinedOpeningCurves.Any(c => c.IsContaining(list, false)))
                            {
                                isInside = true;
                                break;
                            }
                        }

                    }
                    if (isInside)
                        pts.Add(pt);
                }
            }

            return pts;
        }

        /***************************************************/
        /**** Public Methods Interface                  ****/
        /***************************************************/

        public static List<Point> IPointGrid(this IAreaElement element)
        {
            return PointGrid(element as dynamic);
        }

        /***************************************************/
        /**** Private methods                           ****/
        /***************************************************/

        private static bool SimpleIsContaining(PolyCurve crv, Plane plane, Point pt)
        {
            Point end = pt.Translate(Engine.Geometry.Create.RandomVectorInPlane(plane, true));
            Line ray = new Line { Start = pt, End = end };
            ray.Infinite = true;

            List<Point> interPts = crv.Curves.SelectMany(x => x.ILineIntersections(ray, true)).ToList();

            return interPts.Count % 2 != 0;
        }

        /***************************************************/
    }

}


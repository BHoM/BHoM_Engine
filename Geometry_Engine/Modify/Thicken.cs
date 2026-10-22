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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Thicken a line to create a closed polyline with the same direction and a custom thickness.")]
        [Input("line", "A line to convert into a closed polyline via thickening.")]
        [Input("thickness", "A custom thickness for the new closed polyline.")]
        [Input("plane", "Plane that defines the thicken direction via its normal.")]
        [Output("polyline", "A closed polyline with the same direction as the input line and a custom thickness.")]
        public static Polyline Thicken(this Line line, double thickness, Plane plane)
        {
            Vector chordDirection = (line.End - line.Start).Normalise();
            if (chordDirection.IsParallel(plane.Normal) != 0)
            {
                Base.Compute.RecordError("Can't thicken a line parallel to the plane normal.");
                return null;
            }

            Vector offsetVector = chordDirection.CrossProduct(plane.Normal).Normalise() * thickness / 2;

            Point p1 = line.Start + offsetVector;
            Point p2 = line.End + offsetVector;
            Point p3 = line.End - offsetVector;
            Point p4 = line.Start - offsetVector;

            return new Polyline { ControlPoints = new List<Point> { p1, p2, p3, p4, p1 } };
        }

        /***************************************************/

        [Description("Thicken a curve to create a closed polyline with a custom thickness.")]
        [Input("crv", "A curve to convert into a closed polyline via thickening.")]
        [Input("thickness", "A custom thickness for the new closed polyline.")]
        [Input("plane", "Plane that defines the thicken direction via its normal.")]
        [Output("polyline", "A closed polyline thickened from the input curve.")]
        public static Polyline Thicken(this ICurve crv, double thickness, Plane plane)
        {
            List<Point> controlPoints = crv.IControlPoints();
            Vector chordDirection = (controlPoints.Last() - controlPoints.First()).Normalise();
            if (chordDirection.IsParallel(plane.Normal) != 0)
            {
                Base.Compute.RecordError("Can't thicken a curve parallel to the plane normal.");
                return null;
            }

            if (crv is Line line)
                return line.Thicken(thickness, plane);

            if (crv is NurbsCurve)
            {
                Polyline pLine = new Polyline { ControlPoints = crv.IControlPoints().CullDuplicates() };
                return pLine.Thicken(thickness, plane);
            }

            ICurve c1 = crv.IOffset(thickness, plane.Normal, true);
            ICurve c2 = crv.IOffset(thickness, -plane.Normal, true);
            List<Point> points2 = c2.IControlPoints();
            points2.Reverse();

            List<Point> points = c1.IControlPoints().Concat(points2).ToList();
            return new Polyline { ControlPoints = points }.Close();
        }

        /***************************************************/
    }
}

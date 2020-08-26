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

using BH.oM.Analytical.Elements;
using BH.oM.Geometry;
using BH.oM.Reflection.Attributes;
using BH.Engine.Geometry;
using BH.Engine.Reflection;

using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;

namespace BH.Engine.Analytical
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Determines whether a panel is a rectangle")]
        [Input("panel", "The Panel to check if it is a rectangle")]
        [Output("bool", "True for rectangular panels or false for non-rectangular panels")]
        public static bool IsOutlineRectangular<TEdge, TOpening>(this IPanel<TEdge, TOpening> panel)
            where TEdge : IEdge
            where TOpening : IOpening<TEdge>
        {
            List<ICurve> curves = panel.ExternalEdges.SelectMany(x => x.Curve.ISubParts()).ToList();

            List<PolyCurve> polycurves = Engine.Geometry.Compute.IJoin(curves);

            //Check there is a single continuous curve defining the Panel
            if (polycurves.Count != 1)
                return false;

            PolyCurve polycurve = polycurves.First();

            //Check that all subparts of the curve are linear
            if (polycurve.SubParts().Any(x => !x.IIsLinear()))
                return false;

            //Group curves by direction vector to obtain discontinuity points
            List<Point> points = new List<Point>();
            var groupedCurves = curves.GroupBy(x => x.IStartDir());
            foreach (var groupedCurve in groupedCurves)
            {
                ICurve jointCurve = Engine.Geometry.Compute.IJoin(groupedCurve.Select(x => x).ToList()).First();
                points.Add(jointCurve.IStartPoint());
            }

            //Check there are four discontinuity points present
            if (points.Count != 4)
                return false;

            //Create vectors for all four sides of the quadilateral
            List<Vector> vectors = new List<Vector>();
            for (int i = 0; i < 3; i++)
            {
                vectors.Add(points[i + 1] - points[i]);
            }
            vectors.Add(points[0] - points[3]);

            //Get the angles in the panel, only three are needed
            List<double> angles = new List<double>() { vectors[3].Angle(vectors[0]) };
            for (int i = 0; i < 3; i++)
            {
                angles.Add(vectors[i].Angle(vectors[i + 1]));
            }

            //Check the three angles are pi/2 degrees within tolerance
            if (angles.Any(x => Math.Abs(Math.PI / 2 - x) > Tolerance.Angle))
                return false;

            //Check if all sides are the same length
            double length = vectors.First().Length();
            if (vectors.Skip(0).All(x => (Math.Abs(x.Length() - length) < Tolerance.Distance)))
                return false;

            //Check opposing sides are of equal length
            return Math.Abs(vectors[0].Length() - vectors[2].Length()) < Tolerance.Distance && Math.Abs(vectors[1].Length() - vectors[3].Length()) < Tolerance.Distance ? true : false;
        }

        /***************************************************/

    }

}
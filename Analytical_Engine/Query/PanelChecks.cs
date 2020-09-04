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
using System.Linq.Expressions;

namespace BH.Engine.Analytical
{
    public static partial class Query
    {
        /***************************************************/
        /**** Private Methods                            ****/
        /***************************************************/

        [Description("Gets the discrete discontinuity points from a Polycurve and checks four points are present")]
        [Input("polycurve", "The Polycurve to get discrete discontinuity points from")]
        [Output("points", "The discrete discontinuity points of the Polycurve")]
        private static List<Point> GetPoints(this PolyCurve polycurve, out bool isCheck)
        {
            isCheck = true;

            //Group curves by direction vector to obtain discontinuity points
            List<Point> points = new List<Point>();
            var groupedCurves = polycurve.SubParts().GroupBy(x => x.IStartDir());
            foreach (var groupedCurve in groupedCurves)
            {
                ICurve jointCurve = Engine.Geometry.Compute.IJoin(groupedCurve.Select(x => x).ToList()).First();
                points.Add(jointCurve.IStartPoint());
            }

            //Check there are four discontinuity points present 
            if (points.Count != 4)
                isCheck = false;

            return points;
        }

        /***************************************************/

        [Description("Gets the vectors from the provided list of pints")]
        [Input("points", "The list of points")]
        [Output("vectors", "The vectors computed from the list of points")]
        private static List<Vector> GetVectors(this List<Point> points)
        {
            //Create vectors for all four sides of the quadilateral
            List<Vector> vectors = new List<Vector>();
            for (int i = 0; i < 3; i++)
            {
                vectors.Add(points[i + 1] - points[i]);
            }
            vectors.Add(points[0] - points[3]);

            return vectors;
        }

        /***************************************************/

        [Description("Gets the internal angle between sequential vectors")]
        [Input("vectors", "The vectors to find the internal angle between")]
        [Output("angles", "The internal angle between sequential vectors")]
        private static List<double> GetAngles(this List<Vector> vectors)
        {
            //Get the angles in the panel, only three are needed
            List<double> angles = new List<double>() { vectors[3].Angle(vectors[0]) };
            for (int i = 0; i < 3; i++)
            {
                angles.Add(vectors[i].Angle(vectors[i + 1]));
            }

            return angles;
        }

        /***************************************************/

    }

}
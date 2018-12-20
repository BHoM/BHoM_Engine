/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2018, the respective contributors. All rights reserved.
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine_Explore.BHoM.Geometry
{

    // Test NURBS curve using array of double instead of list of Point
    public class NurbCurveB : BHoMGeometry, ICurve
    {
        /***************************************************/
        /**** Properties                                ****/
        /***************************************************/

        public double[] ControlPoints { get; set; } = new double[0];

        public List<double> Weights { get; set; } = new List<double>();

        public List<double> Knots { get; set; } = new List<double>();


        /***************************************************/
        /**** Constructors                              ****/
        /***************************************************/

        public NurbCurveB() { }

        /***************************************************/

        public NurbCurveB(IEnumerable<Point> controlPoints, int degree = 3)
        {
            int n = controlPoints.Count();
            int d = degree - 1;
            List<Point> points = controlPoints.ToList();

            ControlPoints = new double[n*3];
            for (int i = 0; i < n; i++)
            {
                ControlPoints[3 * i] = points[i].X;
                ControlPoints[3 * i + 1] = points[i].Y;
                ControlPoints[3 * i + 2] = points[i].Z;
            }

            Weights = Enumerable.Repeat(1.0, n).ToList();
            Knots = Enumerable.Repeat(0, d).Concat(Enumerable.Range(0, n - d).Concat(Enumerable.Repeat(n - d - 1, d))).Select(x => (double)x).ToList();
        }

        /***************************************************/

        public NurbCurveB(IEnumerable<Point> controlPoints, IEnumerable<double> weights, int degree = 3)
        {
            int n = controlPoints.Count();
            int d = degree - 1;
            List<Point> points = controlPoints.ToList();

            ControlPoints = new double[n * 3];
            for (int i = 0; i < n; i++)
            {
                ControlPoints[3 * i] = points[i].X;
                ControlPoints[3 * i + 1] = points[i].Y;
                ControlPoints[3 * i + 2] = points[i].Z;
            }

            Weights = weights.ToList();
            Knots = Enumerable.Repeat(0, d).Concat(Enumerable.Range(0, n - d).Concat(Enumerable.Repeat(n - d - 1, d))).Select(x => (double)x).ToList();
        }

        /***************************************************/

        public NurbCurveB(IEnumerable<Point> controlPoints, IEnumerable<double> weights, IEnumerable<double> knots)
        {
            int n = controlPoints.Count();
            List<Point> points = controlPoints.ToList();

            ControlPoints = new double[n * 3];
            for (int i = 0; i < n; i++)
            {
                ControlPoints[3 * i] = points[i].X;
                ControlPoints[3 * i + 1] = points[i].Y;
                ControlPoints[3 * i + 2] = points[i].Z;
            }

            Weights = weights.ToList();
            Knots = knots.ToList();
        }


        /***************************************************/
        /**** Local Methods                             ****/
        /***************************************************/

        public int GetDegree()
        {
            return 1 + Knots.Count - ControlPoints.Length/3;
        }
    }
}

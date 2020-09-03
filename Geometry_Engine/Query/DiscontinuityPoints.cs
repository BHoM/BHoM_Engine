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
using System.Collections.Generic;
using System.Linq;
using System;
using BH.oM.Reflection.Attributes;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods - Curves                   ****/
        /***************************************************/

        public static List<Point> DiscontinuityPoints(this Arc curve)
        {
            return new List<Point> { curve.StartPoint(), curve.EndPoint() };
        }

        /***************************************************/

        public static List<Point> DiscontinuityPoints(this Circle curve)
        {
            return new List<Point>();
        }

        /***************************************************/

        public static List<Point> DiscontinuityPoints(this Line curve)
        {
            return new List<Point> { curve.Start, curve.End };
        }

        /***************************************************/

        [NotImplemented]
        public static List<Point> DiscontinuityPoints(this PolyCurve curve)
        {
            return curve.Curves.SelectMany((x, i) => x.IDiscontinuityPoints().Skip((i > 0) ? 1 : 0)).ToList();
        }

        /***************************************************/

        public static List<Point> DiscontinuityPoints(this Polyline curve, double distanceTolerance = Tolerance.Distance, double angleTolerance = Tolerance.Angle)
        {
            List<Point> ctrlPts = new List<Point>(curve.ControlPoints);

            if (ctrlPts.Count < 3)
                return ctrlPts;

            double sqTol = distanceTolerance * distanceTolerance;
            int j = 0;
            if (!curve.IsClosed(distanceTolerance))
                j += 2;

            for (int i = j; i < ctrlPts.Count; i++)
            {
                int cc = ctrlPts.Count;
                int i1 = (i - 1 + cc) % cc;
                int i2 = (i - 2 + cc) % cc;
                Vector v1 = ctrlPts[i1] - ctrlPts[i2];
                Vector v2 = ctrlPts[i] - ctrlPts[i1];
                double angle = v1.Angle(v2);

                if (angle <= angleTolerance || angle >= (2 * Math.PI) - angleTolerance || ctrlPts[i2].SquareDistance(ctrlPts[i1]) <= sqTol)
                {
                    ctrlPts.RemoveAt(i1);
                    i--;
                }
            }

            return ctrlPts;
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static List<Point> IDiscontinuityPoints(this ICurve curve)
        {
            return DiscontinuityPoints(curve as dynamic);
        }


        /***************************************************/
        /**** Private Fallback Methods                  ****/
        /***************************************************/

        private static List<Point> DiscontinuityPoints(this ICurve curve)
        {
            Reflection.Compute.RecordError($"DiscontinuityPoints is not implemented for ICurves of type: {curve.GetType().Name}.");
            return null;
        }

        /***************************************************/
    }
}
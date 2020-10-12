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

using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using BH.oM.Reflection.Attributes;
using BH.oM.Geometry;

namespace BH.Engine.Geometry
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static PolyCurve Clean(this PolyCurve curve, double tolerance = Tolerance.Distance)
        {
            List<ICurve> subparts = curve.SubParts();

            for (int i = 0; i < subparts.Count; i++)
            {
                for (int j = i + 1; j < subparts.Count; j++)
                {
                    if (subparts[i].IsSimilarSegment(subparts[j], tolerance))
                    {
                        subparts.RemoveAt(j);
                        subparts.RemoveAt(i);
                        i--;
                        break;
                    }
                }
            }

            List<PolyCurve> result = subparts.IJoin(tolerance);

            return result[0];
        }

        public static ICurve IClean(this ICurve curve, double tolerance = Tolerance.Distance)
        {
            return Clean(curve as dynamic, tolerance);
        }


        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static bool IsSimilarSegment(this ICurve curve, ICurve refCurve, double tolerance = Tolerance.Distance)
        {
            double sqTol = tolerance * tolerance;
            Point sp = curve.IStartPoint();
            Point rsp = refCurve.IStartPoint();
            Point mp = curve.IPointAtParameter(0.5);
            Point rmp = refCurve.IPointAtParameter(0.5);
            Point ep = curve.IEndPoint();
            Point rep = refCurve.IEndPoint();

            return mp.SquareDistance(rmp) <= sqTol &&
                   ((sp.SquareDistance(rsp) <= sqTol && ep.SquareDistance(rep) <= sqTol) ||
                   (sp.SquareDistance(rep) <= sqTol && ep.SquareDistance(rsp) <= sqTol));
        }

        /***************************************************/

    }
}

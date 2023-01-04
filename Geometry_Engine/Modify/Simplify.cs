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

using BH.oM.Geometry;
using System.Collections.Generic;

namespace BH.Engine.Geometry
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Polyline Simplify(this Polyline polyline, double distanceTolerance = Tolerance.Distance, double angleTolerance = Tolerance.Angle)
        {
            bool isClosed = polyline.IsClosed(distanceTolerance);
            List<Point> ctrlPts = polyline.DiscontinuityPoints(distanceTolerance, angleTolerance);

            for (int i = 1; i < ctrlPts.Count - 1; i++)
            {
                List<Point> checkPoints = new List<Point>() { ctrlPts[i - 1], ctrlPts[i] }.CullDuplicates(distanceTolerance);
                if (checkPoints.Count == 1)
                {
                    ctrlPts.RemoveRange(i - 1, 2);
                    ctrlPts.Insert(i - 1, checkPoints[0]);
                    i--;
                }
            }

            if (ctrlPts.Count < 2)
                return polyline;

            if (isClosed)
                ctrlPts.Add(ctrlPts[0]);

            return new Polyline { ControlPoints = ctrlPts };
        }

        /***************************************************/
    }
}




/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2025, the respective contributors. All rights reserved.
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

using BH.Engine.Geometry;
using BH.oM.Architecture.BuildersWork;
using BH.oM.Geometry;
using BH.oM.Spatial.ShapeProfiles;
using System;

namespace BH.Engine.Architecture
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static bool IsValidForInPlaneDistanceCheck(this Opening opening1, Opening opening2, double maxDistance = double.NaN, double distanceTolerance = Tolerance.Distance, double angleTolerance = Tolerance.Angle)
        {
            // Check if openings aren't nulls
            if (opening1 == null || opening2 == null)
            {
                BH.Engine.Base.Compute.RecordError("One of the openings is null, the query cannot be verified.");
                return false;
            }

            // Check if the openings are parallel
            if (opening1.CoordinateSystem.Z.IsParallel(opening2.CoordinateSystem.Z, angleTolerance) == 0)
            {
                return false;
            }

            // Check if the openigns are on the same plane/wall
            Point centerPoint1 = opening1.CoordinateSystem.Origin;
            Point centerPoint2 = opening2.CoordinateSystem.Origin;
            Plane plane2 = new Plane { Origin = centerPoint2, Normal = opening2.CoordinateSystem.Z };
            double distancePoint1Plane2 = centerPoint1.Distance(plane2);
            double distanceDepths = opening1.Depth/2 + opening2.Depth/2;
            if (distancePoint1Plane2 > distanceDepths + distanceTolerance)
            {
                return false;
            }

            // Check if the openings are closer than maxDistance
            if (!double.IsNaN(maxDistance))
            {
                double diag1 = opening1.MaxDiagonal();
                double diag2 = opening2.MaxDiagonal();
                if (double.IsNaN(diag1) || double.IsNaN(diag2))
                {
                    return true;
                }
                Point point1onPlane2 = centerPoint1.Project(plane2);
                if (maxDistance + (diag1/2 + diag2/2) + distanceTolerance < centerPoint2.Distance(point1onPlane2)) 
                {
                    return false;
                }
            }

            return true;
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static double MaxDiagonal(this Opening opening)
        {
            if (opening.Profile is CircleProfile)
            {
                return ((CircleProfile)opening.Profile).Diameter;
            }
            else if (opening.Profile is RectangleProfile)
            {
                return Math.Sqrt(Math.Pow(((RectangleProfile)opening.Profile).Height, 2) + Math.Pow(((RectangleProfile)opening.Profile).Width, 2));
            }
            else
                return double.NaN;
        }

        /***************************************************/
    }
}







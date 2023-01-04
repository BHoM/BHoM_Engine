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
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods - BoundingBox              ****/
        /***************************************************/

        public static bool IsInRange(this BoundingBox box1, BoundingBox box2, double tolerance = Tolerance.Distance)
        {
            return (box1.Min.X <= box2.Max.X + tolerance && box2.Min.X <= box1.Max.X + tolerance &&
                     box1.Min.Y <= box2.Max.Y + tolerance && box2.Min.Y <= box1.Max.Y + tolerance &&
                     box1.Min.Z <= box2.Max.Z + tolerance && box2.Min.Z <= box1.Max.Z + tolerance);
        }


        /***************************************************/
        /**** Public Methods - Point                    ****/
        /***************************************************/

        public static bool IsInRange(this Point point, BoundingBox box, double tolerance = Tolerance.Distance)
        {
            return box.IsContaining(point, true, tolerance);
        }


        /***************************************************/
        /**** Public Methods - Curve                    ****/
        /***************************************************/

        public static bool IsInRange(this ICurve curve, BoundingBox box, double tolerance = Tolerance.Distance)
        {
            if (box.IsContaining(curve.IStartPoint()) || box.IsContaining(curve.IEndPoint()))
                return true;

            List<Plane> bBoxPlanes = new List<Plane>
            {
            new Plane { Origin = box.Min, Normal = Vector.XAxis },
            new Plane { Origin = box.Min, Normal = Vector.YAxis },
            new Plane { Origin = box.Min, Normal = Vector.ZAxis },
            new Plane { Origin = box.Max, Normal = Vector.XAxis },
            new Plane { Origin = box.Max, Normal = Vector.YAxis },
            new Plane { Origin = box.Max, Normal = Vector.ZAxis }
            };

            foreach (Plane plane in bBoxPlanes)
            {
                foreach (Point point in curve.IPlaneIntersections(plane, tolerance))
                {
                    if (box.IsContaining(point, true, tolerance))
                        return true;
                }
            }

            return false;
        }

        /***************************************************/
    }
}





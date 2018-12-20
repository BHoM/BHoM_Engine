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

using BH.oM.Geometry;
using System;
using System.ComponentModel;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static double Angle(this Vector v1, Vector v2)
        {
            double dotProduct = v1.DotProduct(v2);
            double length = v1.Length() * v2.Length();

            return (Math.Abs(dotProduct) < length) ? Math.Acos(dotProduct / length) : (dotProduct < 0) ? Math.PI : 0;
        }

        /***************************************************/

        [Description("Calculates the counterclockwise angle between two vectors in a plane")]
        public static double Angle(this Vector v1, Vector v2, Plane p)
        {
            v1 = v1.Project(p).Normalise();
            v2 = v2.Project(p).Normalise();

            double dotProduct = v1.DotProduct(v2);
            Vector n = p.Normal.Normalise();
           
            double det = v1.X * v2.Y * n.Z + v2.X * n.Y * v1.Z + n.X * v1.Y * v2.Z - v1.Z * v2.Y * n.X - v2.Z * n.Y * v1.X - n.Z * v1.Y * v2.X;

            double angle = Math.Atan2(det, dotProduct);
            return angle >= 0 ? angle : Math.PI * 2 + angle;
        }

        /***************************************************/

        public static double Angle(this Arc arc)
        {
            return arc.EndAngle - arc.StartAngle;
        }

        /***************************************************/

        public static double SignedAngle(this Vector a, Vector b, Vector normal) // use normal vector to define the sign of the angle
        {
            double angle = Angle(a, b);

            Vector crossproduct = a.CrossProduct(b);
            if (crossproduct.DotProduct(normal) < 0)
                return -angle;
            else
                return angle;
        }

        /***************************************************/
    }
}

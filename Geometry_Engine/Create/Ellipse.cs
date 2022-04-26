/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2022, the respective contributors. All rights reserved.
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
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Ellipse Ellipse(Point centre, double radius1, double radius2)
        {
            return new Ellipse
            {
                Centre = centre,
                Radius1 = radius1,
                Radius2 = radius2
            };
        }

        /***************************************************/

        public static Ellipse Ellipse(Point centre, Vector axis1, Vector axis2, double radius1, double radius2)
        {
            if (Math.Abs(axis1.DotProduct(axis2)) > Tolerance.Angle)
                Base.Compute.RecordWarning("Axis1 and axis2 are not orthogonal. Result may be wrong.");

            return new Ellipse
            {
                Centre = centre,
                Axis1 = axis1.Normalise(),
                Axis2 = axis2.Normalise(),
                Radius1 = radius1,
                Radius2 = radius2
            };
        }


        /***************************************************/
        /**** Random Geometry                           ****/
        /***************************************************/

        public static Ellipse RandomEllipse(int seed = -1, BoundingBox box = null)
        {
            if (seed == -1)
                seed = NextSeed();
            Random rnd = new Random(seed);
            return RandomEllipse(rnd, box);
        }

        /***************************************************/

        public static Ellipse RandomEllipse(Random rnd, BoundingBox box = null)
        {
            if (box == null)
            {
                Vector axis1 = RandomVector(rnd).Normalise();
                return new Ellipse
                {
                    Centre = RandomPoint(rnd),
                    Axis1 = axis1,
                    Axis2 = RandomVector(rnd).Project(new Plane { Normal = axis1 }).Normalise(),
                    Radius1 = rnd.NextDouble(),
                    Radius2 = rnd.NextDouble()
                };
            }
            else
            {
                Point centre = RandomPoint(rnd, box);
                Vector axis1 = RandomVector(rnd).Normalise();
                double maxRadius = new double[]
                {
                    box.Max.X - centre.X,
                    box.Max.Y - centre.Y,
                    box.Max.Z - centre.Z,
                    centre.X - box.Min.X,
                    centre.Y - box.Min.Y,
                    centre.Z - box.Min.Z
                }.Min();

                return new Ellipse
                {
                    Centre = centre,
                    Axis1 = axis1,
                    Axis2 = RandomVector(rnd).Project(new Plane { Normal = axis1 }).Normalise(),
                    Radius1 = maxRadius * rnd.NextDouble(),
                    Radius2 = maxRadius * rnd.NextDouble()
                };
            }
        }

        /***************************************************/
    }
}




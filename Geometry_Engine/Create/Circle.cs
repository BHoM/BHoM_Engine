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

        public static Circle Circle(Point centre, double radius = 0)
        {
            return new Circle
            {
                Centre = centre,
                Radius = radius
            }; 
        }

        /***************************************************/

        public static Circle Circle(Point centre, Vector normal, double radius = 0)
        {
            return new Circle
            {
                Centre = centre,
                Normal = normal,
                Radius = radius
            };
        }
        
        /***************************************************/

        public static Circle Circle(Point pt1, Point pt2, Point pt3, double tolerance = Tolerance.Distance)
        {
            Vector v1 = pt1 - pt3;
            Vector v2 = pt2 - pt3;
            Vector normal = v1.CrossProduct(v2).Normalise();

            Point centre = Query.LineIntersection(
                Create.Line(pt3 + v1 / 2, v1.CrossProduct(normal)),
                Create.Line(pt3 + v2 / 2, v2.CrossProduct(normal)),
                true,
                tolerance
            );

            return new Circle { Centre = centre, Normal = normal, Radius = pt1.Distance(centre) };
        }


        /***************************************************/
        /**** Random Geometry                           ****/
        /***************************************************/

        public static Circle RandomCircle(int seed = -1, BoundingBox box = null)
        {
            if (seed == -1)
                seed = NextSeed();
            Random rnd = new Random(seed);
            return RandomCircle(rnd, box);
        }

        /***************************************************/

        public static Circle RandomCircle(Random rnd, BoundingBox box = null)
        {
            if (box == null)
            {
                return new Circle
                {
                    Centre = RandomPoint(rnd),
                    Normal = RandomVector(rnd).Normalise(),
                    Radius = rnd.NextDouble()
                };
            }
            else
            {
                Point centre = RandomPoint(rnd, box);
                double maxRadius = new double[]
                {
                    box.Max.X - centre.X,
                    box.Max.Y - centre.Y,
                    box.Max.Z - centre.Z,
                    centre.X - box.Min.X,
                    centre.Y - box.Min.Y,
                    centre.Z - box.Min.Z
                }.Min();

                return new Circle
                {
                    Centre = centre,
                    Normal = RandomVector(rnd).Normalise(),
                    Radius = maxRadius * rnd.NextDouble()
                };
            }
        }

        /***************************************************/
    }
}




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
using BH.oM.Geometry.CoordinateSystem;
using System;
using System.Linq;
using System.ComponentModel;
using BH.oM.Base.Attributes;
using BH.oM.Quantities.Attributes;

namespace BH.Engine.Geometry
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates an Arc based on its core properties.")]
        [InputFromProperty("coordinateSystem")]
        [InputFromProperty("radius")]
        [InputFromProperty("startAngle")]
        [InputFromProperty("endAngle")]
        [Output("arc", "The created Arc.")]
        public static Arc Arc(Cartesian coordinateSystem, double radius, double startAngle, double endAngle)
        {
            return new Arc
            {
                CoordinateSystem = coordinateSystem,
                Radius = radius,
                StartAngle = startAngle,
                EndAngle = endAngle
            };
        }

        /***************************************************/

        [Description("Creates an Arc from a start and end point as well as a point along its length.")]
        [Input("start", "Start point of the Arc.")]
        [Input("middle", "Point somehwere along the arc between the start and end point.")]
        [Input("end", "End point of the Arc.")]
        [Input("tolerance", "Tolerance to be used in the method.", typeof(Length))]
        [Output("arc", "The created Arc.")]
        public static Arc Arc(Point start, Point middle, Point end, double tolerance = Tolerance.Distance)
        {
            Vector v1 = start - middle;
            Vector v2 = end - middle;
            Vector normal = v2.CrossProduct(v1);

            Point centre =  Query.LineIntersection(
                Create.Line(middle + v1 / 2, v1.CrossProduct(normal)),
                Create.Line(middle + v2 / 2, v2.CrossProduct(normal)),
                true,
                tolerance
            );

            Vector stVec = start - centre;
            Vector enVec = end - centre;

            Cartesian system = CartesianCoordinateSystem(centre, stVec, stVec.Rotate(0.5*Math.PI, normal));
            double angle = stVec.Angle(enVec, (Plane)system);

            return new Arc
            {
                CoordinateSystem = system,
                Radius = stVec.Length(),
                StartAngle = 0,
                EndAngle = angle
            };
        }


        /***************************************************/
        /**** Arc by Centre - Special Case              ****/
        /***************************************************/

        [Description("Creates an arc by centre, start and end points. Only able to create arcs with angle < 180 degrees.")]
        [Input("centre", "Centre point of the arc.")]
        [Input("start", "Start point of the Arc.")]
        [Input("end", "End point of the Arc.")]
        [Input("tolerance", "Tolerance to be used in the method.", typeof(Length))]
        [Output("arc", "The created Arc.")]
        public static Arc ArcByCentre(Point centre, Point start, Point end, double tolerance = Tolerance.Distance)
        {
            double radius = start.Distance(centre);

            //Check that start and end points are the same distance from the centre point
            if (Math.Abs(radius - end.Distance(centre)) > tolerance)
            {
                Base.Compute.RecordError("Start and end points need to have the same distance from the centre point");
                return null;
            }

            Vector v1 = start - centre;
            Vector v2 = end - centre;

            double angle = v1.Angle(v2);

            if (Math.Abs(angle - Math.PI) < Tolerance.Angle)
            {
                Base.Compute.RecordError("Method does not work for colinear points");
                return null;
            }

            Cartesian system = CartesianCoordinateSystem(centre, v1, v2);
            
            return new Arc
            {
                CoordinateSystem = system,
                Radius = radius,
                StartAngle = 0,
                EndAngle = angle
            };
        }


        /***************************************************/
        /**** Random Geometry                           ****/
        /***************************************************/

        [Description("Creates a random Arc based on a seed. If no seed is provided, a random one will be generated. If Box is provided, the resulting geometry will be contained within the box.")]
        [Input("seed", "Input seed for random generation. If -1 is provided, a random seed will be generated.")]
        [Input("box", "Optional containing box. The geometry created will be limited to the bounding box. If no box is provided, values between 0 and 1 will be used when generating properties for the geometry.")]
        [Output("arc", "The generated random Arc.")]
        public static Arc RandomArc(int seed = -1, BoundingBox box = null)
        {
            if (seed == -1)
                seed = NextRandomSeed();
            Random rnd = new Random(seed);
            return RandomArc(rnd, box);
        }

        /***************************************************/

        [Description("Creates a random Arc using the provided Random class. If Box is provided, the resulting geometry will be contained within the box.")]
        [Input("rnd", "Random object to be used to generate the random geometry.")]
        [Input("box", "Optional containing box. The geometry created will be limited to the bounding box. If no box is provided, values between 0 and 1 will be used when generating properties for the geometry.")]
        [Output("arc", "The generated random Arc.")]
        public static Arc RandomArc(Random rnd, BoundingBox box = null)
        {
            Circle circle = RandomCircle(rnd, box);
            double length = circle.Length();
            double startLength = length * rnd.NextDouble();
            double endLength = length * rnd.NextDouble();

            //TODO: Can be made more efficient with new definition of arc
            return Arc(
                circle.PointAtLength(startLength),
                circle.PointAtLength((startLength + endLength) / 2),
                circle.PointAtLength(endLength)
            );
        }

        /***************************************************/

        [Description("Creates a random Arc with a set start point based on a seed. If no seed is provided, a random one will be generated. If Box is provided, the resulting geometry will be contained within the box.")]
        [Input("from", "The start point of the Arc.")]
        [Input("seed", "Input seed for random generation. If -1 is provided, a random seed will be generated.")]
        [Input("box", "Optional containing box. The geometry created will be limited to the bounding box. If no box is provided, values between 0 and 1 will be used when generating properties for the geometry.")]
        [Output("arc", "The generated random Arc.")]
        public static Arc RandomArc(Point from, int seed = -1, BoundingBox box = null)
        {
            if (seed == -1)
                seed = NextRandomSeed();
            Random rnd = new Random(seed);
            return RandomArc(from, rnd, box);
        }

        /***************************************************/

        [Description("Creates a random Arc with a set start point using the provided Random class. If Box is provided, the resulting geometry will be contained within the box.")]
        [Input("from", "The start point of the Arc.")]
        [Input("rnd", "Random object to be used to generate the random geometry.")]
        [Input("box", "Optional containing box. The geometry created will be limited to the bounding box. If no box is provided, values between 0 and 1 will be used when generating properties for the geometry.")]
        [Output("arc", "The generated random Arc.")]
        public static Arc RandomArc(Point from, Random rnd, BoundingBox box = null)
        {
            Point centre;
            Vector normal;
            double radius;
            if (box == null)
            {
                centre = RandomPoint(rnd);
                normal = RandomVector(rnd).CrossProduct(centre - from).Normalise();
                radius = from.Distance(centre);
            }
            else
            {
                double maxRadius = new double[]
                {
                    box.Max.X - from.X,
                    box.Max.Y - from.Y,
                    box.Max.Z - from.Z,
                    from.X - box.Min.X,
                    from.Y - box.Min.Y,
                    from.Z - box.Min.Z
                }.Min()/2;

                radius = maxRadius * rnd.NextDouble();
                Vector v = RandomVector(rnd).Normalise();
                centre = from + v * radius;
                normal = RandomVector(rnd).CrossProduct(v).Normalise();
            }

            Circle circle = Circle(centre, normal, radius);
            double length = circle.Length();
            double endLength = length * rnd.NextDouble();

            return ArcByCentre(centre, from, circle.PointAtLength(endLength));

        }

        /***************************************************/
        /**** Private Method                            ****/
        /***************************************************/

        [Description("Gets next seed for random generation.")]
        private static int NextRandomSeed()
        {
            lock (m_RandomLock)
            {
                return m_Random.Next();
            }
        }

        /***************************************************/
        /**** Private Fields                            ****/
        /***************************************************/

        private static Random m_Random = new Random();
        private static object m_RandomLock = new object();
        /***************************************************/
    }
}





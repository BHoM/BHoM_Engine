/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2024, the respective contributors. All rights reserved.
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
using System.Collections.Generic;
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

        [Description("Creates a Polyline based on a collection of points.")]
        [InputFromProperty("points")]
        [Output("pLine", "The created Polyline.")]
        public static Polyline Polyline(IEnumerable<Point> points)
        {
            return new Polyline { ControlPoints = points.ToList() };
        }

        /***************************************************/

        [Description("Creates a Polyline From a collection of lines by taking the start point of the first line and the end points of all lines.")]
        [Input("lines", "The lines to create a polyline from.")]
        [Output("pLine", "The created Polyline.")]
        public static Polyline Polyline(List<Line> lines)
        {
            if (lines.Count == 0)
                return null;

            List<Point> controlPoints = new List<Point> { lines[0].Start };
            foreach (Line l in lines)
            {
                controlPoints.Add(l.End);
            }
            return new Polyline { ControlPoints = controlPoints };
        }


        /***************************************************/
        /**** Random Geometry                           ****/
        /***************************************************/

        [Description("Creates a random Polyline based on a seed. If no seed is provided, a random one will be generated. If Box is provided, the resulting geometry will be contained within the box.")]
        [Input("seed", "Input seed for random generation. If -1 is provided, a random seed will be generated.")]
        [Input("box", "Optional containing box. The geometry created will be limited to the bounding box. If no box is provided, values between 0 and 1 will be used when generating properties for the geometry.")]
        [Input("minNbCPs", "Minimun number of vertices in the random Polyline.")]
        [Input("maxNbCPs", "Maximum number of vertices in the random Polyline.")]
        [Output("Mesh", "The generated random Polyline.")]
        public static Polyline RandomPolyline(int seed = -1, BoundingBox box = null, int minNbCPs = 3, int maxNbCPs = 20)
        {
            if (seed == -1)
                seed = NextRandomSeed();
            Random rnd = new Random(seed);
            return RandomPolyline(rnd, box, minNbCPs, maxNbCPs);
        }

        /***************************************************/

        [Description("Creates a random Polyline using the provided Random class. If Box is provided, the resulting geometry will be contained within the box.")]
        [Input("rnd", "Random object to be used to generate the random geometry.")]
        [Input("box", "Optional containing box. The geometry created will be limited to the bounding box. If no box is provided, values between 0 and 1 will be used when generating properties for the geometry.")]
        [Input("minNbCPs", "Minimun number of vertices in the random Polyline.")]
        [Input("maxNbCPs", "Maximum number of vertices in the random Polyline.")]
        [Output("loft", "The generated random Polyline.")]
        public static Polyline RandomPolyline(Random rnd, BoundingBox box = null, int minNbCPs = 3, int maxNbCPs = 20)
        {
            List<Point> points = new List<Point>();
            for (int i = 0; i < rnd.Next(minNbCPs, maxNbCPs + 1); i++)
                points.Add(RandomPoint(rnd, box));
            return new Polyline { ControlPoints = points };
        }

        /***************************************************/

        [Description("Creates a random Polyline with a set start point based on a seed. If no seed is provided, a random one will be generated. If Box is provided, the resulting geometry will be contained within the box.")]
        [Input("from", "The start point of the Polyline.")]
        [Input("seed", "Input seed for random generation. If -1 is provided, a random seed will be generated.")]
        [Input("box", "Optional containing box. The geometry created will be limited to the bounding box. If no box is provided, values between 0 and 1 will be used when generating properties for the geometry.")]
        [Input("minNbCPs", "Minimun number of vertices in the random Polyline.")]
        [Input("maxNbCPs", "Maximum number of vertices in the random Polyline.")]
        [Output("pLine", "The generated random Polyline.")]
        public static Polyline RandomPolyline(Point from, int seed = -1, BoundingBox box = null, int minNbCPs = 3, int maxNbCPs = 20)
        {
            if (seed == -1)
                seed = NextRandomSeed();
            Random rnd = new Random(seed);
            return RandomPolyline(from, rnd, box, minNbCPs, maxNbCPs);
        }

        /***************************************************/

        [Description("Creates a random Polyline with a set start point using the provided Random class. If Box is provided, the resulting geometry will be contained within the box.")]
        [Input("from", "The start point of the Polyline.")]
        [Input("rnd", "Random object to be used to generate the random geometry.")]
        [Input("box", "Optional containing box. The geometry created will be limited to the bounding box. If no box is provided, values between 0 and 1 will be used when generating properties for the geometry.")]
        [Input("minNbCPs", "Minimun number of vertices in the random Polyline.")]
        [Input("maxNbCPs", "Maximum number of vertices in the random Polyline.")]
        [Output("pLine", "The generated random Polyline.")]
        public static Polyline RandomPolyline(Point from, Random rnd, BoundingBox box = null, int minNbCPs = 3, int maxNbCPs = 20)
        {
            List<Point> points = new List<Point>();
            points.Add(from);
            for (int i = 0; i < rnd.Next(minNbCPs, maxNbCPs + 1) - 1; i++)
            {
                points.Add(RandomPoint(rnd, box));
            }

            return new Polyline { ControlPoints = points };
        }

        /***************************************************/
    }
}






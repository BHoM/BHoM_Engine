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

using BH.Engine.Data;
using BH.oM.Data.Collections;
using BH.oM.Geometry;
using BH.oM.Base.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Compute
    {
        /***************************************************/
        /****   Public Methods                          ****/
        /***************************************************/

        [Description("Computes contour lines of the iso-values in the scalar field defined by the mesh and the values. \n" +
                     "The values are assumed to change linearly between vertices.")]
        [Input("mesh", "Mesh defining the positions of the values in the scalar field.")]
        [Input("vertexValues", "The value for of scalar field for each vertex in the mesh.")]
        [Input("isoValues", "Values in the scalar field to produce the iso-lines for.")]
        [Output("isoLines", "Lines covering the iso-values in the scalar field defined by the mesh and values.")]
        public static List<List<Line>> MarchingSquares(this Mesh mesh, List<double> vertexValues, List<double> isoValues)
        {
            if (mesh?.Vertices == null || vertexValues == null || mesh.Vertices.Count != vertexValues.Count)
            {
                Base.Compute.RecordError("Number of vertexValues must match the number of vertices in the mesh.");
                return new List<List<Line>>();
            }

            if (isoValues == null)
                return new List<List<Line>>();

            List<List<Line>> results = new List<List<Line>>();
            for (int i = 0; i < isoValues.Count; i++)
                results.Add(new List<Line>());

            // Look at every face
            foreach (Face face in mesh.Faces)
            {
                // go around its edges and create lines between the intermediate points
                Point[] prePoint = new Point[isoValues.Count];
                int[] indexArray = face.Vertices();
                for (int i = 0; i < indexArray.Length; i++)
                {
                    int indJ = indexArray[(i + 1) % indexArray.Length];
                    int indI = indexArray[i];

                    // get points on face edge at the iso value
                    List<Point> pts = IntermediatePoints(
                        mesh.Vertices[indI], vertexValues[indI],
                        mesh.Vertices[indJ], vertexValues[indJ],
                        isoValues);

                    // Check if there were a result for each iso value
                    for (int k = 0; k < pts.Count; k++)
                    {
                        Point pt = pts[k];

                        if (pt == null)
                            continue;

                        // As a Faces edges are a closed loop, there will always be an even number of results.
                        if (prePoint[k] == null)
                            prePoint[k] = pt;
                        else
                        {
                            results[k].Add(new Line()
                            {
                                Start = prePoint[k],
                                End = pt,
                            });
                            prePoint[k] = null;
                        }
                    }
                }
            }

            return results;
        }


        /***************************************************/
        /****   Private Methods                         ****/
        /***************************************************/

        private static List<Point> IntermediatePoints(Point firstPt, double fisrtValue, Point secondPt, double secondValue, List<double> evaluationValues)
        {
            Domain domain = Data.Create.Domain(fisrtValue, secondValue);
            double distance = domain.Max - domain.Min;
            List<Point> results = new List<Point>();

            foreach (double eVal in evaluationValues)
            {
                if (!domain.IsInRange(eVal))
                {
                    results.Add(null);
                    continue;
                }

                double d = Math.Abs(eVal - fisrtValue);
                double factor = d / distance;

                results.Add(firstPt + (secondPt - firstPt) * factor);
            }

            return results;
        }

        /***************************************************/

    }
}





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

using BH.oM.Geometry;
using BH.oM.Dimensional;
using System;
using System.Collections.Generic;
using System.Linq;
using BH.oM.Analytical.Elements;
using BH.oM.Facade.Elements;
using BH.oM.Facade;
using BH.oM.Facade.SectionProperties;
using BH.Engine.Geometry;
using BH.Engine.Spatial;
using BH.oM.Reflection;
using BH.oM.Reflection.Attributes;
using System.ComponentModel;

namespace BH.Engine.Facade
{
    public static partial class Compute
    {
        /***************************************************/
        /****          Public Methods                   ****/
        /***************************************************/

        [Description("Returns adjacent edges and elements at a provided frame edge for a collection of panels")]
        [Input("edge", "Frame edge to find adjacencies at")]
        [Input("elems", "2D elements to use to find edge adjacencies (These should be panels and/or openings)")]
        [MultiOutput(0, "adjEdges", "Adjacent edges")]
        [MultiOutput(1, "adjElems", "Adjacent Elements per adjacent edge")]
        public static Output<List<IElement1D>, List<IElement2D>> FrameEdgeAdjacencies(this IEdge edge, List<IElement2D> elems)
        {
            Point point = edge.Curve.IPointAtParameter(0.5);
            Vector dir = edge.IGeometry().IEndDir();
            return FrameEdgeAdjacencies(point, elems, dir)
        }

        /***************************************************/

        [Description("Returns adjacent edges and elements at a provided point for a collection of panels")]
        [Input("point", "Frame edge to find adjacencies at")]
        [Input("elems", "2D elements to use to find edge adjacencies (These should be panels and/or openings)")]
        [Input("refDir", "Reference vector direction to check adjacencies. Edges not parallel to this direction will not be included.")]
        [MultiOutput(0, "adjEdges", "Adjacent edges")]
        [MultiOutput(1, "adjElems", "Adjacent Elements per adjacent edge")]
        public static Output<List<IElement1D>, List<IElement2D>> FrameEdgeAdjacencies(this Point point, List<IElement2D> elems, Vector refDir)
        {
            List<IElement1D> adjEdges = new List<IElement1D>();
            List<IElement2D> adjElems = new List<IElement2D>();

            foreach (IElement2D elem in elems)
            {
                List<IElement1D> edges = elem.IOutlineElements1D();

                foreach (IElement1D refEdge in edges)
                {
                    double distance = point.IDistance(refEdge.IGeometry());
                    Vector edgeDir = refEdge.IGeometry().IEndDir();
                    double angleBetween1 = edgeDir.Angle(refDir);
                    double angleBetween2 = edgeDir.Angle(refDir.Reverse());
                    double angleBetween = Math.Min(angleBetween1, angleBetween2);

                    if (distance < Tolerance.Distance && angleBetween < Tolerance.Angle)
                    {
                        adjEdges.Add(refEdge);
                        adjElems.Add(elem);
                    }
                }
            }

            // Return the adjacencies
            return new Output<List<IElement1D>, List<IElement2D>>
            {
                Item1 = adjEdges,
                Item2 = adjElems,
            };
        }

    }
}


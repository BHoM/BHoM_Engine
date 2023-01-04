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
using BH.oM.Base;
using BH.oM.Base.Attributes;
using System.ComponentModel;

namespace BH.Engine.Facade
{
    public static partial class Compute
    {
        /***************************************************/
        /****          Public Methods                   ****/
        /***************************************************/

        [Description("Returns adjacent edges and elements at a provided frame edge for a collection of panels.")]
        [Input("edge", "Edge to find adjacencies at.")]
        [Input("elements", "2D elements to use to find edge adjacencies (These should be panels and/or openings).")]
        [Input("tolerance", "Tolerance is the minimum overlap amount required to consider adjacent.")]
        [MultiOutput(0, "adjacentEdges", "Adjacent edges.")]
        [MultiOutput(1, "adjacentElems", "Adjacent Elements per adjacent edge.")]
        public static Output<List<IElement1D>, List<IElement2D>> EdgeAdjacencies(this IElement1D edge, IEnumerable<IElement2D> elements, double tolerance = Tolerance.Distance)
        {
            if (edge == null || elements == null)
            {
                Base.Compute.RecordWarning("Can not get adjacencies of a null element.");
                return null;
            }

            List<IElement1D> adjEdges = new List<IElement1D>();
            List<IElement2D> adjElems = new List<IElement2D>();

            foreach (IElement2D elem in elements)
            {
                List<IElement1D> edges = elem.IOutlineElements1D();

                foreach (IElement1D refEdge in edges)
                {
                    if (Query.IIsAdjacent(refEdge, edge))
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

        /***************************************************/

    }
}





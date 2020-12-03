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

        [Description("Returns unique adjacent edge and element conditions for a collection of elements, based on their property names.")]
        [Input("elems", "Facade elements to use to find unique edge adjacencies.")]
        [Input("splitHorAndVert", "If true, matching horizontal and vertical adjacencies are counted as unique and labeled.")]
        [MultiOutput(0, "uniqueAdjacencyNames", "A list of the unique adjacencies existent within the collection of elements.")]
        [MultiOutput(1, "uniqueAdjacencyEdges", "The collection of edges that represents the first found case of the unique adjacency.")]
        [MultiOutput(2, "uniqueAdjacencyElements", "The collection of elements that represents the first found case of the unique adjacency.")]
        public static Output<List<string>, List<List<IElement1D>>, List<List<IElement2D>>> UniqueAdjacencies(List<IElement2D> elems, bool splitHorAndVert = false)
        {
            List<string> adjacencyIDs = new List<string>();
            List<List<IElement1D>> adjEdges = new List<List<IElement1D>>();
            List<List<IElement2D>> adjElems = new List<List<IElement2D>>();

            foreach (IElement2D elem in elems)
            {
                List<IElement2D> tempElems = elems.Except(new List<IElement2D> { elem }).ToList();
                foreach (IEdge edge in elem.IOutlineElements1D())
                {                  
                    BH.oM.Reflection.Output<List<IElement1D>, List<IElement2D>> result = edge.EdgeAdjacencies(tempElems);
                    for (int i = 0; i < result.Item1.Count; i++)
                    {
                        string adjPrefix = "";
                        List<IElement1D> edgeAdjPair = new List<IElement1D> { edge, result.Item1[i] };
                        List<IElement2D> elemAdjPair = new List<IElement2D> { elem, result.Item2[i] };
                        if (splitHorAndVert)
                        {
                            adjPrefix = Math.Abs(edge.Curve.IEndDir().Z) > 0.707 ? "Vertical-" : "Horizontal-"; // check if line is closer to vertical or horizontal
                        }
                        string adjacencyID = adjPrefix + Query.AdjacencyID(edgeAdjPair, elemAdjPair);
                        if (!adjacencyIDs.Contains(adjacencyID))
                        {
                            adjacencyIDs.Add(adjacencyID);
                            adjEdges.Add(edgeAdjPair);
                            adjElems.Add(elemAdjPair);
                        }
                    }
                }
            }
            // Return the adjacency ids and elemens as multi output
            return new Output<List<string>, List<List<IElement1D>>, List<List<IElement2D>>>
            {
                Item1 = adjacencyIDs,
                Item2 = adjEdges,
                Item3 = adjElems,
            };

        }

    }
}


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
        [MultiOutput(1, "uniqueAdjacencyEdge1", "The first edge of each pair per each unique adjacency.")]
        [MultiOutput(2, "uniqueAdjacencyElement1", "The first element of each pair per each unique adjacency.")]
        [MultiOutput(3, "uniqueAdjacencyEdge2", "The second edge of each pair per each unique adjacency.")]
        [MultiOutput(4, "uniqueAdjacencyElement2", "The second element of each pair per each unique adjacency.")]
        public static Output<List<string>, List<List<IElement1D>>, List<List<IElement2D>>, List<List<IElement1D>>, List<List<IElement2D>>> UniqueAdjacencies(List<IElement2D> elems, bool splitHorAndVert = false)
        {
            List<List<IElement1D>> adjEdges = new List<List<IElement1D>>();
            List<List<IElement2D>> adjElems = new List<List<IElement2D>>();

            IEnumerable<Panel> panels = elems.OfType<Panel>();

            foreach (Panel panel in panels.ToList())
            {
                List<Opening> panelOpenings = panel.Openings;
                elems.AddRange(panelOpenings);
            }

            List<IElement2D> uniqueElems = elems.Distinct().ToList();
            Dictionary <string, List<IElement1D>> edgeDict1 = new Dictionary <string, List<IElement1D>> ();
            Dictionary<string, List<IElement1D>> edgeDict2 = new Dictionary<string, List<IElement1D>>();
            Dictionary <string, List<IElement2D>> elemDict1 = new Dictionary <string, List<IElement2D>>();
            Dictionary<string, List<IElement2D>> elemDict2 = new Dictionary<string, List<IElement2D>>();

            foreach (IElement2D elem in uniqueElems)
            {
                List<IElement2D> tempElems = elems.Except(new List<IElement2D> { elem }).ToList();
                foreach (IElement1D edge in elem.IOutlineElements1D())
                {                  
                    BH.oM.Reflection.Output<List<IElement1D>, List<IElement2D>> result = edge.EdgeAdjacencies(tempElems);
                    for (int i = 0; i < result.Item1.Count; i++)
                    {
                        string adjPrefix = ""; 
                        List<IElement1D> edgeAdjPair = new List<IElement1D> { edge, result.Item1[i] };
                        List<IElement2D> elemAdjPair = new List<IElement2D> { elem, result.Item2[i] };
                        if (splitHorAndVert)
                        {
                            adjPrefix = Math.Abs(edge.ElementCurves()[0].IEndDir().Z) > 0.707 ? "Vertical-" : "Horizontal-"; // check if line is closer to vertical or horizontal
                        }
                        string adjacencyID = adjPrefix + Query.AdjacencyID(edgeAdjPair, elemAdjPair);
                        if (edgeDict1.Keys.Contains(adjacencyID))
                        {
                            List<IElement1D> prevVals = edgeDict1[adjacencyID];
                            prevVals.Add(edgeAdjPair[0]);
                            edgeDict1[adjacencyID] = prevVals;
                            prevVals = edgeDict2[adjacencyID];
                            prevVals.Add(edgeAdjPair[1]);
                            edgeDict2[adjacencyID] = prevVals;
                            List<IElement2D> prevVals2D = elemDict1[adjacencyID];
                            prevVals2D.Add(elemAdjPair[0]);
                            elemDict1[adjacencyID] = prevVals2D;
                            prevVals2D = elemDict2[adjacencyID];
                            prevVals2D.Add(elemAdjPair[1]);
                            elemDict2[adjacencyID] = prevVals2D;
                        }
                        else
                        {
                            List<IElement1D> vals = new List<IElement1D>();
                            vals.Add(edgeAdjPair[0]);
                            edgeDict1.Add(adjacencyID, vals);
                            vals = new List<IElement1D>();
                            vals.Add(edgeAdjPair[1]);
                            edgeDict2.Add(adjacencyID, vals);
                            List<IElement2D> vals2D = new List<IElement2D>();
                            vals2D.Add(elemAdjPair[0]);
                            elemDict1.Add(adjacencyID, vals2D);
                            vals2D = new List<IElement2D>();
                            vals2D.Add(elemAdjPair[1]);
                            elemDict2.Add(adjacencyID, vals2D);
                        }
                    }
                }

                if (elem.InternalOutlineCurves().Count > 0)
                {
                    List<PolyCurve> internalEdgesOfPanel = elem.InternalOutlineCurves().BooleanUnion();
                    List<ICurve> intCrvs = new List<ICurve>();
                    foreach (PolyCurve intPanelEdge in internalEdgesOfPanel)
                    {
                        List<ICurve> subCrvs = intPanelEdge.SubParts();
                        intCrvs.AddRange(subCrvs);
                    }

                    foreach (ICurve intPanelEdge in intCrvs)
                    {
                        BH.oM.Reflection.Output<List<IElement1D>, List<IElement2D>> result = intPanelEdge.EdgeAdjacencies(tempElems);
                        for (int i = 0; i < result.Item1.Count; i++)
                        {
                            string adjPrefix = "";
                            List<IElement1D> edgeAdjPair = new List<IElement1D> { intPanelEdge, result.Item1[i] };
                            List<IElement2D> elemAdjPair = new List<IElement2D> { elem, result.Item2[i] };
                            if (splitHorAndVert)
                            {
                                adjPrefix = Math.Abs(intPanelEdge.IEndDir().Z) > 0.707 ? "Vertical-" : "Horizontal-"; // check if line is closer to vertical or horizontal
                            }
                            string adjacencyID = adjPrefix + Query.AdjacencyID(edgeAdjPair, elemAdjPair);
                            if (edgeDict1.Keys.Contains(adjacencyID))
                            {
                                List<IElement1D> prevVals = edgeDict1[adjacencyID];
                                prevVals.Add(edgeAdjPair[0]);
                                edgeDict1[adjacencyID] = prevVals;
                                prevVals = edgeDict2[adjacencyID];
                                prevVals.Add(edgeAdjPair[1]);
                                edgeDict2[adjacencyID] = prevVals;
                                List<IElement2D> prevVals2D = elemDict1[adjacencyID];
                                prevVals2D.Add(elemAdjPair[0]);
                                elemDict1[adjacencyID] = prevVals2D;
                                prevVals2D = elemDict2[adjacencyID];
                                prevVals2D.Add(elemAdjPair[1]);
                                elemDict2[adjacencyID] = prevVals2D;
                            }
                            else
                            {
                                List<IElement1D> vals = new List<IElement1D>();
                                vals.Add(edgeAdjPair[0]);
                                edgeDict1.Add(adjacencyID, vals);
                                vals = new List<IElement1D>();
                                vals.Add(edgeAdjPair[1]);
                                edgeDict2.Add(adjacencyID, vals);
                                List<IElement2D> vals2D = new List<IElement2D>();
                                vals2D.Add(elemAdjPair[0]);
                                elemDict1.Add(adjacencyID, vals2D);
                                vals2D = new List<IElement2D>();
                                vals2D.Add(elemAdjPair[1]);
                                elemDict2.Add(adjacencyID, vals2D);
                            }
                        }
                    }
                }
            }
            List<List<IElement1D>> edgePair1 = edgeDict1.Values.ToList();
            List<List<IElement2D>> elemPair1 = elemDict1.Values.ToList();
            List<List<IElement1D>> edgePair2 = edgeDict2.Values.ToList();
            List<List<IElement2D>> elemPair2 = elemDict2.Values.ToList();
            // Return the adjacency ids and elements as multi output
            return new Output<List<string>, List<List<IElement1D>>, List<List<IElement2D>>, List<List<IElement1D>>, List<List<IElement2D>>>
            {
                Item1 = edgeDict1.Keys.ToList(),
                Item2 = edgePair1,
                Item3 = elemPair1,
                Item4 = edgePair2,
                Item5 = elemPair2,
            };

        }

    }
}


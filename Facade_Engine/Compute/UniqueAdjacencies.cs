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
using BH.oM.Dimensional;
using System;
using System.Collections.Generic;
using System.Linq;
using BH.oM.Analytical.Elements;
using BH.oM.Facade.Elements;
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

        [Description("Returns unique adjacent edge and element conditions for a collection of elements, based on their property names.")]
        [Input("elems", "Facade elements to use to find unique edge adjacencies.")]
        [Input("splitHorAndVert", "If true, matching horizontal and vertical adjacencies are counted as unique and labeled.")]
        [MultiOutput(0, "uniqueAdjacencyNames", "A list of the unique adjacencies existent within the collection of elements.")]
        [MultiOutput(1, "uniqueAdjacencyEdge1", "The first edge of each pair per each unique adjacency.")]
        [MultiOutput(2, "uniqueAdjacencyElement1", "The first element of each pair per each unique adjacency.")]
        [MultiOutput(3, "uniqueAdjacencyEdge2", "The second edge of each pair per each unique adjacency.")]
        [MultiOutput(4, "uniqueAdjacencyElement2", "The second element of each pair per each unique adjacency.")]
        public static Output<List<string>, List<List<IElement1D>>, List<List<IElement2D>>, List<List<IElement1D>>, List<List<IElement2D>>> UniqueAdjacencies(IEnumerable<IElement2D> elems, bool splitHorAndVert = false)
        {
            if(elems == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot calculate unique adjacencies if the input elements are null.");
                return new Output<List<string>, List<List<IElement1D>>, List<List<IElement2D>>, List<List<IElement1D>>, List<List<IElement2D>>>();
            }

            List<IElement2D> elemsList = elems.ToList(); 
            
            List<string> adjacencyIDs = new List<string>();
            List<List<IElement1D>> adjEdges = new List<List<IElement1D>>();
            List<List<IElement2D>> adjElems = new List<List<IElement2D>>();
            List<List<IObject>> elemsPerAdj = new List<List<IObject>>();

            List<List<IElement1D>> edgePair1 = new List<List<IElement1D>>();
            List<List<IElement1D>> edgePair2 = new List<List<IElement1D>>();
            List<List<IElement2D>> elemPair1 = new List<List<IElement2D>>();
            List<List<IElement2D>> elemPair2 = new List<List<IElement2D>>();

            IEnumerable<Panel> panels = elems.OfType<Panel>();
            foreach (Panel panel in panels.ToList())
            {
                List<Opening> panelOpenings = panel.Openings;
                elemsList.AddRange(panelOpenings);
            }
            List<IElement2D> uniqueElems = elemsList.Distinct().ToList();

            foreach (IElement2D elem in uniqueElems)
            {
                List<IElement2D> tempElems = uniqueElems.Except(new List<IElement2D> { elem }).ToList();
                foreach (IEdge edge in elem.IOutlineElements1D())
                {
                    BH.oM.Base.Output<List<IElement1D>, List<IElement2D>> result = edge.EdgeAdjacencies(tempElems);
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
                        List<IObject> adjObjList = new List<IObject>() { edge, result.Item1[i], elem, result.Item2[i] };
                        if (!adjacencyIDs.Contains(adjacencyID))
                        {
                            adjacencyIDs.Add(adjacencyID);
                            elemsPerAdj.Add(new List<IObject>());
                        }
                        int listNum = adjacencyIDs.IndexOf(adjacencyID);
                        elemsPerAdj[listNum].AddRange(adjObjList);                       
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
                        BH.oM.Base.Output<List<IElement1D>, List<IElement2D>> result = intPanelEdge.EdgeAdjacencies(tempElems);
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
                            List<IObject> adjObjList = new List<IObject>() { intPanelEdge, result.Item1[i], elem, result.Item2[i] };
                            if (!adjacencyIDs.Contains(adjacencyID))
                            {
                                adjacencyIDs.Add(adjacencyID);
                                elemsPerAdj.Add(new List<IObject>());
                            }
                            int listNum = adjacencyIDs.IndexOf(adjacencyID);
                            elemsPerAdj[listNum].AddRange(adjObjList);
                        }
                    }
                }
            }
            foreach (List<IObject> adjObjs in elemsPerAdj)
            {
                List<IObject> edge1List = adjObjs.Where((x, i) => i % 4 == 0).ToList();
                List<IObject> edge2List = adjObjs.Where((x, i) => i % 4 == 1).ToList();
                List<IObject> elem1List = adjObjs.Where((x, i) => i % 4 == 2).ToList();
                List<IObject> elem2List = adjObjs.Where((x, i) => i % 4 == 3).ToList();
                edgePair1.Add(edge1List.Cast<IElement1D>().ToList());
                edgePair2.Add(edge2List.Cast<IElement1D>().ToList());
                elemPair1.Add(elem1List.Cast<IElement2D>().ToList());
                elemPair2.Add(elem2List.Cast<IElement2D>().ToList());
            }
            // Return the adjacency ids and elements as multi output
            return new Output<List<string>, List<List<IElement1D>>, List<List<IElement2D>>, List<List<IElement1D>>, List<List<IElement2D>>>
            {
                Item1 = adjacencyIDs,
                Item2 = edgePair1,
                Item3 = elemPair1,
                Item4 = edgePair2,
                Item5 = elemPair2,
            };

        }

    }
}






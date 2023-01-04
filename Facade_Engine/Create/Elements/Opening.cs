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

using BH.oM.Facade.Elements;
using BH.oM.Facade.SectionProperties;
using BH.oM.Geometry;
using BH.oM.Physical.Constructions;
using BH.Engine.Geometry;
using System.Collections.Generic;
using System.Linq;
using BH.oM.Base.Attributes;
using BH.oM.Quantities.Attributes;
using System.ComponentModel;


namespace BH.Engine.Facade
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates a facade Opening from a collection of curves forming a closed loop.")]
        [Input("edges", "Closed curve defining the outline of the Opening.")]
        [Input("construction", "Construction applied to the Opening.")]
        [Input("frameEdgeProperty", "An optional FrameEdgeProperty to apply to all edges of the opening.")]
        [Input("name", "Name of the opening to be created.")]
        [Output("opening", "Created Opening.")]
        public static Opening Opening(IEnumerable<ICurve> edges, IConstruction construction = null, FrameEdgeProperty frameEdgeProperty = null, string name = "")
        {
            if(edges == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot create an opening from a null collection of edges.");
                return null;
            }

            List<ICurve> externalEdges = new List<ICurve>();
            foreach (ICurve edge in edges)
            {
                externalEdges.AddRange(edge.ISubParts());
            }

            List<PolyCurve> joined = Geometry.Compute.IJoin(edges.ToList());

            if (joined.Count == 0)
            {
                Base.Compute.RecordError("Could not join Curves. Opening not Created.");
                return null;
            }
            else if (joined.Count > 1)
            {
                Base.Compute.RecordError("Provided curves could not be joined to a single curve. Opening not created.");
                return null;
            }

            //Single joined curve
            if (joined[0].IIsClosed())
                return new Opening { Edges = externalEdges.Select(x => new FrameEdge { Curve = x, FrameEdgeProperty = frameEdgeProperty }).ToList(), OpeningConstruction = construction, Name = name };
            else
            {
                Base.Compute.RecordError("Provided curves do not form a closed loop. Could not create opening.");
                return null;
            }
            
        }

        /***************************************************/

        [Description("Creates a facade Opening from a collection of curves forming a closed loop and specified Sill, Jamb, and Head FrameEdgeProperties.")]
        [Input("edges", "Closed curve defining the outline of the Opening.")]
        [Input("headProperty", "A FrameEdgeProperty to apply to the head edge(s) of the opening.")]
        [Input("jambProperty", "A FrameEdgeProperty to apply to the jamb edge(s) of the opening.")]
        [Input("sillProperty", "A FrameEdgeProperty to apply to the sill edge(s) of the opening.")]
        [Input("construction", "Construction applied to the Opening.")]
        [Input("name", "Name of the opening to be created.")]
        [Output("opening", "Created Opening.")]
        public static Opening Opening(IEnumerable<ICurve> edges, FrameEdgeProperty headProperty = null, FrameEdgeProperty jambProperty = null, FrameEdgeProperty sillProperty = null, IConstruction construction = null, string name = "")
        {
            if (edges == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot create an opening from a null collection of edges.");
                return null;
            }

            List<ICurve> externalEdges = edges.SelectMany(x => x.ISubParts()).ToList();

            List<PolyCurve> joined = Geometry.Compute.IJoin(edges.ToList());

            if (joined.Count == 0)
            {
                Base.Compute.RecordError("Could not join Curves. Opening not created.");
                return null;
            }
            else if (joined.Count > 1)
            {
                Base.Compute.RecordError("Provided curves could not be joined to a single curve. Opening not created.");
                return null;
            }

            //Single joined curve
            if (joined[0].IIsClosed())
            {
                Opening opening = new Opening { Edges = externalEdges.Select(x => new FrameEdge { Curve = x }).ToList(), OpeningConstruction = construction, Name = name };
                foreach (FrameEdge edge in opening.Edges)
                {
                    string edgeType = edge.FrameEdgeType(opening);
                    switch (edgeType)
                    {
                        case "Head":
                            edge.FrameEdgeProperty = headProperty;
                            break;
                        case "Jamb":
                            edge.FrameEdgeProperty = jambProperty;
                            break;
                        case "Sill":
                            edge.FrameEdgeProperty = sillProperty;
                            break;
                        default:
                            break;
                    }
                }
                return opening;
            }
            else
            {
                Base.Compute.RecordError("Provided curves do not form a closed loop. Could not create opening.");
                return null;
            }

        }

        /***************************************************/
    }
}





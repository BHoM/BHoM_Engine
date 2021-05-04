/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2021, the respective contributors. All rights reserved.
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
using BH.oM.Dimensional;
using BH.oM.Physical.Constructions;
using BH.Engine.Geometry;
using BH.Engine.Physical;
using System.Collections.Generic;
using System.Linq;
using BH.oM.Reflection.Attributes;
using BH.oM.Quantities.Attributes;
using System.ComponentModel;


namespace BH.Engine.Facade
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates a facade CurtainWall from a collection of curves forming closed loops and FrameEdgeProperties to assign to the CurtainWall's edges per their type.")]
        [Input("outlines", "Closed curves defining the outlines of the Openings of the CurtainWall.")]
        [Input("constructions", "Constructions to apply to the Openings, with one construction per corresponding outline.")]
        [Input("headProperty", "A FrameEdgeProperty to apply to the head edge(s) of the openings.")]
        [Input("jambProperty", "A FrameEdgeProperty to apply to the jamb edge(s) of the openings.")]
        [Input("sillProperty", "A FrameEdgeProperty to apply to the sill edge(s) of the openings.")]
        [Input("externalHeadProperty", "A FrameEdgeProperty to apply to the head edge(s) of openings at the edge of the CurtainWall.")]
        [Input("externalJambProperty", "A FrameEdgeProperty to apply to the jamb edge(s) of openings at the edge of the CurtainWall.")]
        [Input("externalSillProperty", "A FrameEdgeProperty to apply to the sill edge(s) of openings at the edge of the CurtainWall.")]
        [Input("name", "Name of the CurtainWall to be created.")]
        [Output("curtainWall", "Created CurtainWall.")]
        public static CurtainWall CurtainWall(IEnumerable<ICurve> outlines, IEnumerable<IConstruction> constructions = null, FrameEdgeProperty headProperty = null, FrameEdgeProperty jambProperty = null, FrameEdgeProperty sillProperty = null,
                                              FrameEdgeProperty externalHeadProperty = null, FrameEdgeProperty externalJambProperty = null, FrameEdgeProperty externalSillProperty = null, string name = "")
        {
            if(outlines == null)
            {
                BH.Engine.Reflection.Compute.RecordError("Cannot create a CurtainWall from a null collection of outlines.");
                return null;
            }

            bool useConstructions = true;
            if (outlines.Count() != constructions.Count() )
            {
                BH.Engine.Reflection.Compute.RecordWarning("Outline and Construction list lengths do not match. CurtainWall will be created with no Opening Constructions applied.");
                useConstructions = false;
            }

            externalHeadProperty = externalHeadProperty ?? headProperty;
            externalJambProperty = externalJambProperty ?? jambProperty;
            externalSillProperty = externalSillProperty ?? sillProperty;

            List<Opening> openings = new List<Opening>();
            for (int i = 0; i < outlines.Count(); i++)
            {
                ICurve outline = outlines.ElementAt(i);
                if (outline.IIsClosed() != true)
                {
                    BH.Engine.Reflection.Compute.RecordError("Outline at index " + i + " was not closed and was excluded from the created CurtainWall. This method only works with closed outlineswhich each represent one opening in the CurtainWall.");
                }
                else
                {
                    IConstruction construction = useConstructions ? constructions.ElementAt(i) : null;
                    Opening opening = Create.Opening(new List<ICurve> { outline }, headProperty, jambProperty, sillProperty, construction, name + "_" + i);
                    openings.Add(opening);
                }
            }

            List<IElement1D> externalEdges = Query.ExternalEdges(openings);

            foreach (Opening opening in openings)
            {
                foreach (FrameEdge edge in opening.Edges)
                {
                    if (edge.AdjacentElements(externalEdges).Count > 0)
                    {
                        string edgeType = edge.FrameEdgeType(opening);
                        switch (edgeType)
                        {
                            case "Head":
                                edge.FrameEdgeProperty = externalHeadProperty;
                                break;
                            case "Jamb":
                                edge.FrameEdgeProperty = externalJambProperty;
                                break;
                            case "Sill":
                                edge.FrameEdgeProperty = externalSillProperty;
                                break;
                            default:
                                break;
                        }
                    }
                }
            }

            List<FrameEdge> extFrameEdges = externalEdges.OfType<ICurve>().Select(x => new FrameEdge { Curve = x }).ToList();
            CurtainWall curtainWall = new CurtainWall { ExternalEdges = extFrameEdges, Openings = openings, Name = name };

            return curtainWall;
            
        }

        /***************************************************/
    }
}



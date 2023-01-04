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

using BH.Engine.Spatial;
using BH.oM.Base.Attributes;
using BH.oM.Facade.Elements;
using BH.oM.Facade.SectionProperties;
using BH.oM.Geometry;
using BH.oM.Dimensional;
using BH.oM.Physical.Constructions;
using System;
using System.Collections.Generic;
using System.Linq;
using BH.Engine.Geometry;
using System.ComponentModel;
using BH.Engine.Analytical;

namespace BH.Engine.Facade
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates a Panel from its fundamental parts.")]
        [InputFromProperty("externalEdges")]
        [InputFromProperty("openings")]
        [InputFromProperty("construction")]
        [Input("panelType", "The type of Panel this is, eg an external wall or spandrel panel." )]
        [Input("name", "The name of the created Panel.")]
        [Output("panel", "The created Panel.")]
        public static Panel Panel(List<FrameEdge> externalEdges, List<Opening> openings = null, IConstruction construction = null, PanelType panelType = PanelType.Undefined, string name = "")
        {
            Panel panel = new Panel
            {
                ExternalEdges = externalEdges,
                Openings = openings ?? new List<Opening>(),
                Construction = construction,
                Type = panelType,
                Name = name
            };

            return panel;
        }

        /***************************************************/

        [Description("Creates a Panel from a closed curve defining the outline, any number of closed curves defining openings, and an optional frame edge property to apply to all edges.")]
        [Input("outline", "A closed Curve defining the outline of the Panel. The ExternalEdges of the Panel will be the subparts of this curve, where each edge will corespond to one curve segment.")]
        [Input("openings", "A collection of closed curves representing the openings of the Panel.")]
        [InputFromProperty("construction")]
        [Input("frameEdgeProperty", "The FrameEdgeProperty to apply to all edges of the panel.")]
        [Input("panelType", "The type of Panel this is, eg an external wall or spandrel panel.")]
        [InputFromProperty("openingConstruction")]
        [Input("openingFrameEdgeProperty", "The FrameEdgeProperty to apply to all edges of the openings.")]
        [Input("name", "The name of the created Panel.")]
        [Output("panel","The created Panel.")]
        public static Panel Panel(ICurve outline, List<ICurve> openings = null, IConstruction construction = null, FrameEdgeProperty frameEdgeProperty = null, PanelType panelType = PanelType.Undefined, IConstruction openingConstruction = null, FrameEdgeProperty openingFrameEdgeProperty = null, string name = "")
        {
            if (outline == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot create a panel from a null outline.");
                return null;
            }
            
            if (!outline.IIsClosed())
            {
                Base.Compute.RecordError("Outline not closed. Could not create Panel.");
                return null;
            }
            List<Opening> pOpenings = openings != null ? openings.Select(o => Create.Opening(new List<ICurve> {o}, openingConstruction, openingFrameEdgeProperty)).Where(x => x != null).ToList() : new List<Opening>();
            List<FrameEdge> externalEdges = outline.ISubParts().Select(x => new FrameEdge { Curve = x, FrameEdgeProperty = frameEdgeProperty }).ToList();

            return Create.Panel(externalEdges, pOpenings, construction, panelType, name);
        }

        /***************************************************/

        [Description("Creates a Panel from a PlanarSurface, creating external edges from the ExternalBoundary and openings from the InternalBoundaries of the PlanarSurface.")]
        [Input("surface", "A planar surface used to define the geometry of the panel, i.e. the external edges and the openings.")]
        [InputFromProperty("construction")]
        [Input("panelType", "The type of Panel this is, eg an external wall or spandrel panel.")]
        [Input("name", "The name of the created Panel.")]
        [Output("panel", "The created Panel.")]
        public static Panel Panel(PlanarSurface surface, Construction construction = null, FrameEdgeProperty frameEdgeProperty = null, PanelType panelType = PanelType.Undefined, IConstruction openingConstruction = null, FrameEdgeProperty openingFrameEdgeProperty = null, string name = "")
        {
            if(surface == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot create a panel from a null surface.");
                return null;
            }

            return Panel(surface.ExternalBoundary, surface.InternalBoundaries.ToList(), construction, frameEdgeProperty, panelType, openingConstruction, openingFrameEdgeProperty, name);
        }

        /***************************************************/
    }
}






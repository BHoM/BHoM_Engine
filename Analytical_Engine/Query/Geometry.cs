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


using BH.Engine.Geometry;
using BH.oM.Geometry;
using BH.oM.Analytical.Elements;
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using BH.oM.Reflection.Attributes;
using BH.oM.Base;

namespace BH.Engine.Analytical
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Gets the geometry of a Node as a Point. Method required for automatic display in UI packages.")]
        [Input("node", "Node to get the Point from.")]
        [Output("point", "The geometry of the Node.")]
        public static Point Geometry(this INode node)
        {
            return node.Position;
        }

        /***************************************************/

        [Description("Gets the geometry of a Bar as its centreline. Method required for automatic display in UI packages.")]
        [Input("bar", "Bar to get the centreline geometry from.")]
        [Output("line", "The geometry of the Bar as its centreline.")]
        public static Line Geometry<TNode>(this ILink<TNode> link)
            where TNode : INode
        {
            return new Line { Start = link.StartNode.Position, End = link.EndNode.Position };
        }

        /***************************************************/

        [Description("Gets the geometry of a Edge as its Curve. Method required for automatic display in UI packages.")]
        [Input("edge", "Edge to get the curve geometry from.")]
        [Output("curve", "The geometry of the Edge as its Curve.")]
        public static ICurve Geometry(this IEdge edge)
        {
            return edge.Curve;
        }

        /***************************************************/

        [Description("Gets the geometry of a analytical Panel at its centre. Method required for automatic display in UI packages.")]
        [Input("panel", "Panel to get the planar surface geometry from.")]
        [Output("surface", "The geometry of the analytical Panel at its centre.")]
        public static PlanarSurface Geometry<TEdge, TOpening>(this IPanel<TEdge, TOpening> panel)
            where TEdge : IEdge
            where TOpening : IOpening<TEdge>
        {
            return new PlanarSurface(
                    Engine.Geometry.Compute.IJoin(panel.ExternalEdges.Select(x => x.Curve).ToList()).FirstOrDefault(),
                    panel.Openings.SelectMany(x => Engine.Geometry.Compute.IJoin(x.Edges.Select(y => y.Curve).ToList())).Cast<ICurve>().ToList());
        }

        /***************************************************/
    }
}

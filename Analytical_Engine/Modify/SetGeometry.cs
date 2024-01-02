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

using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using BH.oM.Base.Attributes;
using BH.oM.Analytical.Elements;
using BH.oM.Geometry;
using BH.Engine.Geometry;
using BH.Engine.Base;

namespace BH.Engine.Analytical
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Updates the position of a INode.")]
        [Input("node", "The INode to set the postion to.")]
        [Input("point", "The new position of the INode.")]
        [Output("node", "The INode with updated geometry.")]
        public static INode SetGeometry(this INode node, Point point)
        {
            if(node == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot set the geometry of a null node.");
                return null;
            }

            INode clone = node.ShallowClone();
            clone.Position = point.DeepClone();
            return clone;
        }

        /***************************************************/

        [PreviousInputNames("link", "bar")]
        [Description("Updates geometry of an ILink by updating the positions of its end Nodes.")]
        [Input("link", "The ILink to update.")]
        [Input("curve", "The new centreline curve of the ILink. Should be a linear curve. \n" +
                        "The start point of the curve will be used to set the position of the StartNode and the end point to set the position of the EndNode.")]
        [Output("link", "The ILink with updated geometry.")]
        public static ILink<TNode> SetGeometry<TNode>(this ILink<TNode> link, ICurve curve)
            where TNode : INode
        {
            if (!curve.IIsLinear())
            {
                Base.Compute.RecordError("The curve used to set the geometry of a Bar needs to be linear.");
                return null;
            }

            ILink<TNode> clone = link.ShallowClone();
            clone.StartNode = (TNode)clone.StartNode.SetGeometry(curve.IStartPoint());
            clone.EndNode = (TNode)clone.EndNode.SetGeometry(curve.IEndPoint());
            return clone;
        }

        /***************************************************/

        [Description("Updates the curve geometry of an IEdge.")]
        [Input("edge", "The IEdge to update.")]
        [Input("curve", "The curve to set to the IEdge.")]
        [Output("edge", "The IEdge with updated geometry.")]
        public static IEdge SetGeometry(this IEdge edge, ICurve curve)
        {
            if(edge == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot set the geometry of a null edge.");
                return null;
            }

            IEdge clone = edge.ShallowClone();
            clone.Curve = curve.DeepClone();
            return clone;
        }

        /***************************************************/

        [PreviousInputNames("anaSurface", "strSurface")]
        [Description("Updates the geometrical ISurface of a analytical ISurface.")]
        [Input("anaSurface", "The analytical Surface to update.")]
        [Input("geoSurface", "The geometrical ISurface to set to the structural Surface.")]
        [Output("strSurface", "The analytical Surface with updated geometry.")]
        public static BH.oM.Analytical.Elements.ISurface SetGeometry(this BH.oM.Analytical.Elements.ISurface anaSurface, BH.oM.Geometry.ISurface geoSurface)
        {
            if(anaSurface == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot set the geometry of a null surface.");
                return null;
            }

            anaSurface.Extents = geoSurface;
            return anaSurface;
        }

        /***************************************************/
    }
}





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
using BH.oM.Geometry.CoordinateSystem;
using BH.oM.Structure.Elements;
using BH.oM.Reflection.Attributes;
using BH.oM.Quantities.Attributes;
using System.ComponentModel;


namespace BH.Engine.Structure
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Updates the position of a Node.")]
        [Input("node", "The Node to set the postion to.")]
        [Input("point", "The new position of the Node.")]
        [Output("node", "The Node with updated geometry.")]
        public static Node SetGeometry(this Node node, Point point)
        {
            Node clone = node.GetShallowClone(true) as Node;
            clone.Position = point.Clone();
            return clone;
        }

        /***************************************************/

        [Description("Updates geometry of a Bar by updating the positions of its end Nodes.")]
        [Input("bar", "The Bar to update.")]
        [Input("curve", "The new centreline curve of the Bar. Should be a linear curve. \n" +
                        "The start point of the curve will be used to set the position of the StartNode and the end point to set the position of the EndNode.")]
        [Output("bar", "The Bar with updated geometry.")]
        public static Bar SetGeometry(this Bar bar, ICurve curve)
        {
            if (!curve.IIsLinear())
            {
                Reflection.Compute.RecordError("The curve used to set the geometry of a Bar needs to be linear.");
                return null;
            }

            Bar clone = bar.GetShallowClone(true) as Bar;
            clone.StartNode = clone.StartNode.SetGeometry(curve.IStartPoint());
            clone.EndNode = clone.EndNode.SetGeometry(curve.IEndPoint());
            return clone;
        }

        /***************************************************/

        [Description("Updates the curve geometry of an Edge.")]
        [Input("edge", "The Edge to update.")]
        [Input("curve", "The curve to set to the Edge.")]
        [Output("edge", "The Edge with updated geometry.")]
        public static Edge SetGeometry(this Edge edge, ICurve curve)
        {
            Edge clone = edge.GetShallowClone(true) as Edge;
            clone.Curve = curve.IClone();
            return clone;
        }

        /***************************************************/

        [Description("Updates the geometrical ISurface of a structural Surface.")]
        [Input("strSurface", "The structural Surface to update.")]
        [Input("geoSurface", "The geometrical ISurface to set to the structural Surface.")]
        [Output("strSurface", "The structural Surface with updated geometry.")]
        public static Surface SetGeometry(this Surface strSurface, ISurface geoSurface)
        {
            Surface clone = strSurface.GetShallowClone(true) as Surface;
            clone.Extents = geoSurface.IClone() as ISurface;
            return clone;
        }

        /***************************************************/
    }
}


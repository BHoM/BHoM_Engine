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
using BH.oM.Environment.Elements;
using BH.oM.Environment;
using BH.oM.Geometry;

using BH.oM.Reflection.Attributes;
using System.ComponentModel;
using BH.Engine.Base;
using BH.oM.Environment.Analysis;

namespace BH.Engine.Environment
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Updates the position of a Node")]
        [Input("node", "The Node to set the postion to")]
        [Input("point", "The new position of the Node")]
        [Output("node", "The Node with updated geometry")]
        public static Node SetGeometry(this Node node, Point point)
        {
            Node clone = node.ShallowClone(true);
            clone.Position = point.DeepClone();
            return clone;
        }

        [Description("Assign a new ICurve boundary to a generic Environment Object")]
        [Input("environmentObject", "Any object implementing the IEnvironmentObject interface that can have its geometry changed")]
        [Input("curve", "Any object implementing the ICurve interface from BHoM Geometry Curves")]
        [Output("environmentObject", "The environment object with an updated external boundary")]
        public static IEnvironmentObject ISetGeometry(this IEnvironmentObject environmentObject, ICurve curve)
        { 
            return SetGeometry(environmentObject as dynamic, curve as dynamic);
        }

        [Description("Assign a new ICurve external boundary to an Environment Panel")]
        [Input("panel", "An Environment Panel to set the external boundary of")]
        [Input("curve", "Any object implementing the ICurve interface from BHoM Geometry Curves")]
        [Output("panel", "An Environment Panel with an updated external boundary")]
        public static Panel SetGeometry(this Panel panel, ICurve curve)
        {
            Panel aPanel = panel.ShallowClone();
            aPanel.ExternalEdges = curve.DeepClone().ToEdges();
            return aPanel;
        }

        [Description("Assign a new ICurve external boundary to an Environment Opening")]
        [Input("opening", "An Environment Opening to set the external boundary of")]
        [Input("curve", "Any object implementing the ICurve interface from BHoM Geometry Curves")]
        [Output("opening", "An Environment Opening with an updated external boundary")]
        public static Opening SetGeometry(this Opening opening, ICurve curve)
        {
            Opening aOpening = opening.ShallowClone();
            aOpening.Edges = curve.DeepClone().ToEdges();
            return aOpening;
        }

        [Description("Assign a new ICurve definition to an Environment Edge")]
        [Input("edge", "An Environment Edge to set the geometry of")]
        [Input("curve", "Any object implementing the ICurve interface from BHoM Geometry Curves")]
        [Output("edge", "An Environment Edge with an updated geometry")]
        public static Edge SetGeometry(this Edge edge, ICurve curve)
        {
            Edge clone = edge.ShallowClone();
            clone.Curve = curve.DeepClone();
            return clone;
        }

        [Description("Assign a new locaion point to an Environment Space")]
        [Input("space", "An Environment Space to set the geometry of")]
        [Input("locationPoint", "A BHoM Geometry Point defining the location of the space")]
        [Output("space", "An Environment Space with an updated geometry")]
        public static Space SetGeometry(this Space space, Point locationPoint)
        {
            Space clone = space.DeepClone<Space>();
            clone.Location = locationPoint;
            return clone;
        }
    }
}


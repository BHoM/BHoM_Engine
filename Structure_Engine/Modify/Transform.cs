﻿/*
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

using BH.Engine.Geometry;
using BH.Engine.Spatial;
using BH.oM.Geometry;
using BH.oM.Reflection.Attributes;
using BH.oM.Structure.Elements;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.Structure
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Interface Methods - IElements             ****/
        /***************************************************/

        [Description("Transforms the Node's position and orientation by the transform matrix. Only rigid body transformations are supported.")]
        [Input("node", "Node to transform.")]
        [Input("transform", "Transform matrix.")]
        [Output("transformed", "Modified Node with unchanged properties, but transformed position and orientation.")]
        public static Node Transform(this Node node, TransformMatrix transform)
        {
            if (!transform.IsRigidTransformation())
            {
                BH.Engine.Reflection.Compute.RecordError("Transformation failed: only rigid body transformations are currently supported.");
                return null;
            }

            Node result = node.GetShallowClone() as Node;
            result.Position = result.Position.Transform(transform);
            result.Orientation = result.Orientation.Transform(transform);
            return result;
        }

        /***************************************************/
        
        [Description("Transforms the Bar's nodes and rotation by the transform matrix. Only rigid body transformations are supported.")]
        [Input("bar", "Bar to transform.")]
        [Input("transform", "Transform matrix.")]
        [Output("transformed", "Modified Bar with unchanged properties, but transformed nodes and rotation.")]
        public static Bar Transform(this Bar bar, TransformMatrix transform)
        {
            if (!transform.IsRigidTransformation())
            {
                BH.Engine.Reflection.Compute.RecordError("Transformation failed: only rigid body transformations are currently supported.");
                return null;
            }

            Bar result = bar.GetShallowClone() as Bar;
            result.StartNode = result.StartNode.Transform(transform);
            result.EndNode = result.EndNode.Transform(transform);
            
            Vector normalBefore = new Line { Start = bar.StartNode.Position, End = bar.EndNode.Position }.ElementNormal(bar.OrientationAngle);
            Vector normalAfter = normalBefore.Transform(transform);
            result.OrientationAngle = normalAfter.OrientationAngleLinear(new Line { Start = result.StartNode.Position, End = result.EndNode.Position });

            return result;
        }

        /***************************************************/

        [Description("Transforms the RigidLink's primary and secondary nodes by the transform matrix. Only rigid body transformations are supported.")]
        [Input("link", "RigidLink to transform.")]
        [Input("transform", "Transform matrix.")]
        [Output("transformed", "Modified RigidLink with unchanged properties, but transformed primary and secondary nodes.")]
        public static RigidLink Transform(this RigidLink link, TransformMatrix transform)
        {
            if (!transform.IsRigidTransformation())
            {
                BH.Engine.Reflection.Compute.RecordError("Transformation failed: only rigid body transformations are currently supported.");
                return null;
            }

            RigidLink result = link.GetShallowClone() as RigidLink;
            result.PrimaryNode = result.PrimaryNode.Transform(transform);
            result.SecondaryNodes = result.SecondaryNodes.Select(x => x.Transform(transform)).ToList();
            return result;
        }

        /***************************************************/

        [Description("Transforms the Edge's location by the transform matrix. Only rigid body transformations are supported.")]
        [Input("edge", "Edge to transform.")]
        [Input("transform", "Transform matrix.")]
        [Output("transformed", "Modified Edge with unchanged properties, but transformed location.")]
        public static Edge Transform(this Edge edge, TransformMatrix transform)
        {
            if (!transform.IsRigidTransformation())
            {
                BH.Engine.Reflection.Compute.RecordError("Transformation failed: only rigid body transformations are currently supported.");
                return null;
            }

            Edge result = edge.GetShallowClone() as Edge;
            result.Curve = result.Curve.ITransform(transform);
            return result;
        }

        /***************************************************/

        [Description("Transforms the Opening's edges by the transform matrix. Only rigid body transformations are supported.")]
        [Input("opening", "Opening to transform.")]
        [Input("transform", "Transform matrix.")]
        [Output("transformed", "Modified Opening with unchanged properties, but transformed edges.")]
        public static Opening Transform(this Opening opening, TransformMatrix transform)
        {
            if (!transform.IsRigidTransformation())
            {
                BH.Engine.Reflection.Compute.RecordError("Transformation failed: only rigid body transformations are currently supported.");
                return null;
            }

            Opening result = opening.GetShallowClone() as Opening;
            result.Edges = result.Edges.Select(x => x.Transform(transform)).ToList();
            return result;
        }

        /***************************************************/

        [Description("Transforms the Panel's edges, openings and orientation angle by the transform matrix. Only rigid body transformations are supported.")]
        [Input("panel", "Panel to transform.")]
        [Input("transform", "Transform matrix.")]
        [Output("transformed", "Modified Panel with unchanged properties, but transformed edges, openings and orientation angle.")]
        public static Panel Transform(this Panel panel, TransformMatrix transform)
        {
            if (!transform.IsRigidTransformation())
            {
                BH.Engine.Reflection.Compute.RecordError("Transformation failed: only rigid body transformations are currently supported.");
                return null;
            }

            Panel result = panel.GetShallowClone() as Panel;
            result.ExternalEdges = result.ExternalEdges.Select(x => x.Transform(transform)).ToList();
            result.Openings = result.Openings.Select(x => x.Transform(transform)).ToList();

            Basis orientation = panel.LocalOrientation().Transform(transform);
            result.OrientationAngle = orientation.Z.OrientationAngleAreaElement(orientation.X);

            return result;
        }

        /***************************************************/

        [Description("Transforms the FEMesh's nodes by the transform matrix. Only rigid body transformations are supported.")]
        [Input("mesh", "FEMesh to transform.")]
        [Input("transform", "Transform matrix.")]
        [Output("transformed", "Modified FEMesh with unchanged properties, but transformed nodes.")]
        public static FEMesh Transform(this FEMesh mesh, TransformMatrix transform)
        {
            if (!transform.IsRigidTransformation())
            {
                BH.Engine.Reflection.Compute.RecordError("Transformation failed: only rigid body transformations are currently supported.");
                return null;
            }

            FEMesh result = mesh.GetShallowClone() as FEMesh;
            result.Nodes = result.Nodes.Select(x => x.Transform(transform)).ToList();

            List<Basis> orientationsBefore = mesh.LocalOrientations();
            result.Faces = new List<FEMeshFace>(mesh.Faces);
            for (int i = 0; i < orientationsBefore.Count; i++)
            {
                result.Faces[i] = result.Faces[i].SetLocalOrientation(result, orientationsBefore[i].Transform(transform).X);
            }

            return result;
        }

        /***************************************************/
    }
}



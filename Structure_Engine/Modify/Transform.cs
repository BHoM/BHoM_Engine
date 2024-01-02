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

using BH.Engine.Base;
using BH.Engine.Geometry;
using BH.Engine.Spatial;
using BH.oM.Geometry;
using BH.oM.Base.Attributes;
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
        [Input("tolerance", "Tolerance used in the check whether the input matrix is equivalent to the rigid body transformation.")]
        [Output("transformed", "Modified Node with unchanged properties, but transformed position and orientation.")]
        public static Node Transform(this Node node, TransformMatrix transform, double tolerance = Tolerance.Distance)
        {
            if (node == null)
            {
                BH.Engine.Base.Compute.RecordError("Could not transform the Node because it was null.");
                return null;
            }

            if (!transform.IsRigidTransformation(tolerance))
            {
                BH.Engine.Base.Compute.RecordError("Transformation failed: only rigid body transformations are currently supported.");
                return null;
            }

            Node result = node.ShallowClone();
            result.Position = result.Position.Transform(transform);
            result.Orientation = result.Orientation?.Transform(transform);
            return result;
        }

        /***************************************************/

        [Description("Transforms the Bar's nodes and orientation by the transform matrix. Only rigid body transformations are supported.")]
        [Input("bar", "Bar to transform.")]
        [Input("transform", "Transform matrix.")]
        [Input("tolerance", "Tolerance used in the check whether the input matrix is equivalent to the rigid body transformation.")]
        [Output("transformed", "Modified Bar with unchanged properties, but transformed nodes and orientation.")]
        public static Bar Transform(this Bar bar, TransformMatrix transform, double tolerance = Tolerance.Distance)
        {
            if (bar == null)
            {
                BH.Engine.Base.Compute.RecordError("Could not transform the Bar because it was null.");
                return null;
            }

            if (bar.StartNode?.Position == null || bar.EndNode?.Position == null)
            {
                BH.Engine.Base.Compute.RecordWarning("The bar could not be transformed because at least one of its nodes (or their location) is null.");
                return bar;
            }

            if (!transform.IsRigidTransformation(tolerance))
            {
                BH.Engine.Base.Compute.RecordError("Transformation failed: only rigid body transformations are currently supported.");
                return null;
            }

            Bar result = bar.ShallowClone();
            result.StartNode = result.StartNode.Transform(transform, tolerance);
            result.EndNode = result.EndNode.Transform(transform, tolerance);

            Vector normalBefore = new Line { Start = bar.StartNode.Position, End = bar.EndNode.Position }.ElementNormal(bar.OrientationAngle);
            Vector normalAfter = normalBefore.Transform(transform);
            result.OrientationAngle = normalAfter.OrientationAngleLinear(new Line { Start = result.StartNode.Position, End = result.EndNode.Position });

            return result;
        }

        /***************************************************/

        [Description("Transforms the RigidLink's primary and secondary nodes by the transform matrix. Only rigid body transformations are supported.")]
        [Input("link", "RigidLink to transform.")]
        [Input("transform", "Transform matrix.")]
        [Input("tolerance", "Tolerance used in the check whether the input matrix is equivalent to the rigid body transformation.")]
        [Output("transformed", "Modified RigidLink with unchanged properties, but transformed primary and secondary nodes.")]
        public static RigidLink Transform(this RigidLink link, TransformMatrix transform, double tolerance = Tolerance.Distance)
        {
            if (link == null)
            {
                BH.Engine.Base.Compute.RecordError("Could not transform the RigidLink because it was null.");
                return null;
            }

            if (!transform.IsRigidTransformation(tolerance))
            {
                BH.Engine.Base.Compute.RecordError("Transformation failed: only rigid body transformations are currently supported.");
                return null;
            }

            RigidLink result = link.ShallowClone();
            result.PrimaryNode = result.PrimaryNode.Transform(transform, tolerance);
            result.SecondaryNodes = result.SecondaryNodes.Select(x => x.Transform(transform, tolerance)).ToList();
            return result;
        }

        /***************************************************/

        [Description("Transforms the Edge's location by the transform matrix. Only rigid body transformations are supported.")]
        [Input("edge", "Edge to transform.")]
        [Input("transform", "Transform matrix.")]
        [Input("tolerance", "Tolerance used in the check whether the input matrix is equivalent to the rigid body transformation.")]
        [Output("transformed", "Modified Edge with unchanged properties, but transformed location.")]
        public static Edge Transform(this Edge edge, TransformMatrix transform, double tolerance = Tolerance.Distance)
        {
            if (edge == null)
            {
                BH.Engine.Base.Compute.RecordError("Could not transform the Edge because it was null.");
                return null;
            }

            if (!transform.IsRigidTransformation(tolerance))
            {
                BH.Engine.Base.Compute.RecordError("Transformation failed: only rigid body transformations are currently supported.");
                return null;
            }

            Edge result = edge.ShallowClone();
            result.Curve = result.Curve.ITransform(transform);
            return result;
        }

        /***************************************************/

        [Description("Transforms the Opening's edges by the transform matrix. Only rigid body transformations are supported.")]
        [Input("opening", "Opening to transform.")]
        [Input("transform", "Transform matrix.")]
        [Input("tolerance", "Tolerance used in the check whether the input matrix is equivalent to the rigid body transformation.")]
        [Output("transformed", "Modified Opening with unchanged properties, but transformed edges.")]
        public static Opening Transform(this Opening opening, TransformMatrix transform, double tolerance = Tolerance.Distance)
        {
            if (opening == null)
            {
                BH.Engine.Base.Compute.RecordError("Could not transform the Opening because it was null.");
                return null;
            }

            if (!transform.IsRigidTransformation(tolerance))
            {
                BH.Engine.Base.Compute.RecordError("Transformation failed: only rigid body transformations are currently supported.");
                return null;
            }

            Opening result = opening.ShallowClone();
            result.Edges = result.Edges.Select(x => x.Transform(transform, tolerance)).ToList();
            return result;
        }

        /***************************************************/

        [Description("Transforms the Panel's edges, openings and orientation angle by the transform matrix. Only rigid body transformations are supported.")]
        [Input("panel", "Panel to transform.")]
        [Input("transform", "Transform matrix.")]
        [Input("tolerance", "Tolerance used in the check whether the input matrix is equivalent to the rigid body transformation.")]
        [Output("transformed", "Modified Panel with unchanged properties, but transformed edges, openings and orientation angle.")]
        public static Panel Transform(this Panel panel, TransformMatrix transform, double tolerance = Tolerance.Distance)
        {
            if (panel == null)
            {
                BH.Engine.Base.Compute.RecordError("Could not transform the Panel because it was null.");
                return null;
            }

            if (!transform.IsRigidTransformation(tolerance))
            {
                BH.Engine.Base.Compute.RecordError("Transformation failed: only rigid body transformations are currently supported.");
                return null;
            }

            Panel result = panel.ShallowClone();
            result.ExternalEdges = result.ExternalEdges.Select(x => x.Transform(transform, tolerance)).ToList();
            result.Openings = result.Openings.Select(x => x.Transform(transform, tolerance)).ToList();

            Basis orientation = panel.LocalOrientation()?.Transform(transform);
            if (orientation != null)
                result.OrientationAngle = orientation.Z.OrientationAngleAreaElement(orientation.X);
            else
                BH.Engine.Base.Compute.RecordWarning("Local orientation of the panel could not be transformed. Please note that the orientation of the resultant panel may be incorrect.");

            return result;
        }

        /***************************************************/

        [Description("Transforms the FEMesh's nodes by the transform matrix. Only rigid body transformations are supported.")]
        [Input("mesh", "FEMesh to transform.")]
        [Input("transform", "Transform matrix.")]
        [Input("tolerance", "Tolerance used in the check whether the input matrix is equivalent to the rigid body transformation.")]
        [Output("transformed", "Modified FEMesh with unchanged properties, but transformed nodes.")]
        public static FEMesh Transform(this FEMesh mesh, TransformMatrix transform, double tolerance = Tolerance.Distance)
        {
            if (mesh == null)
            {
                BH.Engine.Base.Compute.RecordError("Could not transform the FEMesh because it was null.");
                return null;
            }

            if (!transform.IsRigidTransformation(tolerance))
            {
                BH.Engine.Base.Compute.RecordError("Transformation failed: only rigid body transformations are currently supported.");
                return null;
            }

            FEMesh result = mesh.ShallowClone();
            result.Nodes = result.Nodes.Select(x => x.Transform(transform, tolerance)).ToList();

            List<Basis> orientationsBefore = mesh.LocalOrientations();
            result.Faces = new List<FEMeshFace>(mesh.Faces);
            for (int i = 0; i < orientationsBefore.Count; i++)
            {
                result.Faces[i] = result.Faces[i].SetLocalOrientation(result, orientationsBefore[i].Transform(transform).X);
            }

            return result;
        }

        /***************************************************/

        [Description("Transforms the PadFoundation's edges, openings and basis by the transform matrix. Only rigid body transformations are supported.")]
        [Input("padFoundation", "PadFoundation to transform.")]
        [Input("transform", "Transform matrix.")]
        [Input("tolerance", "Tolerance used in the check whether the input matrix is equivalent to the rigid body transformation.")]
        [Output("transformed", "Modified PadFoundation with unchanged properties, but transformed edges and basis.")]
        public static PadFoundation Transform(this PadFoundation padFoundation, TransformMatrix transform, double tolerance = Tolerance.Distance)
        {
            if (padFoundation.IsNull())
                return padFoundation;

            if (transform.IsNull())
                return padFoundation;

            if (!transform.IsRigidTransformation(tolerance))
            {
                Base.Compute.RecordError("Transformation failed: only rigid body transformations are currently supported.");
                return null;
            }

            PadFoundation result = padFoundation.ShallowClone();

            result.TopOutline = result.TopOutline.Transform(transform);

            Basis orientation = padFoundation.LocalOrientation()?.Transform(transform);
            if (orientation != null)
                result.OrientationAngle = orientation.Z.OrientationAngleAreaElement(orientation.X);
            else
                BH.Engine.Base.Compute.RecordWarning("Local orientation of the PadFoundation could not be transformed. Please note that the orientation of the resultant panel may be incorrect.");

            return result;
        }

        /***************************************************/

        [Description("Transforms the Pile by the transform matrix. Only rigid body transformations are supported.")]
        [Input("pile", "Pile to transform.")]
        [Input("transform", "Transform matrix.")]
        [Input("tolerance", "Tolerance used in the check whether the input matrix is equivalent to the rigid body transformation.")]
        [Output("transformed", "Modified Pile with unchanged properties, but transformed.")]
        public static Pile Transform(this Pile pile, TransformMatrix transform, double tolerance = Tolerance.Distance)
        {
            if (pile.IsNull())
                return pile;

            if (transform.IsNull())
                return pile;

            if (!transform.IsRigidTransformation(tolerance))
            {
                Base.Compute.RecordError("Transformation failed: only rigid body transformations are currently supported.");
                return null;
            }

            Pile result = pile.ShallowClone();
            result.TopNode = result.TopNode.Transform(transform, tolerance);
            result.BottomNode = result.BottomNode.Transform(transform, tolerance);

            Vector normalBefore = new Line { Start = pile.TopNode.Position, End = pile.BottomNode.Position }.ElementNormal(pile.OrientationAngle);
            Vector normalAfter = normalBefore.Transform(transform);
            result.OrientationAngle = normalAfter.OrientationAngleLinear(new Line { Start = result.TopNode.Position, End = result.BottomNode.Position });

            return result;
        }

        /***************************************************/

        [Description("Transforms the PileFoundation by the transform matrix. Only rigid body transformations are supported.")]
        [Input("pileFoundation", "PileFoundation to transform.")]
        [Input("transform", "Transform matrix.")]
        [Input("tolerance", "Tolerance used in the check whether the input matrix is equivalent to the rigid body transformation.")]
        [Output("transformed", "Modified PileFoundation with unchanged properties, but transformed.")]
        public static PileFoundation Transform(this PileFoundation pileFoundation, TransformMatrix transform, double tolerance = Tolerance.Distance)
        {
            if (pileFoundation.IsNull())
                return pileFoundation;

            if (transform.IsNull())
                return pileFoundation;

            if (!transform.IsRigidTransformation(tolerance))
            {
                Base.Compute.RecordError("Transformation failed: only rigid body transformations are currently supported.");
                return null;
            }

            PileFoundation result = pileFoundation.ShallowClone();

            result.PileCap = result.PileCap.Transform(transform, tolerance);
            result.Piles = result.Piles.Select(x => x.Transform(transform, tolerance)).ToList();

            return result;
        }

        /***************************************************/

    }
}





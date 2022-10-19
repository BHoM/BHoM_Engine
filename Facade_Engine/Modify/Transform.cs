/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2022, the respective contributors. All rights reserved.
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
using BH.oM.Facade.Elements;
using BH.oM.Geometry;
using BH.oM.Physical.FramingProperties;
using BH.oM.Base.Attributes;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.Facade
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Transforms the FrameEdge's location curve and profile orientations by the transform matrix. Only rigid body transformations are supported.")]
        [Input("edge", "FrameEdge to transform.")]
        [Input("transform", "Transform matrix.")]
        [Input("tolerance", "Tolerance used in the check whether the input matrix is equivalent to the rigid body transformation.")]
        [Output("transformed", "Modified FrameEdge with unchanged properties, but transformed location curve and profile orientations.")]
        public static FrameEdge Transform(this FrameEdge edge, TransformMatrix transform, double tolerance = Tolerance.Distance)
        {
            if (!transform.IsRigidTransformation(tolerance))
            {
                BH.Engine.Base.Compute.RecordError("Transformation failed: only rigid body transformations are currently supported.");
                return null;
            }

            FrameEdge result = edge.ShallowClone();
            result.Curve = result.Curve.ITransform(transform);

            if (edge.FrameEdgeProperty != null)
            {
                if (edge.Curve is Line)
                {
                    Line lineBefore = (Line)edge.Curve;
                    Line lineAfter = (Line)result.Curve;

                    result.FrameEdgeProperty = edge.FrameEdgeProperty.ShallowClone();
                    List<ConstantFramingProperty> newProperties = new List<ConstantFramingProperty>();
                    foreach (ConstantFramingProperty property in edge.FrameEdgeProperty.SectionProperties)
                    {
                        ConstantFramingProperty newProperty = property.ShallowClone();
                        Vector normalBefore = lineBefore.ElementNormal(property.OrientationAngle);
                        Vector normalAfter = normalBefore.Transform(transform);
                        newProperty.OrientationAngle = normalAfter.OrientationAngleLinear(lineAfter);
                        newProperties.Add(newProperty);
                    }

                    result.FrameEdgeProperty.SectionProperties = newProperties;
                }
                else if (!edge.Curve.IIsPlanar(tolerance))
                    BH.Engine.Base.Compute.RecordWarning($"The element's location is a nonlinear, nonplanar curve. Correctness of orientation angle after transform could not be ensured in 100%. BHoM_Guid: {edge.BHoM_Guid}");
            }

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

        [Description("Transforms the Panel's edges and openings by the transform matrix. Only rigid body transformations are supported.")]
        [Input("panel", "Panel to transform.")]
        [Input("transform", "Transform matrix.")]
        [Input("tolerance", "Tolerance used in the check whether the input matrix is equivalent to the rigid body transformation.")]
        [Output("transformed", "Modified Panel with unchanged properties, but transformed edges and openings.")]
        public static Panel Transform(this Panel panel, TransformMatrix transform, double tolerance = Tolerance.Distance)
        {
            if (!transform.IsRigidTransformation(tolerance))
            {
                BH.Engine.Base.Compute.RecordError("Transformation failed: only rigid body transformations are currently supported.");
                return null;
            }

            Panel result = panel.ShallowClone();
            result.ExternalEdges = result.ExternalEdges.Select(x => x.Transform(transform, tolerance)).ToList();
            result.Openings = result.Openings.Select(x => x.Transform(transform, tolerance)).ToList();

            return result;
        }

        /***************************************************/

        [Description("Transforms the CurtainWall's edges and openings by the transform matrix. Only rigid body transformations are supported.")]
        [Input("wall", "CurtainWall to transform.")]
        [Input("transform", "Transform matrix.")]
        [Input("tolerance", "Tolerance used in the check whether the input matrix is equivalent to the rigid body transformation.")]
        [Output("transformed", "Modified CurtainWall with unchanged properties, but transformed edges and openings.")]
        public static CurtainWall Transform(this CurtainWall wall, TransformMatrix transform, double tolerance = Tolerance.Distance)
        {
            if (!transform.IsRigidTransformation(tolerance))
            {
                BH.Engine.Base.Compute.RecordError("Transformation failed: only rigid body transformations are currently supported.");
                return null;
            }

            CurtainWall result = wall.ShallowClone();
            result.ExternalEdges = result.ExternalEdges.Select(x => x.Transform(transform, tolerance)).ToList();
            result.Openings = result.Openings.Select(x => x.Transform(transform, tolerance)).ToList();

            return result;
        }

        /***************************************************/
    }
}



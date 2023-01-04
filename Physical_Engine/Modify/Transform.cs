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

using System.Collections.Generic;
using BH.oM.Dimensional;
using BH.oM.Geometry;
using System.ComponentModel;
using BH.oM.Base.Attributes;
using BH.Engine.Geometry;
using BH.oM.Physical.Elements;
using System.Linq;
using BH.oM.Physical.FramingProperties;
using BH.Engine.Spatial;
using BH.Engine.Base;

namespace BH.Engine.Physical
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Interface Methods - IElements             ****/
        /***************************************************/

        [Description("Transforms the IFramingElement's location and orientation angle by the transform matrix. Only rigid body transformations are supported.")]
        [Input("framingElement", "IFramingElement to transform.")]
        [Input("transform", "Transform matrix.")]
        [Input("tolerance", "Tolerance used in the check whether the input matrix is equivalent to the rigid body transformation.")]
        [Output("transformed", "Modified IFramingElement with unchanged properties, but transformed location and orientation angle.")]
        public static IFramingElement Transform(this IFramingElement framingElement, TransformMatrix transform, double tolerance = Tolerance.Distance)
        {
            if (!transform.IsRigidTransformation(tolerance))
            {
                BH.Engine.Base.Compute.RecordError("Transformation failed: only rigid body transformations are currently supported.");
                return null;
            }

            IFramingElement result = framingElement.ShallowClone();
            result.Location = result.Location.ITransform(transform);

            ConstantFramingProperty property = result.Property as ConstantFramingProperty;
            if (property == null)
                BH.Engine.Base.Compute.RecordWarning($"Orientation angle of the IFramingElement has not been transformed because its property is not ConstantFramingProperty. BHoM_Guid: {framingElement.BHoM_Guid}");
            else
            {
                if (framingElement.Location is Line)
                {
                    ConstantFramingProperty newProperty = property.ShallowClone();
                    Vector normalBefore = ((Line)framingElement.Location).ElementNormal(property.OrientationAngle);
                    Vector normalAfter = normalBefore.Transform(transform);
                    newProperty.OrientationAngle = normalAfter.OrientationAngleLinear((Line)result.Location);
                    result.Property = newProperty;
                }
                else if (!framingElement.Location.IIsPlanar(tolerance))
                    BH.Engine.Base.Compute.RecordWarning($"The element's location is a nonlinear, nonplanar curve. Correctness of orientation angle after transform could not be ensured in 100%. BHoM_Guid: {framingElement.BHoM_Guid}");
            }

            return result;
        }

        /***************************************************/

        [Description("Transforms the IOpening's location by the transform matrix. Only rigid body transformations are supported.")]
        [Input("opening", "IOpening to transform.")]
        [Input("transform", "Transform matrix.")]
        [Input("tolerance", "Tolerance used in the check whether the input matrix is equivalent to the rigid body transformation.")]
        [Output("transformed", "Modified IOpening with unchanged properties, but transformed location.")]
        public static IOpening Transform(this IOpening opening, TransformMatrix transform, double tolerance = Tolerance.Distance)
        {
            if (!transform.IsRigidTransformation(tolerance))
            {
                BH.Engine.Base.Compute.RecordError("Transformation failed: only rigid body transformations are currently supported.");
                return null;
            }

            IOpening result = opening.ShallowClone();
            result.Location = result.Location?.ITransform(transform);
            return result;
        }

        /***************************************************/

        [Description("Transforms the ISurface's location and openings by the transform matrix. Only rigid body transformations are supported.")]
        [Input("panel", "ISurface to transform.")]
        [Input("transform", "Transform matrix.")]
        [Input("tolerance", "Tolerance used in the check whether the input matrix is equivalent to the rigid body transformation.")]
        [Output("transformed", "Modified ISurface with unchanged properties, but transformed location and openings.")]
        public static oM.Physical.Elements.ISurface Transform(this oM.Physical.Elements.ISurface panel, TransformMatrix transform, double tolerance = Tolerance.Distance)
        {
            if (!transform.IsRigidTransformation(tolerance))
            {
                BH.Engine.Base.Compute.RecordError("Transformation failed: only rigid body transformations are currently supported.");
                return null;
            }

            oM.Physical.Elements.ISurface result = panel.ShallowClone();
            result.Location = result.Location?.ITransform(transform);
            result.Openings = result.Openings?.Select(x => x?.Transform(transform, tolerance)).ToList();
            return result;
        }

        /***************************************************/
    }
}



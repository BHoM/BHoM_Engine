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

using BH.Engine.Geometry;
using BH.Engine.Spatial;
using BH.oM.Geometry;
using BH.oM.MEP.System;
using BH.oM.Reflection.Attributes;
using System.ComponentModel;

namespace BH.Engine.MEP
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Interface Methods - IElements             ****/
        /***************************************************/

        [Description("Transforms the CableTray's end points and orientation angle by the transform matrix. Only rigid body transformations are supported.")]
        [Input("cableTray", "CableTray to transform.")]
        [Input("transform", "Transform matrix.")]
        [Output("transformed", "Modified CableTray with unchanged properties, but transformed end points and orientation angle.")]
        public static CableTray Transform(this CableTray cableTray, TransformMatrix transform)
        {
            if (!transform.IsRigidTransformation())
            {
                BH.Engine.Reflection.Compute.RecordError("Transformation failed: only rigid body transformations are currently supported.");
                return null;
            }

            CableTray result = cableTray.GetShallowClone() as CableTray;
            result.StartPoint = result.StartPoint.Transform(transform);
            result.EndPoint = result.EndPoint.Transform(transform);

            Vector normalBefore = new Line { Start = cableTray.StartPoint, End = cableTray.EndPoint }.ElementNormal(cableTray.OrientationAngle);
            Vector normalAfter = normalBefore.Transform(transform);
            result.OrientationAngle = normalAfter.OrientationAngleLinear(new Line { Start = result.StartPoint, End = result.EndPoint });

            return result;
        }

        /***************************************************/

        [Description("Transforms the Duct's end points and orientation angle by the transform matrix. Only rigid body transformations are supported.")]
        [Input("duct", "Duct to transform.")]
        [Input("transform", "Transform matrix.")]
        [Output("transformed", "Modified Duct with unchanged properties, but transformed end points and orientation angle.")]
        public static Duct Transform(this Duct duct, TransformMatrix transform)
        {
            if (!transform.IsRigidTransformation())
            {
                BH.Engine.Reflection.Compute.RecordError("Transformation failed: only rigid body transformations are currently supported.");
                return null;
            }

            Duct result = duct.GetShallowClone() as Duct;
            result.StartPoint = result.StartPoint.Transform(transform);
            result.EndPoint = result.EndPoint.Transform(transform);

            Vector normalBefore = new Line { Start = duct.StartPoint, End = duct.EndPoint }.ElementNormal(duct.OrientationAngle);
            Vector normalAfter = normalBefore.Transform(transform);
            result.OrientationAngle = normalAfter.OrientationAngleLinear(new Line { Start = result.StartPoint, End = result.EndPoint });

            return result;
        }

        /***************************************************/
    }
}



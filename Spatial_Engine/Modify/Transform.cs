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

using System.Collections.Generic;
using BH.oM.Dimensional;
using BH.oM.Geometry;
using System.ComponentModel;
using BH.oM.Base.Attributes;
using BH.Engine.Geometry;

namespace BH.Engine.Spatial
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Interface Methods - IElements             ****/
        /***************************************************/

        [Description("Transforms the geometrical definition and location-dependent properties of an IElement by the transform matrix. Only rigid body transformations are supported.")]
        [Input("element", "IElement to transform.")]
        [Input("transform", "Transformation matrix to apply.")]
        [Input("tolerance", "Tolerance used in the check whether the input matrix is equivalent to the rigid body transformation.")]
        [Output("transformed", "Modified IElement with unchanged general properties, but transformed geometrical definition and location-dependent properties.")]
        public static IElement ITransform(this IElement element, TransformMatrix transform, double tolerance = Tolerance.Distance)
        {
            return ITransform(element as dynamic, transform, tolerance);
        }

        /***************************************************/

        [Description("Transforms the geometrical definition and location-dependent properties of an IElement2D by the transform matrix. Only rigid body transformations are supported.")]
        [Input("element2D", "IElement2D to transform.")]
        [Input("transform", "Transformation matrix to apply.")]
        [Input("tolerance", "Tolerance used in the check whether the input matrix is equivalent to the rigid body transformation.")]
        [Output("transformed", "Modified IElement2D with unchanged general properties, but transformed geometrical definition and location-dependent properties.")]
        public static IElement2D ITransform(this IElement2D element2D, TransformMatrix transform, double tolerance = Tolerance.Distance)
        {
            object result;
            if (!Base.Compute.TryRunExtensionMethod(element2D, "Transform", new object[] { transform, tolerance }, out result))
                result = element2D.Transform(transform, tolerance);

            return result as IElement2D;
        }

        /***************************************************/

        [Description("Transforms the geometrical definition and location-dependent properties of an IElement1D by the transform matrix. Only rigid body transformations are supported.")]
        [Input("element1D", "IElement1D to transform.")]
        [Input("transform", "Transformation matrix to apply.")]
        [Input("tolerance", "Tolerance used in the check whether the input matrix is equivalent to the rigid body transformation.")]
        [Output("transformed", "Modified IElement1D with unchanged general properties, but transformed geometrical definition and location-dependent properties.")]
        public static IElement1D ITransform(this IElement1D element1D, TransformMatrix transform, double tolerance = Tolerance.Distance)
        {
            object result;
            if (!Base.Compute.TryRunExtensionMethod(element1D, "Transform", new object[] { transform, tolerance }, out result))
                result = element1D.Transform(transform, tolerance);

            return result as IElement1D;
        }

        /***************************************************/

        [Description("Transforms the geometrical definition and location-dependent properties of an IElement0D by the transform matrix. Only rigid body transformations are supported.")]
        [Input("element0D", "IElement0D to transform.")]
        [Input("transform", "Transformation matrix to apply.")]
        [Input("tolerance", "Tolerance used in the check whether the input matrix is equivalent to the rigid body transformation.")]
        [Output("transformed", "Modified IElement0D with unchanged general properties, but transformed geometrical definition and location-dependent properties.")]
        public static IElement0D ITransform(this IElement0D element0D, TransformMatrix transform, double tolerance = Tolerance.Distance)
        {
            object result;
            if (!Base.Compute.TryRunExtensionMethod(element0D, "Transform", new object[] { transform, tolerance }, out result))
                result = element0D.Transform(transform, tolerance);

            return result as IElement0D;
        }


        /***************************************************/
        /**** Private Methods - IElements               ****/
        /***************************************************/

        private static IElement2D Transform(this IElement2D element2D, TransformMatrix transform, double tolerance)
        {
            if (!transform.IsRigidTransformation(tolerance))
            {
                BH.Engine.Base.Compute.RecordError("Transformation failed: only rigid body transformations are currently supported.");
                return null;
            }

            List<IElement1D> newOutline = new List<IElement1D>();
            foreach (IElement1D element1D in element2D.IOutlineElements1D())
            {
                newOutline.Add(element1D.Transform(transform, tolerance));
            }
            IElement2D result = element2D.ISetOutlineElements1D(newOutline);

            List<IElement2D> newInternalOutlines = new List<IElement2D>();
            foreach (IElement2D internalElement2D in result.IInternalElements2D())
            {
                newInternalOutlines.Add(internalElement2D.Transform(transform, tolerance));
            }
            result = result.ISetInternalElements2D(newInternalOutlines);
            return result;
        }

        /***************************************************/

        private static IElement1D Transform(this IElement1D element1D, TransformMatrix transform, double tolerance)
        {
            if (!transform.IsRigidTransformation(tolerance))
            {
                BH.Engine.Base.Compute.RecordError("Transformation failed: only rigid body transformations are currently supported.");
                return null;
            }

            return element1D.ISetGeometry(Geometry.Modify.ITransform(element1D.IGeometry(), transform));
        }

        /***************************************************/

        private static IElement0D Transform(this IElement0D element0D, TransformMatrix transform, double tolerance)
        {
            if (!transform.IsRigidTransformation(tolerance))
            {
                BH.Engine.Base.Compute.RecordError("Transformation failed: only rigid body transformations are currently supported.");
                return null;
            }

            return element0D.ISetGeometry(Geometry.Modify.Transform(element0D.IGeometry(), transform));
        }

        /***************************************************/
    }
}






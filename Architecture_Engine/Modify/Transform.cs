/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2025, the respective contributors. All rights reserved.
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
using BH.oM.Architecture.BuildersWork;
using BH.oM.Architecture.Elements;
using BH.oM.Geometry;
using BH.oM.Base.Attributes;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.Architecture
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Interface Methods - IElements             ****/
        /***************************************************/

        [Description("Transforms the Ceiling's surface and tiles by the transform matrix. Only rigid body transformations are supported.")]
        [Input("ceiling", "Ceiling to transform.")]
        [Input("transform", "Transform matrix.")]
        [Input("tolerance", "Tolerance used in the check whether the input matrix is equivalent to the rigid body transformation.")]
        [Output("transformed", "Modified Ceiling with unchanged properties, but transformed surface and tiles.")]
        public static Ceiling Transform(this Ceiling ceiling, TransformMatrix transform, double tolerance = Tolerance.Distance)
        {
            if (!transform.IsRigidTransformation(tolerance))
            {
                BH.Engine.Base.Compute.RecordError("Transformation failed: only rigid body transformations are currently supported.");
                return null;
            }

            Ceiling result = ceiling.ShallowClone();
            result.Surface = result.Surface.ITransform(transform);
            result.Tiles = result.Tiles.Select(x => x.Transform(transform)).ToList();
            return result;
        }

        /***************************************************/

        [Description("Transforms the CeilingTile's perimeter by the transform matrix. Only rigid body transformations are supported.")]
        [Input("tile", "CeilingTile to transform.")]
        [Input("transform", "Transform matrix.")]
        [Input("tolerance", "Tolerance used in the check whether the input matrix is equivalent to the rigid body transformation.")]
        [Output("transformed", "Modified CeilingTile with unchanged properties, but transformed perimeter.")]
        public static CeilingTile Transform(this CeilingTile tile, TransformMatrix transform, double tolerance = Tolerance.Distance)
        {
            if (!transform.IsRigidTransformation(tolerance))
            {
                BH.Engine.Base.Compute.RecordError("Transformation failed: only rigid body transformations are currently supported.");
                return null;
            }

            CeilingTile result = tile.ShallowClone();
            result.Perimeter = result.Perimeter.ITransform(transform);
            return result;
        }

        /***************************************************/

        [Description("Transforms the Opening's coordinate system by the transform matrix. Only rigid body transformations are supported.")]
        [Input("opening", "Opening to transform.")]
        [Input("transform", "Transform matrix.")]
        [Input("tolerance", "Tolerance used in the check whether the input matrix is equivalent to the rigid body transformation.")]
        [Output("transformed", "Modified Opening with unchanged properties, but transformed coordinate system.")]
        public static Opening Transform(this Opening opening, TransformMatrix transform, double tolerance = Tolerance.Distance)
        {
            if (!transform.IsRigidTransformation(tolerance))
            {
                BH.Engine.Base.Compute.RecordError("Transformation failed: only rigid body transformations are currently supported.");
                return null;
            }

            Opening result = opening.ShallowClone();
            result.CoordinateSystem = result.CoordinateSystem.Transform(transform);
            return result;
        }

        /***************************************************/

        [Description("Transforms the Room's perimeter and location point by the transform matrix. Only rigid body transformations are supported.")]
        [Input("room", "Room to transform.")]
        [Input("transform", "Transform matrix.")]
        [Input("tolerance", "Tolerance used in the check whether the input matrix is equivalent to the rigid body transformation.")]
        [Output("room", "Modified Room with unchanged properties, but transformed perimeter and location point.")]
        public static Room Transform(this Room room, TransformMatrix transform, double tolerance = Tolerance.Distance)
        {
            if (!transform.IsRigidTransformation(tolerance))
            {
                BH.Engine.Base.Compute.RecordError("Transformation failed: only rigid body transformations are currently supported.");
                return null;
            }

            Room result = room.ShallowClone();
            result.Perimeter = result.Perimeter.ITransform(transform);
            result.Location = result.Location.Transform(transform);
            return result;
        }

        /***************************************************/
    }
}





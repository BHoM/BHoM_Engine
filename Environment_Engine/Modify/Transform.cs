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

using BH.Engine.Base;
using BH.Engine.Geometry;
using BH.oM.Environment.Elements;
using BH.oM.Geometry;
using BH.oM.Base.Attributes;
using System.ComponentModel;

namespace BH.Engine.Environment
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Transforms the Space's perimeter and location point by the transform matrix. Only rigid body transformations are supported.")]
        [Input("space", "Space to transform.")]
        [Input("matrix", "Transform matrix.")]
        [Input("tolerance", "Tolerance used in the check whether the input matrix is equivalent to the rigid body transformation.")]
        [Output("space", "Modified Space with unchanged properties, but transformed perimeter and location point.")]
        [PreviousInputNames("matrix", "transform")]
        public static Space Transform(this Space space, TransformMatrix matrix, double tolerance = Tolerance.Distance)
        {
            if (!matrix.IsRigidTransformation(tolerance))
            {
                BH.Engine.Base.Compute.RecordError("Transformation failed: only rigid body transformations are currently supported.");
                return null;
            }

            Space result = space.ShallowClone();
            result.Perimeter = result.Perimeter.ITransform(matrix);
            result.Location = result.Location.Transform(matrix);
            return result;
        }

        /***************************************************/
    }
}



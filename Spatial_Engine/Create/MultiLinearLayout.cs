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

using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using BH.oM.Base.Attributes;
using BH.oM.Base;
using BH.oM.Spatial.Layouts;
using BH.oM.Geometry;
using BH.oM.Quantities.Attributes;

namespace BH.Engine.Spatial
{
    public static partial class Create
    {
        /***************************************************/
        /****               Public Methods              ****/
        /***************************************************/

        [Description("Creates a MultiLinearLayout from its core properties. Ensures all vectors are in the global XY-plane.")]
        [InputFromProperty("numberOfPoints")]
        [Input("minimumSpacing", "Minimum spacing between points in layers and between each layer.", typeof(Length))]
        [InputFromProperty("direction")]
        [InputFromProperty("offset")]
        [InputFromProperty("referencePoint")]
        [Output("linLayout", "The created LinearLayout.")]
        public static MultiLinearLayout MultiLinearLayout(int numberOfPoints, double minimumSpacing, Vector direction = null, double offset = 0, ReferencePoint referencePoint = ReferencePoint.BottomCenter)
        {
            return MultiLinearLayout(numberOfPoints, minimumSpacing, minimumSpacing, direction, offset, referencePoint);
        }

        /***************************************************/

        [Description("Creates a MultiLinearLayout from its core properties. Ensures all vectors are in the global XY-plane.")]
        [InputFromProperty("numberOfPoints")]
        [InputFromProperty("parallelMinimumSpacing")]
        [InputFromProperty("perpendicularMinimumSpacing")]
        [InputFromProperty("direction")]
        [InputFromProperty("offset")]
        [InputFromProperty("referencePoint")]
        [Output("linLayout", "The created LinearLayout.")]
        public static MultiLinearLayout MultiLinearLayout(int numberOfPoints, double parallelMinimumSpacing, double perpendicularMinimumSpacing, Vector direction = null, double offset = 0, ReferencePoint referencePoint = ReferencePoint.BottomCenter)
        {
            if (numberOfPoints <= 0)
            {
                Engine.Base.Compute.RecordError("MultiLinearLayout requires number of points to be at least 1.");
                return null;
            }

            if (parallelMinimumSpacing <= 0 || perpendicularMinimumSpacing <= 0)
            {
                Engine.Base.Compute.RecordError("MultiLinearLayout requires the minimum spacing to be larger than 0.");
                return null;
            }

            Vector projDir = direction ?? Vector.XAxis;
            if (projDir.Z != 0)
            {
                projDir = new Vector{X = direction.X, Y = direction.Y};
                Engine.Base.Compute.RecordWarning("Direction vector has been projected to the global XY-plane.");
            }

            return new MultiLinearLayout(numberOfPoints, parallelMinimumSpacing, perpendicularMinimumSpacing, projDir, offset, referencePoint);
        }

        /***************************************************/
    }
}
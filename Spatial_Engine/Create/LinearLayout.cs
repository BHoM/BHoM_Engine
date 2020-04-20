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

using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using BH.oM.Reflection.Attributes;
using BH.oM.Base;
using BH.oM.Spatial.Layouts;
using BH.oM.Geometry;

namespace BH.Engine.Spatial
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates a LinearLayout from its core proeprties. Ensures all vectors are in the global XY-plane.")]
        [InputFromProperty("numberOfPoints")]
        [InputFromProperty("direction")]
        [InputFromProperty("offset")]
        [Output("linLayout", "The created LinearLayout.")]
        public static LinearLayout LinearLayout(int numberOfPoints, Vector direction, Vector offset)
        {
            Vector projDir = direction;
            if (direction.Z != 0)
            {
                projDir = new Vector { X = direction.X, Y = direction.Y };
                Engine.Reflection.Compute.RecordWarning("Direction vector has been projected to the global XY-plane.");
            }

            Vector projOffset = offset;
            if (direction.Z != 0)
            {
                projOffset = new Vector { X = offset.X, Y = offset.Y };
                Engine.Reflection.Compute.RecordWarning("Offset vector has been projected to the global XY-plane.");
            }

            return new LinearLayout(numberOfPoints, projDir, projOffset);
        }

        /***************************************************/
    }
}

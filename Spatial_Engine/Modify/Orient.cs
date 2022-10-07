﻿/*
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

using BH.oM.Base;
using BH.oM.Base.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using BH.oM.Dimensional;
using BH.oM.Geometry;
using BH.oM.Geometry.CoordinateSystem;


namespace BH.Engine.Spatial
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Orients the element from one coordinate system to another.")]
        [Input("element", "The element to orient.")]
        [Input("from", "The cartesian coordinate system to orient from. Deaults to global XY if nothing is provided.")]
        [Input("to", "The cartesian coordinate system to orient to. Deaults to global XY if nothing is provided.")]
        [Output("element", "The reoriented element.")]
        public static IElement Orient(this IElement element, Cartesian from = null, Cartesian to = null)
        {
            if (element == null)
            {
                Base.Compute.RecordError("Cannot Orient a null element");
                return null;
            }

            if (from == null && to == null)
            {
                Engine.Base.Compute.RecordWarning($"No {nameof(from)} or {nameof(to)} coordinate systems provided. Unchanged element returned.");
                return element;
            }

            if (from == null)
            {
                Base.Compute.RecordNote($"The {nameof(from)} coordinate system is null. Global XY will be used.");
                from = new Cartesian();
            }

            if (to == null)
            {
                Base.Compute.RecordNote($"The {nameof(to)} coordinate system is null. Global XY will be used.");
                to = new Cartesian();
            }

            TransformMatrix transformMatrix = Geometry.Create.OrientationMatrix(from, to);
            return element.ITransform(transformMatrix);
        }

        /***************************************************/
    }
}
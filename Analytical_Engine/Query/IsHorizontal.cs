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

using BH.oM.Analytical.Elements;
using BH.oM.Geometry;
using BH.oM.Base.Attributes;
using BH.Engine.Geometry;
using BH.Engine.Reflection;
using BH.oM.Quantities.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;

namespace BH.Engine.Analytical
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Checks if the IPanel is aligned with the horizontal plane (global XY).")]
        [Input("panel", "The IPanel to check if it is aligned to the horizontal plane (global XY).")]
        [Input("tolerance", "Angle tolerance between the IPanel and the horizontal plane (global XY)", typeof(Angle))]
        [Output("bool", "True if the IPanel is aligned with the horizontal plane (global XY).")]
        public static bool IsHorizontal<TEdge, TOpening>(this IPanel<TEdge, TOpening> panel, double tolerance = Tolerance.Angle)
            where TEdge : IEdge
            where TOpening : IOpening<TEdge>
        {
            return IsAligned(panel, Plane.XY, tolerance);
        }  
    }
}





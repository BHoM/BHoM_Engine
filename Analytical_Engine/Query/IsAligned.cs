﻿/*
 * This file is part of the Buildings and Habitats object Model(BHoM)
 * Copyright(c) 2015 - 2020, the respective contributors.All rights reserved.

*
* Each contributor holds copyright over their respective contributions.

* The project versioning(Git) records all such contribution source information.
 *                                           
 *                                                                              
 * The BHoM is free software: you can redistribute it and/or modify         
 * it under the terms of the GNU Lesser General Public License as published by  
 * the Free Software Foundation, either version 3.0 of the License, or
* (at your option) any later version.

*
* The BHoM is distributed in the hope that it will be useful,              
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
 * GNU Lesser General Public License for more details.                          
 *                                                                            
 * You should have received a copy of the GNU Lesser General Public License
 * along with this code.If not, see<https://www.gnu.org/licenses/lgpl-3.0.html>.      
 */




using BH.oM.Analytical.Elements;
using BH.oM.Geometry;
using BH.oM.Reflection.Attributes;
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

        [Description("Checks if the Panel is planar and is aligned to the Plane provided.")]
        [Input("panel", "The IPanel to check if it is aligned to the Plane provided.")]
        [Input("plane", "The Plane that the IPanel alignment is checked against.")]
        [Input("tolerance", "Angle tolerance between the IPanel and the Plane provided", typeof(Angle))]
        [Output("bool", "True if the IPanel is aligned withe the Plane provided")]
        public static bool IsAligned<TEdge, TOpening>(this IPanel<TEdge, TOpening> panel, Plane plane, double tolerance = Tolerance.Angle)
            where TEdge : IEdge
            where TOpening : IOpening<TEdge>
        {
            PolyCurve polycurve = ExternalPolyCurve(panel);

            if (!polycurve.IsPlanar())
                return false;
            Vector planeNormal = plane.Normal;
            Vector polycurveNormal = polycurve.Normal();

            return planeNormal.IsParallel(polycurveNormal, tolerance) == 1 || planeNormal.IsParallel(polycurveNormal, tolerance) == -1;
        }

    }
}

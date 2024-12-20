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


using BH.oM.Analytical.Elements;
using BH.oM.Geometry;
using BH.oM.Base.Attributes;
using BH.Engine.Geometry;
using BH.Engine.Reflection;

using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Linq.Expressions;

namespace BH.Engine.Analytical
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Gets the polycurve that defines the outline of the panel and checks for a single continuous linear curve")]
        [Input("panel", "The IPanel to get the polycurve from")]
        [Output("polycurve", "The polycurve defining the outline of the panel")]
        public static PolyCurve ExternalPolyCurve<TEdge, TOpening>(this IPanel<TEdge, TOpening> panel)
            where TEdge : IEdge
            where TOpening : IOpening<TEdge>
        {
            List<ICurve> curves = panel.ExternalEdges.SelectMany(x => x.Curve.ISubParts()).ToList();

            List<PolyCurve> polycurves = Engine.Geometry.Compute.IJoin(curves);

            if (polycurves.Count != 1)
            {
                Base.Compute.RecordError("The curve defining the Panel is not a single continuous curve");
                return null;
            }

            PolyCurve polycurve = polycurves.First();

            return polycurve;
        }

    }
}





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

using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using BH.oM.Base.Attributes;
using BH.oM.Geometry;
using BH.oM.Spatial.Layouts;
using BH.Engine.Geometry;

namespace BH.Engine.Spatial
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates an explicit curve layout. Curves not in the global XY-plane will get projected to it.")]
        [Input("curves", "The explicit shape of curves in the layout. All curves should be planar curves in the global XY plane.\nCurves not in the global XY-plane will get projected to it.")]
        [Output("curveLayout", "Created explicit curve layout.")]
        public static ExplicitCurveLayout ExplicitCurveLayout(IEnumerable<ICurve> curves)
        {
            IEnumerable<ICurve> xyCurves = curves;
            if (xyCurves.Any(curve => !curve.IIsInPlane(Plane.XY, Tolerance.Distance)))
            {
                xyCurves = curves.Select(curve => curve.IProject(Plane.XY));
                Base.Compute.RecordWarning("Curves has been projected to the global XY-plane.");
            }

            return new ExplicitCurveLayout(xyCurves);
        }

        /***************************************************/
    }
}





/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2021, the respective contributors. All rights reserved.
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
using System.ComponentModel;
using BH.oM.Reflection.Attributes;
using BH.oM.Physical.Elements;
using BH.oM.Geometry;
using BH.Engine.Geometry;

namespace BH.Engine.Physical
{
    public static partial class Query
    {
        /***************************************************/
        /****              Public Methods               ****/
        /***************************************************/

        [Description("Returns the bottom of a given generic opening object.")]
        [Input("opening", "Any object implementing the IOpening interface that can have a geometrical bottom.")]
        [Input("distanceTolerance", "Distance tolerance for calculating discontinuity points, default is set to the value defined by BH.oM.Geometry.Tolerance.Distance.")]                
        [Output("curve", "An ICurve representation of the bottom of the object.")]
        public static ICurve Bottom(this IOpening opening, double distanceTolerance = BH.oM.Geometry.Tolerance.Distance)
        {
            if (opening == null)
                return null;

            Polyline workingCurves = opening.Location.IExternalEdges().FirstOrDefault().ICollapseToPolyline(BH.oM.Geometry.Tolerance.Angle);

            if (workingCurves == null)
                return null;

            double heightZ = double.MaxValue;
            ICurve result = null;
            foreach (ICurve curve in workingCurves.SplitAtPoints(workingCurves.DiscontinuityPoints()))
            {
                Point start = curve.IStartPoint();
                Point end = curve.IEndPoint();

                if (end.Z <= heightZ && start.Z <= heightZ)
                {
                    heightZ = Math.Max(end.Z, start.Z);
                    result = curve;
                }
            }

            return result;
        }

        /***************************************************/

    }
}



/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2026, the respective contributors. All rights reserved.
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

using BH.oM.Base.Attributes;
using BH.oM.Geometry;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns the chord direction from the first control point of a polyline to its last (or pre-close) control point.")]
        [Input("pLine", "Polyline whose start-to-end direction is requested.")]
        [Output("direction", "Vector from the first control point to the last non-closing control point.")]
        public static Vector StartToEndDirection(this Polyline pLine)
        {
            if (pLine.ControlPoints.Count == 2)
                return pLine.SubParts().First().StartToEndDirection();

            Point p1 = pLine.ControlPoints.First();
            Point p2 = pLine.ControlPoints.Last();

            if (pLine.IsClosed())
                p2 = pLine.ControlPoints[pLine.ControlPoints.Count - 2];

            return p2 - p1;
        }

        /***************************************************/

        [Description("Returns the normalised chord direction from the first control point of a curve to its last.")]
        [Input("curve", "Curve whose start-to-end direction is requested.")]
        [Output("direction", "Normalised vector from the first control point to the last.")]
        public static Vector StartToEndDirection(this ICurve curve)
        {
            List<Point> controlPoints = curve.IControlPoints();
            return (controlPoints.Last() - controlPoints.First()).Normalise();
        }

        /***************************************************/
    }
}

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

using BH.oM.Base.Attributes;
using BH.oM.Geometry;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Compute
    {
        /***************************************************/
        /****              Public Methods               ****/
        /***************************************************/

        [Description("Splits a closed planar polyline at self intersections and returns a collection of closed polylines.")]
        [Input("polyline", "Closed planar polyline to split at self intersections.")]
        [Input("distanceTolerance", "Distance tolerance used in geometrical processing.")]
        [Output("splitOutlines", "Closed polylines representing the input polyline split at self intersections.")]
        public static List<Polyline> SplitAtSelfIntersections(this Polyline polyline, double distanceTolerance = Tolerance.Distance)
        {
            if (polyline.IsNull())
                return null;

            if (!polyline.IsClosed(distanceTolerance))
            {
                BH.Engine.Base.Compute.RecordError("Splitting at self intersections works only on closed polylines.");
                return null;
            }

            if (!polyline.IsPlanar(distanceTolerance))
            {
                BH.Engine.Base.Compute.RecordError("Splitting at self intersections works only on planar polylines.");
                return null;
            }

            List<Line> segments = polyline.SubParts();
            List<Point> selfIntersections = segments.LineIntersections(false, distanceTolerance).CullDuplicates();
            segments = segments.SelectMany(x => x.SplitAtPoints(selfIntersections, distanceTolerance)).ToList();
            return OutlinesFromLines(segments, distanceTolerance);
        }

        /***************************************************/
    }
}

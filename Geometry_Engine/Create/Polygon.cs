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

using BH.oM.Base;
using BH.oM.Base.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using BH.oM.Geometry;
using BH.oM.Quantities.Attributes;
using BH.Engine.Base;

namespace BH.Engine.Geometry
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates a Polygon from the set of provided Points. The provided points will be checked for planarity and a check will be made to ensure that no segments are intersecting with each other. If those checks are failed, null will be returned.")]
        [Input("vertices", "The set of vertices of the Polygon. Will be checkef for planarity and ensured to not be self intersecting. For a polygon no duplicate points are stored, meaning the first point in the list will be both the start and end point. If the first point provided is the same as the last point, the last point will be culled with no warning given.")]
        [Input("tolerance", "Tolerance used for checking the validity of the incoming vertices.", typeof(Length))]
        [Output("polygon", "The created Polygon.")]
        public static Polygon Polygon(List<Point> vertices, double tolerance = Tolerance.Distance)
        {
            if (vertices.IsNullOrEmpty())
                return null;

            List<Point> checkedVertices = vertices.ToList();

            if (checkedVertices[0].SquareDistance(checkedVertices.Last()) < tolerance * tolerance)
                checkedVertices.RemoveAt(checkedVertices.Count - 1);

            int count = checkedVertices.Count;
            if (count < 3)
            {
                Engine.Base.Compute.RecordError("Insufficent number of points provided to create a Polygon. At least 3 unique points required.");
                return null;
            }

            if (checkedVertices.IsCollinear(tolerance))
            {
                Engine.Base.Compute.RecordError("Provided vertices are co-linear. Polygon not created.");
                return null;
            }

            if (count > 3)  //3 non-colinear vertices are gurarantiued to be planar and non-selfintersecting.
            {
                if (!checkedVertices.IsCoplanar(tolerance))
                {
                    Engine.Base.Compute.RecordError("Provided vertices are not co-planar. Polygon not created.");
                    return null;
                }
                //Temporary polyline to check for self-intersections
                Polyline pLine = new Polyline { ControlPoints = checkedVertices.ToList() };
                pLine.ControlPoints.Add(checkedVertices[0]);

                if (pLine.IsSelfIntersecting(tolerance))
                {
                    Engine.Base.Compute.RecordError("The provided vertices creates line segments that intersect each other. Polygon not created.");
                    return null;
                }
            }
            return new Polygon(checkedVertices);
        }

        /***************************************************/
    }
}




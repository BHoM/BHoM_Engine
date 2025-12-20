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

using BH.oM.Geometry;
using BH.oM.Base.Attributes;
using BH.oM.Quantities.Attributes;
using System.Collections.Generic;
using System.ComponentModel;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {

        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates an IntegrationSlice by slicing a collection of curves at a specified location along a plane.")]
        [Input("edges", "The collection of ICurves to slice.")]
        [Input("location", "The location along the plane normal to slice at.", typeof(Length))]
        [Input("width", "The width of the integration slice.", typeof(Length))]
        [Input("p", "The plane defining the slicing direction.")]
        [Input("tolerance", "The tolerance for geometric calculations.", typeof(Length))]
        [Output("slice", "The resulting IntegrationSlice.")]
        public static IntegrationSlice SliceAt(this IList<ICurve> edges, double location, double width, Plane p, double tolerance = Tolerance.Distance)
        {
            List<Point> y = new List<Point>();
            double length = 0;
            Plane plane = new Plane { Origin = Create.Point(p.Normal * location), Normal = p.Normal };
            for (int edgeIndex = 0; edgeIndex < edges.Count; edgeIndex++)
            {
                y.AddRange(edges[edgeIndex].IPlaneIntersections(plane, tolerance));
            }

            y.RemoveAll(x => x == null);
            List<double> isolatedCoords = new List<double>();

            for (int point = 0; point < y.Count; point++)
            {
                if (p.Normal.X > 0)
                    isolatedCoords.Add(y[point].Y);
                else
                    isolatedCoords.Add(y[point].X);
            }

            isolatedCoords.Sort();

            if (isolatedCoords.Count % 2 != 0)
            {
                for (int k = 0; k < isolatedCoords.Count - 1; k++)
                {
                    if (isolatedCoords[k] == isolatedCoords[k + 1])
                        isolatedCoords.RemoveAt(k + 1);
                }
            }

            for (int j = 0; j < isolatedCoords.Count - 1; j += 2)
            {
                length = length + isolatedCoords[j + 1] - isolatedCoords[j];
            }

            return Create.IntegrationSlice(width, length, location, isolatedCoords.ToArray());
        }

        /***************************************************/
    }
}

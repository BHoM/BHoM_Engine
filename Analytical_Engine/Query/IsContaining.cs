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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.oM.Analytical.Elements;
using BH.oM.Geometry;
using BH.Engine.Geometry;
using BH.oM.Base;
using BH.oM.Base.Attributes;
using BH.oM.Dimensional;
using BH.Engine.Spatial;
using System.ComponentModel;

namespace BH.Engine.Analytical
{
    public static partial class Query
    {
        [Description("Identifies whether a given collection of points are contained within the provided region. Different options allow for all the points to be contained or only some points to be contained.")]
        [Input("region", "The region to check is containing the provided points.")]
        [Input("points", "The points to check if the region contains.")]
        [Input("mustContainAll", "Flag to determine whether the region must contain all the points or only some of the points.")]
        [Input("acceptOnEdge", "Flag to determine whether a point lying on the edge of the regions perimeter is to be counted as contained or not.")]
        [Input("distanceTolerance", "The tolerance to be used for distance calculations on containment.")]
        [MultiOutput(0, "isContaining", "True if a point is contained by the region, False if it isn't, the order matching the inputted point order. If 'mustContainAll' is set to true, then this will be one value of True only if all the points are contained by the region, and False if they aren't.")]
        [MultiOutput(1, "notContained", "A collection of points not contained by the region. If 'mustContainAll' is set to true, this will be empty regardless of the result of the check.")]
        public static Output<List<bool>, List<Point>> IsContaining(this IRegion region, List<Point> points, bool mustContainAll = false, bool acceptOnEdge = true, double distanceTolerance = BH.oM.Geometry.Tolerance.Distance)
        {
            if(region == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query whether a null region contains any of the provided points.");
                return null;
            }

            if(points == null || points.Count == 0)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query whether a null or empty list of points is contained by the region.");
                return null;
            }

            List<bool> results = new List<bool>();
            List<Point> notContained = new List<Point>();

            if (mustContainAll)
                results.Add(region.Perimeter.IIsContaining(points, acceptOnEdge, distanceTolerance));
            else
            {
                foreach (Point p in points)
                {
                    bool result = region.Perimeter.IIsContaining(new List<Point> { p }, acceptOnEdge, distanceTolerance);
                    results.Add(result);
                    if (!result)
                        notContained.Add(p);
                }
            }

            return new Output<List<bool>, List<Point>>
            {
                Item1 = results,
                Item2 = notContained,
            };
        }

        [Description("Defines whether an Environment Space contains each of a provided list of points.")]
        [Input("region", "An Environment Space object defining a perimeter to build a 3D volume from and check if the volume contains the provided point.")]
        [Input("regionHeight", "The height of the region.", typeof(BH.oM.Quantities.Attributes.Length))]
        [Input("points", "The points being checked to see if it is contained within the bounds of the 3D volume.")]
        [Input("acceptOnEdges", "Decide whether to allow the point to sit on the edge of the region, default false.")]
        [Output("isContaining", "True if the point is contained within the region, false if it is not.")]
        public static List<bool> IsContaining(this IRegion region, double regionHeight, List<Point> points, bool acceptOnEdges = false)
        {
            List<Polyline> panelsFromSpace = region.ExtrudeToVolume(regionHeight);
            return panelsFromSpace.IsContaining(points, acceptOnEdges, Tolerance.Distance);
        }

        [Description("Defines whether an Environment Space contains a provided Element.")]
        [Input("region", "An Environment Space object defining a perimeter to build a 3D volume from and check if the volume contains the provided element.")]
        [Input("regionHeight", "The height of the region.", typeof(BH.oM.Quantities.Attributes.Length))]
        [Input("elements", "The elements being checked to see if they are contained within the bounds of the 3D volume.")]
        [Input("acceptOnEdges", "Decide whether to allow the element's point to sit on the edge of the region, default false.")]
        [Input("acceptPartialContainment", "Decide whether to count elements overlapping the region as contained.")]
        [Output("isContaining", "True if the point is contained within the region, false if it is not.")]
        public static List<bool> IsContaining(this IRegion region, double regionHeight, List<IElement> elements, bool acceptOnEdges = false, bool acceptPartialContainment = false)
        {
            List<Polyline> polylines = region.ExtrudeToVolume(regionHeight);

            List<List<Point>> pointLists = new List<List<Point>>();

            foreach (IElement elem in elements)
            {
                List<Point> points = elem.IControlPoints();
                pointLists.Add(points);
            }

            return BH.Engine.Geometry.Query.IsContaining(polylines, pointLists, acceptOnEdges, acceptPartialContainment);
        }
    }
}



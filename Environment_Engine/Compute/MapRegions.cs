/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2020, the respective contributors. All rights reserved.
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
using System.ComponentModel;

using BH.oM.Reflection.Attributes;

using BH.oM.Environment.Elements;
using BH.oM.Geometry;
using BH.Engine.Geometry;

using BH.oM.Analytical.Elements;
using BH.oM.Quantities.Attributes;
using BH.Engine.Spatial;
using BH.oM.Reflection;
using System.Security.Cryptography.X509Certificates;

namespace BH.Engine.Environment
{
    public static partial class Compute
    {
        /***************************************************/
        /****          Public Methods                   ****/
        /***************************************************/

        [Description("Maps regions based on geometry to an original set of regions. This is done by taking the intersections of the regions to map perimeters with the original region perimeters and checking which original region contains those intersections. E.G. mapping IES zoned regions back to original Revit regions. Also filters out unmatched regions and calculates the area percentage of the mapped regions that has been matched to the original regions.")]
        [Input("regionsToMap", "A collection of Environment regions to map to original regions")]
        [Input("originalRegions", "A collection of original regions to map to")]
        [Input("distanceTolerance", "the tolerance used for distance calculations")]
        [Input("angleTolerance", "the tolerance used for angle calculations")]
        [MultiOutput(0, "mappedRegions", "A list of the mapped regions")]
        [MultiOutput(1, "percentages", "A list of area percentages of the mapped regions matched to the original region")]
        [MultiOutput(2, "regionsNotMatched", "A list of the regions that didn't map to any original regions")]
        [MultiOutput(3, "regionsNotFound", "A list of the original regions that didn't find any regions to map")]
        public static Output<List<List<IRegion>>, List<List<double>>, List<IRegion>, List<IRegion>> MapRegions(this List<IRegion> regionsToMap, List<IRegion> originalRegions, double distanceTolerance = BH.oM.Geometry.Tolerance.Distance, double angleTolerance = BH.oM.Geometry.Tolerance.Angle)
        {
            List<List<IRegion>> mappedRegions = new List<List<IRegion>>();
            List<List<double>> percentages = new List<List<double>>();
            List<IRegion> regionsNotMatched = new List<IRegion>();
            List<IRegion> regionNotFound = new List<IRegion>();

            foreach (IRegion region in originalRegions)
            {
                mappedRegions.Add(new List<IRegion>());
                percentages.Add(new List<double>());
            }

            foreach (IRegion region in regionsToMap)
            {          
                // Find the regions to map for each floor by using line intersections
                Polyline perimeter = region.Perimeter.ICollapseToPolyline(angleTolerance);

                List<Point> controlPoints = perimeter.IControlPoints();
                double minZ = controlPoints.Select(x => x.Z).Min() - distanceTolerance;
                double maxZ = controlPoints.Select(x => x.Z).Max() + distanceTolerance;

                List<IRegion> regionsOnLevel = originalRegions.Where(x =>
                                                            {
                                                                double zL = x.Perimeter.IControlPoints()[0].Z;
                                                                return (zL >= minZ && zL <= maxZ);
                                                            }).ToList();

                List<IRegion> matchingPerimeter = regionsOnLevel.Where(x => x.Perimeter.ICollapseToPolyline(angleTolerance).LineIntersections(perimeter, distanceTolerance).Count > 0).ToList();

                matchingPerimeter.AddRange(regionsOnLevel.Where((x => x.Perimeter.IIsContaining(perimeter, true, distanceTolerance))));

                matchingPerimeter = matchingPerimeter.Distinct().ToList();

                // Add a list of regions that haven't been mapped to any original regions
                if (matchingPerimeter.Count == 0)
                    regionsNotMatched.Add(region);

                // Map the matching regions to the original regions 
                foreach (IRegion match in matchingPerimeter)
                {
                    mappedRegions[originalRegions.IndexOf(match)].Add(region);
                }
            }

            // Add a list for original regions without mapped regions
            for (int x = 0; x < mappedRegions.Count; x++)
            {
                if (mappedRegions[x].Count == 0)
                    regionNotFound.Add(originalRegions[x]);
            }

            // Add percentage of original regions matched to the mapped regions
            for (int x = 0; x < originalRegions.Count; x++)
            {
                List<IRegion> mappedRegionsI = new List<IRegion>();

                foreach (IRegion region in mappedRegions[x])
                {
                    double areaIntersecting = 0.0;
                    Polyline originalPerimeter = originalRegions[x].Perimeter.ICollapseToPolyline(angleTolerance);
                    Polyline mappedPerimeter = region.Perimeter.ICollapseToPolyline(angleTolerance);

                    List<Polyline> intersections = BH.Engine.Geometry.Compute.BooleanIntersection(originalPerimeter, mappedPerimeter, distanceTolerance);                  
                    areaIntersecting = intersections.Sum(a => a.Area());

                    if (areaIntersecting != 0)
                    {
                        percentages[x].Add(areaIntersecting / mappedPerimeter.Area());
                    }
                    else
                        mappedRegionsI.Add(region);
                }

                foreach (IRegion r in mappedRegionsI)
                {
                    mappedRegions[x].Remove(r);
                    regionsNotMatched.Add(r);
                }
            }

            return new Output<List<List<IRegion>>, List<List<double>>, List<IRegion>, List<IRegion>>
            {
                Item1 = mappedRegions,
                Item2 = percentages,
                Item3 = regionsNotMatched,
                Item4 = regionNotFound,           
            };
        }
    }
}

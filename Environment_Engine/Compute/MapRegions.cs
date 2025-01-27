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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

using BH.oM.Base.Attributes;

using BH.oM.Environment.Elements;
using BH.oM.Geometry;
using BH.Engine.Geometry;

using BH.oM.Analytical.Elements;
using BH.oM.Quantities.Attributes;
using BH.Engine.Spatial;
using BH.oM.Base;

namespace BH.Engine.Environment
{
    public static partial class Compute
    {
        /***************************************************/
        /****          Public Methods                   ****/
        /***************************************************/

        [Description("Maps a set regions based on geometry to another set of regions by using intersections and overlapping of the region perimeters. E.G. mapping IES zoned regions to Revit regions. This method also filters out unmatched regions and calculates the area percentage of the mapped regions.")]
        [Input("regionListA", "Regions to be grouped according to their intersection or overlap with regionListB.")]
        [Input("regionListB", "Regions to use as basis for grouping of regionListA according to their intersection with regionsA.")]
        [Input("distanceTolerance", "The tolerance used for distance calculations.")]
        [Input("angleTolerance", "The tolerance used for angle calculations.")]
        [MultiOutput(0, "regionListAMapped", "For each item in regionListB, returns the list of intersecting regions from regionListA.")]
        [MultiOutput(1, "percentageAreaOverlap", "For each item in regionListB, returns the area of overlap of intersecting regions from regionsA.")]
        [MultiOutput(2, "regionListANotMapped", "Regions from regionListA which do not intersect with any region from regionListB.")]
        [MultiOutput(3, "regionListBNotMapped", "Regions from regionListB which do not intersect with any region from regionListA.")]
        [PreviousInputNames("regionListA", "regionsToMap")]
        [PreviousInputNames("regionListB", "originalRegions")]
        public static Output<List<List<IRegion>>, List<List<double>>, List<IRegion>, List<IRegion>> MapRegions(this List<IRegion> regionListA, List<IRegion> regionListB, double distanceTolerance = BH.oM.Geometry.Tolerance.Distance, double angleTolerance = BH.oM.Geometry.Tolerance.Angle)
        {
            List<List<IRegion>> regionListAMapped = new List<List<IRegion>>();
            List<List<double>> percentageAreaOverlap = new List<List<double>>();
            List<IRegion> regionListANotMapped = new List<IRegion>();
            List<IRegion> regionNotFound = new List<IRegion>();

            foreach (IRegion region in regionListB)
            {
                regionListAMapped.Add(new List<IRegion>());
                percentageAreaOverlap.Add(new List<double>());
            }

            foreach (IRegion region in regionListA)
            {          
                // Find the regions to map for each floor by using line intersections
                Polyline perimeter = region.Perimeter.ICollapseToPolyline(angleTolerance);

                List<Point> controlPoints = perimeter.IControlPoints();
                double minZ = controlPoints.Select(x => x.Z).Min() - distanceTolerance;
                double maxZ = controlPoints.Select(x => x.Z).Max() + distanceTolerance;

                List<IRegion> regionsOnLevel = regionListB.Where(x =>
                                                            {
                                                                double zL = x.Perimeter.IControlPoints()[0].Z;
                                                                return (zL >= minZ && zL <= maxZ);
                                                            }).ToList();

                // Match regions by checking both line intersections and if regionsB are containing the perimeter of regionsA
                List<IRegion> matchingPerimeter = regionsOnLevel.Where(x => x.Perimeter.ICollapseToPolyline(angleTolerance).LineIntersections(perimeter, distanceTolerance, angleTolerance).Count > 0).ToList();

                matchingPerimeter.AddRange(regionsOnLevel.Where((x => x.Perimeter.IIsContaining(perimeter, true, distanceTolerance))));

                matchingPerimeter = matchingPerimeter.Distinct().ToList();

                // Map the matching regionsA to the corresponding regionB 
                foreach (IRegion match in matchingPerimeter)
                {
                    regionListAMapped[regionListB.IndexOf(match)].Add(region);
                }
            }

            // Add a list for regionsB without mapped regionsA
            for (int x = 0; x < regionListAMapped.Count; x++)
            {
                if (regionListAMapped[x].Count == 0)
                    regionNotFound.Add(regionListB[x]);
            }
                        
            // Add area percentage of regionsA intersecting with regionsB
            for (int x = 0; x < regionListB.Count; x++)
            {
                List<IRegion> mappedRegionsI = new List<IRegion>();

                foreach (IRegion region in regionListAMapped[x])
                {
                    double areaIntersecting = 0.0;
                    Polyline originalPerimeter = regionListB[x].Perimeter.ICollapseToPolyline(angleTolerance);
                    Polyline mappedPerimeter = region.Perimeter.ICollapseToPolyline(angleTolerance);

                    List<Polyline> intersections = BH.Engine.Geometry.Compute.BooleanIntersection(originalPerimeter, mappedPerimeter, distanceTolerance, angleTolerance);                  
                    areaIntersecting = intersections.Sum(a => a.Area());

                    if (areaIntersecting != 0)
                    {
                        percentageAreaOverlap[x].Add(areaIntersecting / mappedPerimeter.Area());
                    }
                    else
                        mappedRegionsI.Add(region);
                }

                //Remove regionsA without any intersectioning area from the mapped regions and add the empty regionsB to regionListBNotMapped
                foreach (IRegion r in mappedRegionsI)
                {
                    regionListAMapped[x].Remove(r);
                }

                if (regionListAMapped[x].Count == 0)
                    regionNotFound.Add(regionListB[x]);
            }

            regionNotFound = regionNotFound.Distinct().ToList();

            // Check if any regionsB are wholly contained by a regionA and add those regionsA to regionListAMapped           
            List<IRegion> regionListBNotMapped = new List<IRegion>();
            foreach (IRegion regionB in regionNotFound)
            {
                bool wasMatched = false;
                for (int x = 0; x < regionListA.Count; x++)
                {
                    if (regionListA[x].Perimeter.IIsContaining(regionB.Perimeter.ICollapseToPolyline(angleTolerance).IControlPoints()))
                    {
                        regionListAMapped[regionListB.IndexOf(regionB)].Add(regionListA[x]);

                        Polyline originalPerimeter = regionB.Perimeter.ICollapseToPolyline(angleTolerance);
                        Polyline mappedPerimeter = regionListA[x].Perimeter.ICollapseToPolyline(angleTolerance);

                        List<Polyline> intersections = BH.Engine.Geometry.Compute.BooleanIntersection(originalPerimeter, mappedPerimeter, distanceTolerance, angleTolerance); ;
                        double areaIntersecting = intersections.Sum(a => a.Area());

                        if (areaIntersecting != 0)
                            percentageAreaOverlap[regionListB.IndexOf(regionB)].Add(areaIntersecting / mappedPerimeter.Area());

                        wasMatched = true;
                    }
                }

                if (!wasMatched)
                    regionListBNotMapped.Add(regionB);
            }

            foreach (IRegion r in regionListA)
            {
                //Find out which ones haven't been mapped anywhere
                int includedElsewhere = regionListAMapped.Where(x => x.Contains(r)).Count();
                if (includedElsewhere == 0)
                    regionListANotMapped.Add(r);
            }

            regionListBNotMapped = regionListBNotMapped.Distinct().ToList();
            regionListANotMapped = regionListANotMapped.Distinct().ToList();

            return new Output<List<List<IRegion>>, List<List<double>>, List<IRegion>, List<IRegion>>
            {
                Item1 = regionListAMapped,
                Item2 = percentageAreaOverlap,
                Item3 = regionListANotMapped,
                Item4 = regionListBNotMapped,           
            };
        }
    }
}






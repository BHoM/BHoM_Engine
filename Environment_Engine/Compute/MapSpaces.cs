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
        [Description("Map regions based on geometry to an original set of regions. This is done by taking the intersections of the region perimeter with the original region perimeter and checking which original region contains that intersection. E.G. mapping IES zoned spaces back to original Revit spaces.")]
        [Input("regionsToMap", "A collection of Environment regions to map to original regions")]
        [Input("originalRegions", "A collection of original regions to map to")]
        [MultiOutput(0, "nestedList", "A list of the mapped regions")]
        [MultiOutput(1, "notMatched", "A list of the regions that didn't map to any original regions")]
        [MultiOutput(2, "regionsNotFound", "A list of the original regions that didn't find any regions to map")]
        public static Output<List<List<IRegion>>, List<IRegion>, List<IRegion>> MapRegions(this List<IRegion> regionsToMap, List<IRegion> originalRegions)
        {    
            List<List<IRegion>> nestedList = new List<List<IRegion>>();
            List<IRegion> notMatched = new List<IRegion>();
            List<IRegion> regionNotFound = new List<IRegion>();

            foreach (IRegion region in originalRegions)
            {
                nestedList.Add(new List<IRegion>());
            }

            foreach (IRegion region in regionsToMap)
            {
                ICurve perimeter = region.Perimeter;
                List<IRegion> matchingPerimeter = originalRegions.Where(x => x.Perimeter.BooleanIntersection(perimeter).Count > 0).ToList();

                // Add a list for IES-spaces without a Revit space

                if (matchingPerimeter.Count == 0)
                    notMatched.Add(region);

                foreach (IRegion match in matchingPerimeter)
                {
                    nestedList[originalRegions.IndexOf(match)].Add(region);
                }
            }

            // Add a list for revitspaces without IES-spaces

            for (int x = 0; x < nestedList.Count; x++)
            {
                if (nestedList[x].Count == 0)
                    regionNotFound.Add(originalRegions[x]);

            }

            return new Output<List<List<IRegion>>, List<IRegion>, List<IRegion>>
            {
                Item1 = nestedList.ToList(),
                Item2 = notMatched.ToList(),
                Item3 = regionNotFound.ToList(),
            };
        }

        public static Output<List<List<Area>>> MapRegionsArea(this List<IRegion> regionsToMap, List<IRegion> originalRegions)
        {
            //List<ICurve> ogPerimeters = new List<ICurve>();

            List<ICurve> perimeters = new List<ICurve>();

            foreach (IRegion region in originalRegions)
            {
                ICurve regionPerimeter = region.Perimeter;

                foreach (IRegion regionToMap in regionsToMap)
                {
                    perimeters.Add(BH.Engine.Geometry.Compute.BooleanIntersection(regionToMap/*.Select(x => x.Perimeter).ToList()*/, region.Perimeter));
                }
            }


            /*foreach (IRegion intersection in matchingPerimeter)
            {
                 Point centre = intersection.Perimeter.ICentroid();
                 IRegion matchingCenterPoint = originalRegions.Where(x => x.Perimeter.IIsContaining(new List<Point> { centre })).First();
                 //nestedList[originalRegions.IndexOf(matchingCenterPoint)].Add(region);

             } */



            /*foreach (IRegion region in originalRegions)
            {
                List<ICurve> perimetersII = region.Perimeter.BooleanIntersection(regionsToMap);
            }

            foreach (IRegion region in originalRegions)
            {
                Compute.BooleanIntersection(regionsToMap, region);
            }

            List<List<IRegion>> nestedList = new List<List<IRegion>>();

            foreach (IRegion region in originalRegions)
            {
                nestedList.Add(new List<IRegion>());
            }

            foreach (IRegion region in regionsToMap)
            {
                ICurve perimeter = region.Perimeter;
                List<ICurve> perimetersIII = originalRegions.BooleanIntersection(perimeter);
            }*/




        }


        /*public static Output<List<List<IRegion>>, List<IRegion>, List<IRegion>> MapRegions(this List<IRegion> regionsToMap, List<IRegion> originalRegions)
        {
            List<List<IRegion>> nestedList = new List<List<IRegion>>();
            List<IRegion> notMatched = new List<IRegion>();
            List<IRegion> regionNotFound = new List<IRegion>();

            foreach (IRegion region in originalRegions)
            {
                nestedList.Add(new List<IRegion>());
            }



            foreach (IRegion region in regionsToMap)
            {
                ICurve perimeter = region.Perimeter;
                List<IRegion> matchingPerimeter = originalRegions.Where(x => x.Perimeter.BooleanIntersection(perimeter).Count > 0).ToList();

                foreach (IRegion intersection in matchingPerimeter)
                {
                    Point centre = intersection.Perimeter.ICentroid();
                    IRegion matchingCenterPoint = originalRegions.Where(x => x.Perimeter.IIsContaining(new List<Point> { centre })).First();
                    //nestedList[originalRegions.IndexOf(matchingCenterPoint)].Add(region);

                }

                foreach (IRegion intersection in matchingPerimeter)
                {
                    Area area = region.IArea();
                }


            }

        }*/

    }
}
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

namespace BH.Engine.Environment
{
    public static partial class Compute
    {
        [Description("Map spaces based on geometry to an original set of spaces. This is done by taking the centre point of the space perimeter and checking which original space contains that centre point. E.G. mapping IES zoned spaces back to original Revit spaces.")]
        [Input("spacesToMap", "A collection of Environment spaces to map to original spaces")]
        [Input("originalSpaces", "A collection of original spaces to map to")]
        [Output("mappedSpaces", "A nested list of spaces mapped to the originals")]
        public static Output<List<List<IRegion>>, List<IRegion>, List<IRegion>> MapSpaces(this List<IRegion> regionsToMap, List<IRegion> originalRegions)
        {    
            List<List<IRegion>> data = new List<List<IRegion>>();
            List<IRegion> notMatched = new List<IRegion>();
            List<IRegion> regionNotFound = new List<IRegion>();

            foreach (IRegion region in originalRegions)
            {
                data.Add(new List<IRegion>());
            }

            foreach (IRegion region in regionsToMap)
            {
                ICurve perimeter = region.Perimeter;
                List<IRegion> matchingPerimeter = originalRegions.Where(x => x.Perimeter.BooleanIntersection(perimeter).Count > 0).ToList();

                if (matchingPerimeter.Count == 0)
                    notMatched.Add(region);

                foreach (IRegion match in matchingPerimeter)
                {
                    data[originalRegions.IndexOf(match)].Add(region);
                }

                /* foreach (IRegion intersection in matchingPerimeter)
                {
                    Point centre = intersection.Perimeter.ICentroid();
                    IRegion matchingCenterPoint = originalRegions.Where(x => x.Perimeter.IIsContaining(new List<Point> { centre })).First();
                    data[originalRegions.IndexOf(matchingCenterPoint)].Add(region);
                } */
            }

            for (int x = 0; x < data.Count; x++)
            {
                if (data[x].Count == 0)
                    regionNotFound.Add(originalRegions[x]);
                
            }

            return new Output<List<List<IRegion>>, List<IRegion>, List<IRegion>>
            {
                Item1 = data.Where(x => x.Count > 0).ToList(),
                Item2 = notMatched.ToList(),
                Item3 = regionNotFound.ToList(),
            };
        }
    }
}
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

using BH.oM.Analytical.Elements;
using BH.oM.Geometry;
using BH.Engine.Geometry;

using BH.oM.Base.Attributes;
using System.ComponentModel;

namespace BH.Engine.Environment
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Maps a collection of names to a collection of regions by checking if points connected to the names are contained within the region perimeters")]
        [Input("regions", "A collection of regions")]
        [Input("points", "A collection of points connected to the space names and located within the region perimeters")]
        [Input("names", "A collection of names to map to the regions")]
        [Output("regions", "The regions with the mapped names")]
        public static List<IRegion> RegionNameMapping(List<IRegion> regions, List<Point> points, List<string> names, double tolerance = Tolerance.Distance)
        {
            List<IRegion> sortedRegions = regions.OrderBy(x => x.Perimeter.IArea()).ToList();
            for (int x = 0; x < points.Count; x++)
            {
                IRegion selRegion = regions.Where(y => y.Perimeter.IIsContaining(new List<Point> { points[x] }, true, tolerance)).FirstOrDefault();
                if (selRegion != null)
                    selRegion.Name = names[x];
            }
            return regions;
        }
    }
}





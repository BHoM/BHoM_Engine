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

using System.ComponentModel;
using BH.oM.Base.Attributes;
using System.Collections.Generic;
using System.Linq;
using System;

using BH.oM.Environment.Elements;

using BH.Engine.Geometry;
using BH.oM.Geometry;
using BH.Engine.Base;
using BH.oM.Analytical.Elements;

namespace BH.Engine.Environment
{
    public static partial class Modify
    {
        [Description("Match closed Polylines to Regions (e.g. Rooms/Spaces) and update geometry. Where a Region Location point exists within a closed Polyline, the Polyline will be set as the Region Perimeter value.")]
        [Input("polyline", "A closed polyline representing the new Region Perimeter to be assigned.")]
        [Input("region", "Original Regions where Perimeter property is to be updated.")]
        [Output("region", "Regions with updated Perimeter geometry.")]
        public static List<IRegion> SetRegionGeometry(this List<Polyline> polyline, List<IRegion> region)
        {
            List<IRegion> returnRegions = new List<IRegion>();
            foreach (IRegion newRegion in region)
            {
                Point pointinregion = newRegion.Perimeter.PointInRegion();
                Polyline newGeometry = polyline.Where(x => x.IIsContaining(new List<Point>() { pointinregion })).FirstOrDefault();
                newRegion.Perimeter = newGeometry;
                returnRegions.Add(newRegion);
            }
            return returnRegions;
        }  
    }
}


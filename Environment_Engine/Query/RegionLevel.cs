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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

using BH.oM.Spatial.SettingOut;
using BH.Engine.Geometry;
using BH.oM.Geometry;
using BH.oM.Environment.Elements;
using BH.oM.Analytical.Elements;
using BH.oM.Base.Attributes;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns the level of an IRegion.")]
        [Input("region", "An IRegion to return the level of.")]
        [Input("searchLevels", "A list of levels to test for.")]
        [Input("distanceTolerance", "The tolerance of the distance calculation for determining the elevation of the IRegion. Default is equal to BH.oM.Geometry.Tolerance.Distance.")]
        [Input("angleTolerance", "The degree of tolerance on the angle calculation for collapsing the region perimeter to a polyline. Default is equal to BH.oM.Geometry.Tolerance.Angle.")]
        [Output("level", "The level that the IRegion is on.")]
        public static Level RegionLevel(this IRegion region, List<Level> searchLevels, double distanceTolerance = BH.oM.Geometry.Tolerance.Distance, double angleTolerance = BH.oM.Geometry.Tolerance.Angle)
        {
            if(region == null || searchLevels == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the level for a region if either the region or the search levels are null.");
                return null;
            }

            double elevation = region.Perimeter.ICollapseToPolyline(angleTolerance).MinimumLevel();
            return searchLevels.Where(x => x.Elevation >= (elevation - distanceTolerance) && x.Elevation <= (elevation + distanceTolerance)).FirstOrDefault();
        }
    }
}




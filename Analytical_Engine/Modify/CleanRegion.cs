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

using BH.oM.Analytical.Elements;
using BH.Engine.Geometry;
using BH.oM.Base.Attributes;
using System.ComponentModel;

namespace BH.Engine.Analytical
{
    public static partial class Modify
    {
        [Description("Takes the perimeter of an IRegion object, collapses it to a polyline, and then cleans it using the Geometry Engine CleanPolyline method.")]
        [Input("region", "The IRegion to clean the perimeter of.")]
        [Input("angleTolerance", "The tolerance to be used for calculating angles when collapsing the perimeter to a polyline, and when cleaning the polyline as the tolerance defining a straight line. Default is set to BH.oM.Geometry.Tolerance.Angle.")]
        [Input("minimumSegmentLength", "The tolerance of how long a segment should be when cleaning the polyline of the region. Default is set to BH.oM.Geometry.Tolerance.Distance.")]
        [Output("region", "A region with a cleaned perimeter.")]
        public static void CleanRegion(this IRegion region, double angleTolerance = BH.oM.Geometry.Tolerance.Angle, double minimumSegmentLength = BH.oM.Geometry.Tolerance.Distance)
        {
            if (region == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot clean the perimeter of a null region.");
                return;
            }

            region.Perimeter = region.Perimeter.ICollapseToPolyline(angleTolerance).CleanPolyline(angleTolerance, minimumSegmentLength);
        }
    }
}




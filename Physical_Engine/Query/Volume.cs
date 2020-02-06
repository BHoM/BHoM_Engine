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

using BH.oM.Reflection.Attributes;
using System.ComponentModel;
using BH.oM.Physical.Elements;
using BH.oM.Geometry.ShapeProfiles;
using BH.oM.Geometry;
using BH.oM.Physical.FramingProperties;
using BH.oM.Physical.Materials;
using BH.Engine.Geometry;
using BH.Engine.Reflection;

namespace BH.Engine.Physical
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/
        [Description("Returns the volume of a column")]
        [Input("column", "A physical oM column object")]
        [Output("volume", "The total volume of the column object")]
        public static double Volume (this Column column)
        {
            double colLength = column.Location.ILength();
            ConstantFramingProperty prop = (ConstantFramingProperty)column.Property;
            List<ICurve> edges = prop.Profile.Edges.ToList();
            List<PolyCurve> perimeters = BH.Engine.Geometry.Compute.IJoin(edges);

            List<double> areas = perimeters.Select(x => BH.Engine.Geometry.Query.Area(x)).ToList();

            double maxArea = areas.Max();
            areas.Remove(maxArea);

            if(areas.Count > 0)
                maxArea -= areas.Select(x => maxArea - x).ToList().Sum();

            return colLength * maxArea;
        }
    }
}

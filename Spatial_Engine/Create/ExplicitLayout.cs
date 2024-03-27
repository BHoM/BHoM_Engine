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
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using BH.oM.Base.Attributes;
using BH.oM.Base;
using BH.oM.Geometry;
using BH.oM.Spatial.Layouts;

namespace BH.Engine.Spatial
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates an Explicit layout based on a free-form set of Points. Points not in the global XY-plane will get projected to it.")]
        [InputFromProperty("points")]
        [Output("expLayout", "Created explicit layout.")]
        public static ExplicitLayout ExplicitLayout(IEnumerable<Point> points)
        {
            IEnumerable<Point> xyPts = points;
            if (points.Any(x => x.Z != 0))
            {
                xyPts = points.Select(pt => new Point() { X = pt.X, Y = pt.Y });
                Engine.Base.Compute.RecordWarning("Points have been projected to the global XY-plane");
            }
            return new ExplicitLayout(xyPts);
        }

        /***************************************************/
    }
}





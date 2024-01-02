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

using BH.oM.Geometry;
using BH.oM.Spatial.SettingOut;
using BH.oM.Base.Attributes;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns a boolean stating whether a polyline is on a certain level.")]
        [Input("polyline", "A polyline to test if it is on a certain level.")]
        [Input("level", "The level to test for.")]
        [Input("tolerance", "The tolerance of the distance calculation for determining whether a polyline is on the level. Default is equal to BH.oM.Geometry.Tolerance.Distance.")]
        [Output("bool", "A boolean stating whether the polyline is on that level.")]
        public static bool IsOnLevel(this Polyline polyline, Level level, double tolerance = BH.oM.Geometry.Tolerance.Distance)
        {
            if (polyline == null || level == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query whether a polyline is on a level if either the polyline or the level are null.");
                return false;
            }

            double minLevel = polyline.MinimumLevel();
            double maxLevel = polyline.MaximumLevel();
            return ((minLevel >= (level.Elevation - tolerance)) && (maxLevel <= (level.Elevation + tolerance)));
        }
    }
}




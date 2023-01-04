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

using BH.oM.Environment.Elements;
using System;
using System.Collections.Generic;
using System.Linq;

using BH.oM.Geometry;
using BH.Engine.Geometry;

using BH.oM.Spatial.SettingOut;
using BH.oM.Base;

using BH.oM.Base.Attributes;
using System.ComponentModel;

using BH.Engine.Base;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns the minimum level of the given polyline based on the z axis")]
        [Input("polyline", "An Environment polyline to find the minimum level from")]
        [Input("decimals", "Provide decimal location for the minimum levels")]
        [Output("minimumLevel", "The minimum level of the z axis of the polyline")]
        public static double MinimumLevel(this Polyline polyline, int decimals = 3)
        {
            List<Point> crvPts = polyline.IControlPoints();

            double min = 1e10;
            foreach (Point p in crvPts)
                min = Math.Min(min, p.Z);

            return Math.Round(min, decimals);
        }

        [Description("Returns the minimum level of the given panel based on the z axis")]
        [Input("panel", "An Environment Panel to find the minimum level from")]
        [Input("decimals", "Provide decimal location for the minimum levels")]
        [Output("minimumLevel", "The minimum level of the z axis of the panel")]
        public static double MinimumLevel(this Panel panel, int decimals = 3)
        {
            if (panel == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the minimum level of a null panel.");
                return -1;
            }

            return panel.Polyline().MinimumLevel(decimals);
        }

        [Description("Returns the minimum level of the given opening based on the z axis")]
        [Input("opening", "An Environment Opening to find the minimum level from")]
        [Input("decimals", "Provide decimal location for the minimum levels")]
        [Output("minimumLevel", "The minimum level of the z axis of the opening")]
        public static double MinimumLevel(this Opening opening, int decimals = 3)
        {
            if (opening == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the minimum level of a null opening.");
                return -1;
            }

            return opening.Polyline().MinimumLevel(decimals);
        }
    }
}





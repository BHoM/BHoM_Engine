/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2021, the respective contributors. All rights reserved.
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

using BH.oM.Geometry.SettingOut;
using BH.oM.Base;

using BH.oM.Reflection.Attributes;
using System.ComponentModel;

using BH.Engine.Base;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns the maximum level of the given polyline based on the z axis")]
        [Input("polyline", "An Environment polyline to find the maximum level from")]
        [Output("maximumLevel", "The maximum level of the z axis of the polyline")]
        public static double MaximumLevel(this Polyline polyline)
        {
            List<Point> crvPts = polyline.IControlPoints();

            double max = -1e10;
            foreach (Point p in crvPts)
                max = Math.Max(max, p.Z);

            return Math.Round(max, 3);
        }

        [Description("Returns the maximum level of the given panel based on the z axis")]
        [Input("panel", "An Environment Panel to find the maximum level from")]
        [Output("maximumLevel", "The maximum level of the z axis of the panel")]
        public static double MaximumLevel(this Panel panel)
        {
            if(panel == null)
            {
                BH.Engine.Reflection.Compute.RecordError("Cannot query the maximum level of a null panel.");
                return -1;
            }

            return panel.Polyline().MaximumLevel();
        }

        [Description("Returns the maximum level of the given opening based on the z axis")]
        [Input("opening", "An Environment Opening to find the maximum level from")]
        [Output("maximumLevel", "The maximum level of the z axis of the opening")]
        public static double MaximumLevel(this Opening opening)
        {
            if (opening == null)
            {
                BH.Engine.Reflection.Compute.RecordError("Cannot query the maximum level of a null opening.");
                return -1;
            }

            return opening.Polyline().MaximumLevel();
        }
    }
}



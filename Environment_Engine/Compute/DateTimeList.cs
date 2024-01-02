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
using BH.oM.Environment;
using BH.Engine.Geometry;
using BH.oM.Geometry;

using BH.oM.Base.Attributes;
using System.ComponentModel;

namespace BH.Engine.Environment
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("A tool to help creation of a list of DateTime objects.")]
        [Input("startDateTime", "The first DateTime to include")]
        [Input("endDateTime", "The last DateTime to include in the list")]
        [Input("minutesBetween", "Number of minutes by which to increment between startDateTime and endDateTime")]
        [Output("dateTimeList", "A list of DateTime objects")]
        public static IEnumerable<DateTime> DateTimeList(DateTime startDateTime, DateTime endDateTime, int minutesBetween)
        {
            int periods = (int)(endDateTime - startDateTime).TotalMinutes / minutesBetween;
            return Enumerable.Range(0, periods + 1).Select(p => startDateTime.AddMinutes(minutesBetween * p));
        }
    }
}






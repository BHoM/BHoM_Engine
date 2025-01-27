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
using BH.oM.Environment;

using BH.Engine.Geometry;
using BH.oM.Geometry;

using BH.oM.Base.Attributes;
using System.ComponentModel;

using BH.oM.Environment.Climate;

namespace BH.Engine.Environment
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Calculate the Julian Day for the curent date time provided")]
        [Input("year", "The year for calculating the Julian Day from")]
        [Input("month", "The month for calculating the Julian Day from")]
        [Input("day", "The day for calculating the Julian Day from")]
        [Input("hour", "The hour for calculating the Julian Day from")]
        [Input("minute", "The minute for calculating the Julian Day from")]
        [Input("second", "The second for calculating the Julian Day from")]
        [Input("timezone", "The timezone for calculating the Julian Day for, defined as the UTC Offset")]
        [Output("julianDay", "The calculated Julian Day")]
        public static double JulianDay(int year, int month, int day, int hour, int minute, double second, double timezone)
        {
            double dayDecimal = day + (hour - timezone + (minute + (second) / 60) / 60) / 24;
            if (month < 3)
            {
                month += 12;
                year--;
            }

            int y = (int)(365.25 * (year + 4716));
            int m = (int)(30.6001 * (month + 1));

            double julianDay = y + m + dayDecimal - 1524.5;

            if (julianDay > 2299160)
            {
                int alteration = ((int)year / 100);
                int a = (int)(alteration / 4);
                julianDay += (2 - alteration + a);
            }

            return julianDay;
        }
    }

}






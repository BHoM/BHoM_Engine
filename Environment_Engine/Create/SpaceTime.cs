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

using BH.oM.Environment.Climate;
using BH.oM.Geometry;

using BH.oM.Reflection.Attributes;
using System.ComponentModel;

namespace BH.Engine.Environment
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns an Environment SpaceTime object")]
        [Input("location", "An Environment Location object specifying the latitude, longitude and other location specifics of the SpaceTime object, default null")]
        [Input("year", "The year of the time for the space time object, default 2007")]
        [Input("month", "The month of the time for the space time object, default 1 (January)")]
        [Input("day", "The day of the time for the space time object, default 1")]
        [Input("hour", "The hour of the day for the space time object, default 12")]
        [Input("minute", "The minute of the hour for the space time object, default 0")]
        [Input("second", "The second of the minute for the space time object, default 0")]
        [Input("millisecond", "The millisecond of the second for the space time object, default 0")]
        [Input("name", "The name of the space time, default empty string")]
        [Output("spaceTime", "An Environment SpaceTime object - used for defining locations in space and time for climate analysis")]
        [Deprecated("3.0", "Deprecated in favour of default create components produced by BHoM")]
        public static SpaceTime SpaceTime(Location location = null, int year = 2007, int month = 1, int day = 1, int hour = 12, int minute = 0, int second = 0, int millisecond = 0, string name = "")
        {
            if (year < 1900 || year > 2500)
            {
                BH.Engine.Reflection.Compute.RecordError("Invalid Year passed. It should be between 1900 and 2500");
                return null;
            }

            if (month < 1 || month > 12)
            {
                BH.Engine.Reflection.Compute.RecordError("Invalid Month passed. It should be between 1 and 12");
                return null;
            }

            if (day < 1 || day > 31)
            {
                BH.Engine.Reflection.Compute.RecordError("Invalid Day passed. It should be between 1 and 31"); // TODO: A more robust method of checking number of days in the Month and Year attribtes to check validity
                return null;
            }

            if (hour < 0 || hour > 23)
            {
                BH.Engine.Reflection.Compute.RecordError("Invalid Hour passed. It should be between 0 and 23");
                return null;
            }

            if (minute < 0 || minute > 59)
            {
                BH.Engine.Reflection.Compute.RecordError("Invalid Minute passed. It should be between 0 and 59");
                return null;
            }

            if (second < 0 || second > 59)
            {
                BH.Engine.Reflection.Compute.RecordError("Invalid Second passed. It should be between 0 and 59");
                return null;
            }

            if (millisecond < 0 || millisecond > 999)
            {
                BH.Engine.Reflection.Compute.RecordError("Invalid Millisecond passed. It should be between 0 and 999");
                return null;
            }

            return new SpaceTime
            {
                Name = name,
                Location = (location ?? new Location()),
                Time = new Time()
                {
                    Year = year,
                    Month = month,
                    Day = day,
                    Hour = hour,
                    Minute = minute,
                    Second = second,
                    Millisecond = millisecond,
                },
            };
        }
    }
}


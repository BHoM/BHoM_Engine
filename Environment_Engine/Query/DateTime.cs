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
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns the date time object from a Space Time")]
        [Input("spaceTime", "A space time object defining a point in space and time")]
        [Output("dateTime", "A C# DateTime object with the values from the SpaceTime object")]
        public static DateTime DateTime(this SpaceTime spaceTime)
        {
            if(spaceTime == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the date/time of a null space time object.");
                return System.DateTime.Now;
            }

            return spaceTime.Time.DateTime();
        }

        [Description("Returns the date time object from a Time object")]
        [Input("time", "A Time object defining a point time for Environmental Analysis")]
        [Output("dateTime", "A C# DateTime object with the values from the Time object")]
        public static DateTime DateTime(this Time time)
        {
            if(time == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the date/time of a null time object.");
                return System.DateTime.Now;
            }

            return new DateTime(time.Year, time.Month, time.Day, time.Hour, time.Minute, time.Second, time.Millisecond);
        }
    }

}





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

        [Description("Returns an Environment Location object")]
        [Input("latitude", "The latitude for the location, default 0")]
        [Input("longitude", "The longitude for the location, default 0")]
        [Input("elevation", "The elevation at the location, default 0")]
        [Input("utcOffset", "The offset from UTC for the location (positive or negative), default 0")]
        [Input("name", "The name of the location, default empty string")]
        [Output("location", "An Environment location object - used for defining locations in space for climate analysis")]
        [Deprecated("3.0", "Deprecated in favour of default create components produced by BHoM")]
        public static Location Location(double latitude = 0, double longitude = 0, double elevation = 0, double utcOffset = 0, string name = "")
        {
            if (latitude < -90 || latitude > 90)
            {
                BH.Engine.Reflection.Compute.RecordError("Invalid Latitude passed. It should be between -90 and 90");
                return null;
            }

            if (longitude < -180 || longitude > 180)
            {
                BH.Engine.Reflection.Compute.RecordError("Invalid Longitude passed. It should be between -180 and 180");
                return null;
            }

            if (elevation < -413 || elevation > 8848)
            {
                BH.Engine.Reflection.Compute.RecordError("Invalid Elevation passed. It should be between -413 and 8848");
                return null;
            }

            if (utcOffset < -12 || utcOffset > 12)
            {
                BH.Engine.Reflection.Compute.RecordError("Invalid UtcOffset passed. It should be between -12 and 12");
                return null;
            }

            return new Location
            {
                Name = name,
                Latitude = latitude,
                Longitude = longitude,
                Elevation = elevation,
                UtcOffset = utcOffset,
            };
        }
    }
}


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
using BH.oM.Environment.Climate;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/
        public static bool IsValidLocation(this Location location)
        {
            if(location == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query if a location is valid if the location is null.");
                return false;
            }

            double latitude = location.Latitude;
            if (latitude < -90 || latitude > 90)
            {
                BH.Engine.Base.Compute.RecordError("Invalid Latitude passed. It should be between -90 and 90");
                return false;
            }

            double longitude = location.Longitude;
            if (longitude < -180 || longitude > 180)
            {
                BH.Engine.Base.Compute.RecordError("Invalid Longitude passed. It should be between -180 and 180");
                return false;
            }

            double elevation = location.Elevation;
            if (elevation < -413 || elevation > 8848)
            {
                BH.Engine.Base.Compute.RecordError("Invalid Elevation passed. It should be between -413 and 8848");
                return false;
            }

            double utcOffset = location.UtcOffset;
            if (utcOffset < -12 || utcOffset > 12)
            {
                BH.Engine.Base.Compute.RecordError("Invalid UtcOffset passed. It should be between -12 and 12");
                return false;
            }

            return true;
        }
    }
}






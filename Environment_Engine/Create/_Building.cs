/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2018, the respective contributors. All rights reserved.
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

using BH.oM.Environment.Elements;

namespace BH.Engine.Environment
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Building Building(string name, double latitude, double longitude, double elevation, BH.oM.Geometry.Point location)
        {
            return new Building
            {
                Name = name,
                Latitude = latitude,
                Longitude = longitude,
                Elevation = elevation,
                Location = location,
            };
        }

        public static Building Building(string name, double latitude, double longitude, double elevation)
        {
            return new Building
            {
                Name = name,
                Latitude = latitude,
                Longitude = longitude,
                Elevation = elevation,
            };
        }

        public static Building Building(string name)
        {
            return new Building
            {
                Name = name,
            };
        }

        public static Building Building(double latitude, double longitude)
        {
            return new Building
            {
                Latitude = latitude,
                Longitude = longitude,
            };
        }

        public static Building Building(double elevation)
        {
            return new Building
            {
                Elevation = elevation,
            };
        }

        public static Building Building(BH.oM.Geometry.Point location)
        {
            return new Building
            {
                Location = location,
            };
        }

        public static Building Building(double elevation, double latitude, double longitude, BH.oM.Geometry.Point location)
        {
            return new Building
            {
                Elevation = elevation,
                Latitude = latitude,
                Longitude = longitude,
                Location = location,
            };
        }

        /***************************************************/
    }
}

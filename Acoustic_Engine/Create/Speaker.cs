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

using BH.oM.Acoustic;
using BH.oM.Geometry;
using System.Collections.Generic;

namespace BH.Engine.Acoustic
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Speaker Speaker(Point location, Vector direction = null, double emissiveLevel = 100, string category = "Omni")
        {
            return new Speaker()
            {
                Location = location,
                Direction = direction,
                EmissiveLevel = emissiveLevel,
                Category = category,
                Gains = new Dictionary<Frequency, double>() { { Frequency.Hz500, 1.6 }, { Frequency.Hz2000, 5.3 } }
            };
        }

        /***************************************************/

        public static Speaker Speaker(Point location, Vector direction, string category, Dictionary<Frequency, double> gains = null, Dictionary<Frequency, double[,]> directivity = null)
        {
            return new Speaker()
            {
                Location = location,
                Direction = direction,
                Category = category,
                Gains = gains,
                Directivity = directivity,
            };
        }

        /***************************************************/
    }
}







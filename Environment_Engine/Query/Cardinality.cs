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
using BH.oM.Environment;

using BH.Engine.Geometry;
using BH.oM.Geometry;

using BH.oM.Reflection.Attributes;
using System.ComponentModel;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns the cardinal orientation of a given angle, where that angle is related to north at 0°")]
        [Input("degreesFromNorth", "The angle in degrees from north (at 0°)")]
        [Input("directions", "The number of cardinal directions into which angles shall be binned (This value should be one of 4, 8, 16 or 32, and is centred about \"north\")")]
        [Output("cardinality", "The cardinal direction the angle represents")]
        public static string Cardinality(double degreesFromNorth, int directions = 8)
        {
            if (!new List<int>() { 4, 8, 16, 32 }.Contains(directions))
            {
                BH.Engine.Reflection.Compute.RecordError("The number of cardinal directions must be either 4, 8, 16 or 32");
                return null;
            }

            if (degreesFromNorth > 360)
            {
                BH.Engine.Reflection.Compute.RecordError("The angle provided must be less than or equal to 360");
                return null;
            }

            Dictionary<int, List<string>> cardinalDirections = new Dictionary<int, List<string>>()
            {
                { 4, new List<string>() { "N", "E", "S", "W" } },
                { 8, new List<string>() { "N", "NE", "E", "SE", "S", "SW", "W", "NW" } },
                { 16, new List<string>() { "N", "NNE", "NE", "ENE", "E", "ESE", "SE", "SSE", "S", "SSW", "SW", "WSW", "W", "WNW", "NW", "NNW" } },
                { 32, new List<string>() { "N", "NbE", "NNE", "NEbN", "NE", "NEbE", "ENE", "EbN", "E", "EbS", "ESE", "SEbE", "SE", "SEbS", "SSE", "SbE", "S", "SbW", "SSW", "SWbS", "SW", "SWbW", "WSW", "WbS", "W", "WbN", "WNW", "NWbW", "NW", "NWbN", "NNW", "NbW" } }
            };

            int n = 0;
            foreach (string cardinalDirection in cardinalDirections[directions])
            {
                double angleBetween = 360 / (double)cardinalDirections[directions].Count;
                double halfAngleBetween = angleBetween / 2;
                if (n == 0)
                {
                    if (degreesFromNorth >= 360 - halfAngleBetween || degreesFromNorth < halfAngleBetween)
                    {
                        return cardinalDirection;
                    }
                }
                else
                {
                    if (degreesFromNorth >= (n * angleBetween) - halfAngleBetween && degreesFromNorth < (n * angleBetween) + halfAngleBetween)
                    {
                        return cardinalDirection;
                    }
                }
                n += 1;
            }
            return null;
        }
    }
}

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

using System.Collections.Generic;

using BH.oM.Base.Attributes;
using System.ComponentModel;
using System;
using System.Linq;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns the cardinal orientation of a given angle, where that angle is related to north at 0 radians")]
        [Input("angleFromNorth", "The angle to north in radians (+Ve is interpreted as clockwise from north at 0.0 radians)")]
        [Input("directions", "The number of cardinal directions into which angles shall be binned (This value should be one of 4, 8, 16 or 32, and is centred about \"north\")")]
        [Output("cardinality", "The cardinal direction the angle represents")]
        public static string Cardinality(double angleFromNorth, int directions = 8)
        {
            if (angleFromNorth > Math.PI * 2 || angleFromNorth < -Math.PI * 2)
            {
                BH.Engine.Base.Compute.RecordWarning("The angle entered is beyond the normally expected range for an angle in radians.");
            }

            Dictionary<int, List<string>> cardinalDirections = new Dictionary<int, List<string>>()
            {
                { 4, new List<string>() { "N", "E", "S", "W" } },
                { 8, new List<string>() { "N", "NE", "E", "SE", "S", "SW", "W", "NW" } },
                { 16, new List<string>() { "N", "NNE", "NE", "ENE", "E", "ESE", "SE", "SSE", "S", "SSW", "SW", "WSW", "W", "WNW", "NW", "NNW" } },
                { 32, new List<string>() { "N", "NbE", "NNE", "NEbN", "NE", "NEbE", "ENE", "EbN", "E", "EbS", "ESE", "SEbE", "SE", "SEbS", "SSE", "SbE", "S", "SbW", "SSW", "SWbS", "SW", "SWbW", "WSW", "WbS", "W", "WbN", "WNW", "NWbW", "NW", "NWbN", "NNW", "NbW" } }
            };

            if (!cardinalDirections.Keys.ToList().Contains(directions))
            {
                BH.Engine.Base.Compute.RecordError(String.Format("The number of cardinal directions must be in [{0}]", string.Join(", ", cardinalDirections.Keys)));
                return null;
            }

            angleFromNorth = angleFromNorth * 180 / Math.PI;

            int n = 0;
            foreach (string cardinalDirection in cardinalDirections[directions])
            {
                double angleBetween = 360 / (double)cardinalDirections[directions].Count;
                double halfAngleBetween = angleBetween / 2;
                if (n == 0)
                {
                    if (angleFromNorth >= 360 - halfAngleBetween || angleFromNorth < halfAngleBetween)
                    {
                        return cardinalDirection;
                    }
                }
                else
                {
                    if (angleFromNorth >= (n * angleBetween) - halfAngleBetween && angleFromNorth < (n * angleBetween) + halfAngleBetween)
                    {
                        return cardinalDirection;
                    }
                }
                n++;
            }
            BH.Engine.Base.Compute.RecordError(String.Format("No cardinal direction found that matches {0:F3} degrees (or {1:F3} radians from north).", angleFromNorth, angleFromNorth * Math.PI / 180));
            return null;
        }
    }
}





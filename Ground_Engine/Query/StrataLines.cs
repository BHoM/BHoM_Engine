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

using System.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using BH.oM.Base.Attributes;
using BH.oM.Geometry;
using BH.oM.Ground;
using BH.oM.Quantities.Attributes;
using BH.Engine.Base;
using BH.Engine.Geometry;


namespace BH.Engine.Ground
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Generates a list of lines relating to the strata within the Borehole which can be used for visualisation purposes.")]
        [Input("borehole", "The Borehole from which to produce the lines representing the strata.")]
        [Output("strata", "A list of lines representing the strata from the Borehole.")]
        public static List<Line> StrataLines(this Borehole borehole)
        {
            List<Line> lines = new List<Line>();

            // Null checks
            if (IsValid(borehole))
            {
                Vector direction = Geometry.Create.Vector(borehole.Top, borehole.Bottom).Normalise();
                foreach (Stratum stratum in borehole.Strata)
                {
                    Point start = borehole.Top.Translate(direction * stratum.Top);
                    Point bottom = borehole.Top.Translate(direction * stratum.Bottom);
                    Line line = new Line() { Start = start, End = bottom };
                    lines.Add(line);
                }
            }
            else
                return null;

            return lines;
        }

    }
}





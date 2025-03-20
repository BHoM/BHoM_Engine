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
using System.Text;
using System.Threading.Tasks;

using BH.oM.Analytical.Elements;
using BH.oM.Base.Attributes;

using BH.oM.Environment.Elements;
using BH.oM.Geometry;
using BH.Engine.Geometry;
using BH.oM.Spatial.SettingOut;

using BH.oM.Quantities.Attributes;
using BH.Engine.Spatial;
 
using System.Security.Cryptography.X509Certificates;

using System.ComponentModel;

namespace BH.Engine.Environment
{
    public static partial class Create
    {
        [Description("Create a collection of levels from a list of regions")]
        [Input("regions", "A collection of BHoM Regions to get the level from.")]
        [Input("decimals", "Provide decimal location to define the degree of tolerance for data matching.")]
        [Input("angleTolerance", "The tolerance used for angle calculations.")]
        [Output("levels", "A collection of BHoM Levels based on the provided regions. If region has several z-values both the minimum and the maximum levels are created")]
        public static List<Level> Levels(List<IRegion> regions, int decimals = 6, double angleTolerance = BH.oM.Geometry.Tolerance.Angle)
        {          
            List<Level> levels = new List<Level>();

            foreach (IRegion region in regions)
            {
                Polyline regionPolyline = region.Perimeter.ICollapseToPolyline(angleTolerance);
                double minLevel = regionPolyline.MinimumLevel(decimals);
                double maxLevel = regionPolyline.MaximumLevel(decimals);

                levels.Add(BH.Engine.Spatial.Create.Level(Math.Round(minLevel, decimals)));

                if (minLevel != maxLevel)
                    levels.Add(BH.Engine.Spatial.Create.Level(Math.Round(maxLevel, decimals)));                   
            }

            return levels;
        }
    }
}





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

using BH.oM.Architecture.Elements;

using BH.oM.Geometry;
using BH.Engine.Geometry;
using BH.oM.Reflection;
using System.Runtime.InteropServices;

namespace BH.Engine.Architecture
{
    public static partial class Compute
    {
        public static List<CeilingTile> CeilingTiles(Ceiling ceiling, List<Line> ceilingTileLines, double angleTolerance = BH.oM.Geometry.Tolerance.Angle, double distanceTolerance = BH.oM.Geometry.Tolerance.Distance, int decimalPlaces = 6)
        {
            List<Line> openingLines = ceiling.Surface.IInternalEdges().SelectMany(x => x.ISubParts()).Cast<Line>().ToList();

            ceilingTileLines.AddRange(openingLines);

            Polyline outerPerimeter = ceiling.Surface.IExternalEdges().Select(x => x.ICollapseToPolyline(angleTolerance)).ToList().Join()[0];

            List<Polyline> regions = BH.Engine.Geometry.Compute.Split(outerPerimeter, ceilingTileLines, distanceTolerance, decimalPlaces);

            List<CeilingTile> tiles = new List<CeilingTile>();

            foreach (Polyline p in regions)
                tiles.Add(new CeilingTile { Perimeter = p });

            return tiles;
        }
    }
}

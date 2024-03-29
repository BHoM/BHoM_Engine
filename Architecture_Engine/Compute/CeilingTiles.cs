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

using BH.Engine.Geometry;
using BH.oM.Architecture.Elements;
using BH.oM.Base.Attributes;
using BH.oM.Geometry;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.Architecture
{
    public static partial class Compute
    {
        [Description("Generate a collection of Ceiling Tile objects that can sit within the given ceiling. Uses the BH.Engine.Geometry.Compute.Split(Polyline, List<Line>) method for its core.")]
        [Input("ceiling", "Ceiling object which provides the outer perimeter of the ceiling tiles")]
        [Input("ceilingTileLines", "The lines across the ceiling which will be used to cut the ceiling into individual tiles.")]
        [Input("angleTolerance", "Tolerance used for angle calculations. Default set to BH.oM.Geometry.Tolerance.Angle.")]
        [Input("distanceTolerance", "Tolerance used for distance calculations. Default set to BH.oM.Geometry.Tolerance.Distance")]
        [Output("ceilingTiles", "Closed Ceiling Tile regions contained within the Ceiling.")]
        public static List<CeilingTile> CeilingTiles(Ceiling ceiling, List<Line> ceilingTileLines, double angleTolerance = BH.oM.Geometry.Tolerance.Angle, double distanceTolerance = BH.oM.Geometry.Tolerance.Distance)
        {
            if(ceiling == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot compute the ceiling tiles for a null ceiling.");
                return new List<CeilingTile>();
            }

            List<Line> openingLines = ceiling.Surface.IInternalEdges().SelectMany(x => x.ISubParts()).Cast<Line>().ToList();

            ceilingTileLines.AddRange(openingLines);

            Polyline outerPerimeter = ceiling.Surface.IExternalEdges().Select(x => x.ICollapseToPolyline(angleTolerance)).ToList().Join()[0];

            List<Polyline> regions = BH.Engine.Geometry.Compute.Split(outerPerimeter, ceilingTileLines, distanceTolerance);

            List<CeilingTile> tiles = new List<CeilingTile>();

            foreach (Polyline p in regions)
                tiles.Add(new CeilingTile { Perimeter = p });

            return tiles;
        }
    }
}




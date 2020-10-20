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

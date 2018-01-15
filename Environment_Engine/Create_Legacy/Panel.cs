using BH.oM.Environmental.Elements_Legacy;
using BH.oM.Environmental.Properties_Legacy;
using System.Collections.Generic;
using BH.oM.Geometry;

namespace BH.Engine.Environment
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Panel Panel(Polyline edges, List<Opening> openings, string type, SurfaceDataProperties surfaceData, BuildingElement buildingElements, CFDProperties cdfProperties)
        {
            return new Panel
            {
                Edges = edges,
                Openings = openings,
                SurfaceData = surfaceData,
                BuildingElements = buildingElements,
                CDFProperties = cdfProperties
            };
        }

        /***************************************************/
    }
}

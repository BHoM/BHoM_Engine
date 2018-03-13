using System;
using System.Collections.Generic;
using System.Linq;
using BHEE = BH.oM.Environmental.Elements;
using BH.Engine.Geometry;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static float GetFloorArea(this BHEE.Space bHoMSpace)
        {
            float floorArea;
            List<BHEE.BuildingElement> bHoMBuildingElement = bHoMSpace.BuildingElements;
            List<double> areaSum = new List<double>();
            foreach (BHEE.BuildingElement element in bHoMBuildingElement)
            {
                if (Inclination(element.BuildingElementGeometry) == 180) // if floor
                {
                    floorArea = (float)element.BuildingElementGeometry.ICurve().IArea();
                    areaSum.Add(floorArea); //if we have many floor surfaces in the same space we ned to calculate the sum
                }
            }
            return (float)areaSum.Sum();
        }

        /***************************************************/
    }
}

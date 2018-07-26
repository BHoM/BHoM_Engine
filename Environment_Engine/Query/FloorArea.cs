using System;
using System.Collections.Generic;
using System.Linq;
using BHEE = BH.oM.Environment.Elements;
using BH.Engine.Geometry;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static double FloorArea(this BHEE.Space bHoMSpace)
        {
            double floorArea;
            List<BHEE.BuildingElement> bHoMBuildingElement = bHoMSpace.BuildingElements;
            List<double> areaSum = new List<double>();

            foreach (BHEE.BuildingElement element in bHoMBuildingElement)
            {
                if (Tilt(element.BuildingElementGeometry) == 180) // if floor
                {
                    floorArea = element.BuildingElementGeometry.ICurve().IArea();
                    areaSum.Add(floorArea); //if we have many floor surfaces in the same space we need to calculate the sum
                }
            }
            return areaSum.Sum();
        }

        /***************************************************/
    }
}

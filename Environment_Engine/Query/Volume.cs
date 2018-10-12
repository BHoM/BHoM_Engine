using System;
using System.Collections.Generic;
using System.Linq;
using BH.oM.Environment.Elements;
using BH.Engine.Geometry;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static double Volume(this Space bHoMSpace)
        {
            //TODO: This does only work for a space where all of the building element panels have the same height. Make it work for all spaces

            /*List<BHEE.BuildingElement> bHoMBuildingElement = bHoMSpace.BuildingElements;

            double roomheight = 0;
            foreach (BHEE.BuildingElement element in bHoMBuildingElement)
            {
                if (Tilt(element.BuildingElementGeometry) == 90) // if wall
                {
                    roomheight = AltitudeRange(element.BuildingElementGeometry);
                    break;
                }
            }

            return FloorArea(bHoMSpace) * roomheight;*/

            throw new NotImplementedException("Calculating the volume of a space has not been implemented yet");
        }

        /***************************************************/

        public static double Volume(this List<BuildingElement> space)
        {
            //TODO: Make this more accurate for irregular spaces
            double maxHeight = 0;
            foreach (BuildingElement be in space)
                maxHeight = Math.Max(maxHeight, (be.MaximumLevel() - be.MinimumLevel()));

            double area = space.Area();

            return area * maxHeight;
        }
    }
}

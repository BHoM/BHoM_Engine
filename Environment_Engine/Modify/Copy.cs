using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.oM.Structural.Elements;
using BH.oM.Geometry;
using BH.Engine.Geometry;
using BH.oM.Environmental.Elements;
using BH.oM.Environmental.Interface;

namespace BH.Engine.Environment
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static BuildingElementCurve Copy(this BuildingElementCurve buildingElementCurve)
        {
            BuildingElementCurve aBuildingElementCurve = buildingElementCurve.GetShallowClone(true) as BuildingElementCurve;
            aBuildingElementCurve.Curve = buildingElementCurve.Curve.IClone();
            return aBuildingElementCurve;
        }

        /***************************************************/

        public static BuildingElementPanel Copy(this BuildingElementPanel buildingElementPanel)
        {
            BuildingElementPanel aBuildingElementPanel = buildingElementPanel.GetShallowClone(true) as BuildingElementPanel;
            aBuildingElementPanel.PolyCurve = buildingElementPanel.PolyCurve.IClone() as PolyCurve;
            return aBuildingElementPanel;
        }

        /***************************************************/

        public static IBuildingElementGeometry Copy(this IBuildingElementGeometry buildingElementGeometry)
        {
            IBuildingElementGeometry aBuildingElementGeometry = Copy(buildingElementGeometry as dynamic);
            return aBuildingElementGeometry;
        }

        /***************************************************/

        public static Storey Copy(this Storey storey)
        {
            return storey.GetShallowClone(true) as Storey;
        }

        /***************************************************/

        public static BuildingElement Copy(this BuildingElement buildingElement)
        {
            BuildingElement aBuildingElement = buildingElement.GetShallowClone(true) as BuildingElement;
            if (buildingElement.BuildingElementGeometry != null)
                aBuildingElement.BuildingElementGeometry = aBuildingElement.BuildingElementGeometry.Copy();
            return aBuildingElement;
        }

        /***************************************************/

        public static Storey Copy(this Storey storey, string name, double elevation)
        {
            Storey aStorey = storey.Copy();
            aStorey.Name = name;
            aStorey.Elevation = elevation;

            return aStorey;
        }

        /***************************************************/

        public static IBuildingElementGeometry Copy(this IBuildingElementGeometry buildingElementGeometry, Vector vector)
        {
            IBuildingElementGeometry aBuildingElementGeometry = Copy(buildingElementGeometry);
            aBuildingElementGeometry.Move(vector);
            return aBuildingElementGeometry;
        }

        /***************************************************/

        public static BuildingElement Copy(this BuildingElement buildingElement, Vector vector)
        {
            BuildingElement aBuildingElement = buildingElement.Copy();
            aBuildingElement.Move(vector);
            return aBuildingElement;
        }

        /***************************************************/

        public static BuildingElement Copy(this BuildingElement buildingElement, Storey storey)
        {
            BuildingElement aBuildingElement = buildingElement.Copy();
            aBuildingElement.Move(storey);
            return aBuildingElement;
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/



        /***************************************************/
    }
}

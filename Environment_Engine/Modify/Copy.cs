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

        public static BuildingElementCurve Copy(this BuildingElementCurve BuildingElementCurve)
        {
            BuildingElementCurve aBuildingElementCurve = BuildingElementCurve.GetShallowClone(true) as BuildingElementCurve;
            aBuildingElementCurve.Curve = BuildingElementCurve.Curve.IClone();
            return aBuildingElementCurve;
        }

        public static BuildingElementPanel Copy(this BuildingElementPanel BuildingElementPanel)
        {
            BuildingElementPanel aBuildingElementPanel = BuildingElementPanel.GetShallowClone(true) as BuildingElementPanel;
            aBuildingElementPanel.PolyCurve = BuildingElementPanel.PolyCurve.IClone() as PolyCurve;
            return aBuildingElementPanel;
        }

        public static IBuildingElementGeometry Copy(this IBuildingElementGeometry BuildingElementGeometry)
        {
            IBuildingElementGeometry aBuildingElementGeometry = Copy(BuildingElementGeometry as dynamic);
            return aBuildingElementGeometry;
        }

        public static Storey Copy(this Storey Storey)
        {
            return Storey.GetShallowClone(true) as Storey;
        }

        public static BuildingElement Copy(this BuildingElement BuildingElement)
        {
            BuildingElement aBuildingElement = BuildingElement.GetShallowClone(true) as BuildingElement;
            if (BuildingElement.BuildingElementGeometry != null)
                aBuildingElement.BuildingElementGeometry = aBuildingElement.BuildingElementGeometry.Copy();
            return aBuildingElement;
        }

        /***************************************************/

        public static Storey Copy(this Storey Storey, string Name, double Elevation)
        {
            Storey aStorey = Storey.Copy();
            aStorey.Name = Name;
            aStorey.Elevation = Elevation;

            return aStorey;
        }

        public static IBuildingElementGeometry Copy(this IBuildingElementGeometry BuildingElementGeometry, Vector Vector)
        {
            IBuildingElementGeometry aBuildingElementGeometry = Copy(BuildingElementGeometry);
            aBuildingElementGeometry.Move(Vector);
            return aBuildingElementGeometry;
        }

        public static BuildingElement Copy(this BuildingElement BuildingElement, Vector Vector)
        {
            BuildingElement aBuildingElement = BuildingElement.Copy();
            aBuildingElement.Move(Vector);
            return aBuildingElement;
        }

        public static BuildingElement Copy(this BuildingElement BuildingElement, Storey Storey)
        {
            BuildingElement aBuildingElement = BuildingElement.Copy();
            aBuildingElement.Move(Storey);
            return aBuildingElement;
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/



        /***************************************************/
    }
}

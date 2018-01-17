using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.oM.Environmental.Elements;
using BH.oM.Environmental.Properties;
using BH.oM.Structural.Elements;
using BH.oM.Geometry;

namespace BH.Engine.Environment
{
    public static partial class Create
    {
        public static BuildingElement BuildingElement(BuildingElementProperties BuildingElementProperties, BuildingElementPanel BuildingElementPanel)
        {
            return new BuildingElement()
            {
                BuildingElementProperties = BuildingElementProperties,
                BuildingElementGeometry = BuildingElementPanel
            };
        }

        public static BuildingElement BuildingElement(BuildingElementProperties BuildingElementProperties, IEnumerable<Polyline> PolyLines)
        {
            return new BuildingElement()
            {
                BuildingElementProperties = BuildingElementProperties,
                BuildingElementGeometry = Create.BuildingElementPanel(PolyLines)
            };
        }

        public static BuildingElement BuildingElement(BuildingElementProperties BuildingElementProperties, ICurve Curve)
        {
            return new BuildingElement()
            {
                BuildingElementProperties = BuildingElementProperties,
                BuildingElementGeometry = Create.BuildingElementPanel(new ICurve[] { Curve })
            };
        }

        public static BuildingElement BuildingElement(BuildingElementProperties BuildingElementProperties, BuildingElementCurve BuildingElementCurve, Storey Storey)
        {
            return new BuildingElement()
            {
                Storey = Storey,
                BuildingElementProperties = BuildingElementProperties,
                BuildingElementGeometry = BuildingElementCurve
            };
        }
    }
}

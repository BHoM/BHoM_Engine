using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.oM.Environmental.Elements;
using BH.oM.Geometry;
using BH.oM.Environmental.Interface;

using BH.Engine.Geometry;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static ICurve Bottom(this IBuildingElementGeometry buildingElementGeometry)
        {
            if (buildingElementGeometry is BuildingElementCurve)
            {
                return buildingElementGeometry.ICurve();
            }
            else if (buildingElementGeometry is BuildingElementPanel)
            {
                BuildingElementPanel aBuildingElementPanel = buildingElementGeometry as BuildingElementPanel;
                double aZ = double.MaxValue;
                ICurve aResult = null;
                foreach (ICurve aCurve in aBuildingElementPanel.PolyCurve.Curves)
                {
                    Point aPoint_Start = aCurve.IStartPoint();
                    Point aPoint_End = aCurve.IEndPoint();

                    if (aPoint_End.Z <= aZ && aPoint_Start.Z <= aZ)
                    {
                        aZ = Math.Max(aPoint_End.Z, aPoint_Start.Z);
                        aResult = aCurve;
                    }
                }
                return aResult;
            }
            else
            {
                return buildingElementGeometry.ICurve();
            }
        }

        /***************************************************/
    }
}

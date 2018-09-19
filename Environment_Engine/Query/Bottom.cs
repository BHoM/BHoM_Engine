using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.oM.Environment.Elements;
using BH.oM.Geometry;
using BH.oM.Environment.Interface;

using BH.Engine.Geometry;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static ICurve Bottom(this IBuildingObject buildingElementGeometry)
        {
            if (buildingElementGeometry == null) return null;

            PolyCurve workingCurves = null;

            if (buildingElementGeometry is Panel)
                workingCurves = (buildingElementGeometry as Panel).PanelCurve as PolyCurve;
            else if (buildingElementGeometry is BuildingElement)
                workingCurves = (buildingElementGeometry as BuildingElement).PanelCurve as PolyCurve;
            else if (buildingElementGeometry is Opening)
                workingCurves = (buildingElementGeometry as Opening).OpeningCurve as PolyCurve;

            if (workingCurves == null) return null;

            double aZ = double.MaxValue;
            ICurve aResult = null;
            foreach (ICurve aCurve in workingCurves.Curves)
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

        /***************************************************/
    }
}

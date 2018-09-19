using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BHE = BH.oM.Environment.Elements;
using BH.oM.Environment.Interface;
using BHG = BH.oM.Geometry;
using BH.Engine.Geometry;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static BHG.Point Centre(this BHE.Space space) //TODO: This does only work for convex spaces. we need to change this method later
        {
            /*List<BHE.BuildingElement> buildingElements = space.BuildingElements;
            List<BHG.Point> pts = new List<BHG.Point>();

            foreach (BHE.BuildingElement element in buildingElements)
            {
                pts.AddRange(Engine.Geometry.Query.IControlPoints(element.BuildingElementGeometry.ICurve()));
            }

            BHG.Point centrePt = BH.Engine.Geometry.Query.Bounds(pts).Centre();
            return centrePt;*/

            return new oM.Geometry.Point();
        }

        /***************************************************/
    }
}

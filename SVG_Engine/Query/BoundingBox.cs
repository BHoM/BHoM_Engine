using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.SVG
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        //public static class IBoundingBox(this IBHoMGeometry geometry)
        //{
        //return BoundingBox(geometry as dynamic);
        //}

        public static BoundingBox BoundingBox(this List<IBHoMGeometry> objList)
        {

            List<BH.oM.Geometry.Point> ptList = new List<BH.oM.Geometry.Point>();

            for (int i = 0; i < objList.Count; i++)
            {
                var geometry = objList[i];

                if (geometry != null)
                {
                    if (geometry is BH.oM.Geometry.Point)
                    {
                        BH.oM.Geometry.Point pt = geometry as BH.oM.Geometry.Point;
                        ptList.Add(pt);
                    }
                    else if (geometry is BH.oM.Geometry.Line)
                    {
                        BH.oM.Geometry.Line line = geometry as BH.oM.Geometry.Line;
                        BH.oM.Geometry.Point startPt = line.Start();
                        BH.oM.Geometry.Point endPt = line.End();

                        ptList.Add(startPt);
                        ptList.Add(endPt);
                    }
                }
            }

            BH.oM.Geometry.BoundingBox CanvasBox = new BH.oM.Geometry.BoundingBox(ptList[0],ptList[1]);

            return CanvasBox;
        }


        

        //public BH.oM.Geometry.BoundingBox defineBounds(List<BH.oM.Geometry.IBHoMGeometry>)


    }
}

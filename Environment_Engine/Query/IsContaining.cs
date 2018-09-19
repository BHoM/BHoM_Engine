using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BHE = BH.oM.Environment.Elements;
using BHG = BH.oM.Geometry;
using BH.Engine.Geometry;
using BH.Engine.Environment;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static bool IsContaining(this BHE.Space space, BHG.Point point)
        {
            /*List<BHE.BuildingElement> buildingElements = space.BuildingElements;
            List<BHG.Plane> planes = buildingElements.Select(x => x.BuildingElementGeometry.ICurve().IControlPoints().FitPlane()).ToList();
            List<BHG.Point> ctrPoints = buildingElements.SelectMany(x => x.BuildingElementGeometry.ICurve().IControlPoints()).ToList();
            BHG.BoundingBox boundingBox = BH.Engine.Geometry.Query.Bounds(ctrPoints);

            if (!BH.Engine.Geometry.Query.IsContaining(boundingBox, point)) return false;

            //We need to check one line that starts in the point and end outside the bounding box
            BHG.Vector vector = new BHG.Vector() { X = 1, Y = 0, Z = 0 };
            //Use a length longer than the longest side in the bounding box. Change to infinite line?
            BHG.Line line = new BHG.Line() { Start = point, End = point.Translate(vector * (((boundingBox.Max - boundingBox.Min).Length()) * 10)) };

            //Check intersections
            int counter = 0;
            List<BHG.Point> intersectPoints = new List<BHG.Point>();

            for(int x = 0; x < planes.Count; x++)
            {
                if ((BH.Engine.Geometry.Query.PlaneIntersection(line, planes[x], false)) == null) continue;

                List<BHG.Point> intersectingPoints = new List<oM.Geometry.Point>();
                intersectingPoints.Add(BH.Engine.Geometry.Query.PlaneIntersection(line, planes[x]));
                BHG.Polyline pLine = new BHG.Polyline() { ControlPoints = buildingElements[x].BuildingElementGeometry.ICurve().IControlPoints() };

                if(intersectingPoints != null && BH.Engine.Geometry.Query.IsContaining(pLine, intersectingPoints, true, 1e-05))
                {
                    intersectPoints.AddRange(intersectingPoints);
                    if (intersectPoints.CullDuplicates().Count == intersectPoints.Count()) //Check if the point already has been added to the list
                        counter++;
                }
            }

            return ((counter % 2) == 0); //If the number of intersections is odd the point is outsde the space*/
        }
    }
}

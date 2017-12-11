using BH.oM.Geometry;
using BH.oM.Structural.Elements;
using BH.oM.Structural.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Structure
{
    public static partial class Query
    {

        //***************************************************/
        //**** Public Methods                            ****/
        //***************************************************/

        public static List<IBHoMGeometry> GetGeometry(this ConcreteSection section)
        {
            if (section.Edges.Count == 0)
                return null;

            CompositeGeometry geom = new CompositeGeometry(section.Edges);
            geom.Elements.AddRange(section.GetReinforcementLayout().Elements);

            return geom.Elements;
        }


        //public static IBHoMGeometry GetGeometry(this Bar bar)
        //{
        //    return new Line(bar.StartNode.Point, bar.EndNode.Point);
        //}

        ///***************************************************/

        //public static IBHoMGeometry GetGeometry(this Node node)
        //{
        //    return node.Point;
        //}

        ///***************************************************/

        //public static IBHoMGeometry GetGeometry(this Contour contour)
        //{
        //    return contour.Surface;
        //}

        ///***************************************************/

        //public static IBHoMGeometry GetGeometry(this Storey storey)
        //{
        //    return new Plane(new Point(0, 0, storey.Elevation), new Vector(0, 0, 1));
        //}

        /***************************************************/

        //public static IBHoMGeometry GetGeometry(this FEMesh feMesh)
        //{
        //    IEnumerable<Point> points = feMesh.Nodes.Select(x => x.Point);
        //    IEnumerable<BH.oM.Geometry.Face> faces = feMesh.Faces.Select( x => x.

        //    return new Mesh(points, feMesh.Faces);
        //}
    }

}

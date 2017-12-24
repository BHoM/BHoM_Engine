using BH.oM.Geometry;
using BH.oM.Structural.Elements;
using BH.oM.Structural.Properties;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Structure
{
    public static partial class Query
    {

        //***************************************************/
        //**** Public Methods                            ****/
        //***************************************************/

        public static List<IBHoMGeometry> Geometry(this ConcreteSection section)
        {
            if (section.Edges.Count == 0)
                return null;

            CompositeGeometry geom = Engine.Geometry.Create.CompositeGeometry(section.Edges);
            geom.Elements.AddRange(section.GetReinforcementLayout().Elements);

            return geom.Elements;
        }

        public static IBHoMGeometry Geometry(this Bar bar)
        {
            return new Line { Start = bar.StartNode.Point, End = bar.EndNode.Point };
        }

        /***************************************************/

        public static IBHoMGeometry Geometry(this Node node)
        {
            return node.Point;
        }

        /***************************************************/

        public static IBHoMGeometry Geometry(this PanelFreeForm contour)
        {
            return contour.Surface;
        }

        /***************************************************/

        public static IBHoMGeometry Geometry(this Storey storey)
        {
            return new Plane(new Point(0, 0, storey.Elevation), new Vector(0, 0, 1));
        }

        /***************************************************/

        public static IBHoMGeometry Geometry(this SteelSection section)
        {
            return new CompositeGeometry { Elements = section.Edges.ToList<IBHoMGeometry>() };
        }


        //public static IBHoMGeometry Geometry(this Bar bar)
        //{
        //    return new Line(bar.StartNode.Point, bar.EndNode.Point);
        //}

        ///***************************************************/

        //public static IBHoMGeometry Geometry(this Node node)
        //{
        //    return node.Point;
        //}

        ///***************************************************/

        //public static IBHoMGeometry Geometry(this Contour contour)
        //{
        //    return contour.Surface;
        //}

        ///***************************************************/

        //public static IBHoMGeometry Geometry(this Storey storey)
        //{
        //    return new Plane(new Point(0, 0, storey.Elevation), new Vector(0, 0, 1));
        //}

        /***************************************************/

        //public static IBHoMGeometry Geometry(this FEMesh feMesh)
        //{
        //    IEnumerable<Point> points = feMesh.Nodes.Select(x => x.Point);
        //    IEnumerable<BH.oM.Geometry.Face> faces = feMesh.Faces.Select( x => x.

        //    return new Mesh(points, feMesh.Faces);
        //}
    }

}

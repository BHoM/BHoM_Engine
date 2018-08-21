using BH.oM.Geometry;
using BH.oM.Structure.Elements;
using BH.oM.Structure.Properties;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Structure
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static CompositeGeometry Geometry(this ConcreteSection section)
        {
            if (section.SectionProfile.Edges.Count == 0)
                return null;

            CompositeGeometry geom = Engine.Geometry.Create.CompositeGeometry(section.SectionProfile.Edges);
            geom.Elements.AddRange(section.Layout().Elements);

            return geom;
        }

        /***************************************************/

        public static Line Geometry(this Bar bar)
        {
            return bar.Centreline();
        }

        /***************************************************/

        public static Point Geometry(this Node node)
        {
            return node.Position;
        }

        /***************************************************/

        public static IGeometry Geometry(this PanelFreeForm contour)
        {
            return contour.Surface;
        }

        /***************************************************/

        public static IGeometry Geometry(this SteelSection section)
        {
            return new CompositeGeometry { Elements = section.SectionProfile.Edges.ToList<IGeometry>() };
        }

        /***************************************************/

        public static Mesh Geometry(this MeshFace meshFace)
        {

            return new Mesh()
            {
                Vertices = meshFace.Nodes.Select(x => x.Position).ToList(),
                Faces = meshFace.Nodes.Count == 3 ? new List<Face>() { new Face() { A = 0, B = 1, C = 2 } } : new List<Face>() { new Face() { A = 0, B = 1, C = 2, D = 3 } }
            };
                
        }

        /***************************************************/

        public static IGeometry Geometry(this RigidLink link)
        {
            List<IGeometry> lines = new List<IGeometry>();

            foreach (Node sn in link.SlaveNodes)
            {
                lines.Add(new Line() { Start = link.MasterNode.Position, End = sn.Position });
            }
            return new CompositeGeometry() { Elements = lines };
        }

        /***************************************************/

        public static ICurve Geometry(this FramingElement element)
        {
            return element.LocationCurve;
        }

        /***************************************************/
    }

}

using BH.oM.Geometry;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Geometry
{
    public static partial class Transform
    {
        /***************************************************/
        /**** Public Methods - Curves                   ****/
        /***************************************************/

        public static List<Line> GetExploded(this Polyline curve)
        {
            List<Line> result = new List<Line>();

            List<Point> pts = curve.ControlPoints;

            for (int i = 1; i < pts.Count; i++)
                result.Add(new Line(pts[i - 1], pts[i]));

            return result;
        }

        /***************************************************/

        public static List<ICurve> GetExploded(this PolyCurve curve)
        {
            List<ICurve> exploded = new List<ICurve>();
            List<ICurve> curves = curve.Curves;

            for (int i = 0; i < curves.Count; i++)
                exploded.AddRange(curves[i].IGetExploded());

            return exploded;
        }

        /***************************************************/
        /**** Public Methods - Surfaces                 ****/
        /***************************************************/

        public static List<ISurface> GetExploded(this PolySurface surface)
        {
            List<ISurface> exploded = new List<ISurface>();
            List<ISurface> surfaces = surface.Surfaces;

            for (int i = 0; i < surfaces.Count; i++)
                exploded.AddRange(surfaces[i].IGetExploded());

            return exploded;
        }

        /***************************************************/
        /**** Public Methods - Meshes                   ****/
        /***************************************************/

        public static List<Mesh> GetExploded(this Mesh mesh)
        {
            List<Mesh> explodedMeshes = new List<Mesh>();
            List<Face> faces = mesh.Faces;
            List<Point> vertices = mesh.Vertices;
            for (int i = 0; i < faces.Count; i++)
            {
                Face localFace = new Face(0, 1, 2);
                List<Point> localVertices = new List<Point>();
                localVertices.Add(vertices[faces[i].A]);
                localVertices.Add(vertices[faces[i].B]);
                localVertices.Add(vertices[faces[i].C]);
                if (faces[i].IsQuad())
                {
                    localVertices.Add(vertices[faces[i].D]);
                    localFace.D = 3;
                }
                explodedMeshes.Add(new Mesh(localVertices, new List<Face>() { localFace }));
            }
            return explodedMeshes;
        }


        /***************************************************/
        /**** Public Methods - Others                   ****/
        /***************************************************/

        public static List<IBHoMGeometry> GetExploded(this CompositeGeometry group)
        {
            List<IBHoMGeometry> exploded = new List<IBHoMGeometry>();
            List<IBHoMGeometry> elements = group.Elements;

            for (int i = 0; i < elements.Count; i++)
                exploded.AddRange(elements[i].IGetExploded());

            return exploded;
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static List<IBHoMGeometry> IGetExploded(this IBHoMGeometry geometry)
        {
            return GetExploded(geometry as dynamic);
        }

        /***************************************************/

        public static List<ICurve> IGetExploded(this ICurve geometry)
        {
            return GetExploded(geometry as dynamic);
        }

        /***************************************************/

        public static List<ISurface> IGetExploded(this ISurface geometry)
        {
            return GetExploded(geometry as dynamic);
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static List<IBHoMGeometry> GetExploded(this IBHoMGeometry geometry)
        {
            return new List<IBHoMGeometry> { geometry };
        }
    }
}

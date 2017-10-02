using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods - Curves                   ****/
        /***************************************************/

        public static double GetArea(this Arc curve)
        {
            return curve.GetAngle() * Math.Pow(curve.GetRadius(), 2);
        }

        /***************************************************/

        public static double GetArea(this Circle curve)
        {
            return Math.PI * Math.Pow(curve.Radius, 2);
        }

        /***************************************************/

        public static double GetArea(this Line curve)
        {
            return 0;
        }

        /***************************************************/

        public static double GetArea(this NurbCurve curve)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        public static double GetArea(this PolyCurve curve)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        public static double GetArea(this Polyline curve)
        {
            throw new NotImplementedException();
        }

        /***************************************************/
        /**** Public Methods - Surfaces                 ****/
        /***************************************************/

        public static double GetArea(this Mesh mesh)
        {
            Mesh tMesh = mesh.GetTriangulated();
            double area = 0;
            List<Face> faces = tMesh.Faces;
            List<Point> vertices = tMesh.Vertices;
            for (int i = 0; i < faces.Count; i++)
            {
                Point pA = vertices[faces[i].A];
                Point pB = vertices[faces[i].B];
                Point pC = vertices[faces[i].C];
                Vector AB = new Vector(pB.X - pA.X, pB.Y - pA.Y, pB.Z - pA.Z);
                Vector AC = new Vector(pC.X - pA.X, pC.Y - pA.Y, pC.Z - pA.Z);
                area += AB.GetCrossProduct(AC).GetLength();
            }
            return area / 2;
        }

        /***************************************************/

        public static double GetArea(this NurbSurface nurbs)
        {
            throw new NotImplementedException();
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static double IGetArea(this IBHoMGeometry geometry)
        {
            return GetArea(geometry as dynamic);
        }

    }
}

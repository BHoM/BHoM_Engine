using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Geometry
{
    public static partial class Measure
    {
        #region Public Methods
        public static double GetArea(this IBHoMGeometry geo)
        {
            return _GetArea(geo as dynamic);
        }
        #endregion


        #region Private Methods

        /*  2D  **************************************************/

        private static double _GetArea(this Arc curve)
        {
            return curve.GetAngle() * Math.Pow(curve.GetRadius(), 2);
        }

        /***************************************************/

        private static double _GetArea(this Circle curve)
        {
            return Math.PI * Math.Pow(curve.Radius, 2);
        }

        /***************************************************/

        private static double _GetArea(this Line curve)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        private static double _GetArea(this NurbCurve curve)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        private static double _GetArea(this PolyCurve curve)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        private static double _GetArea(this Polyline curve)
        {
            throw new NotImplementedException();
        }

        /*  3D  **************************************************/
        private static double _GetArea(this Mesh mesh)
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

        private static double _GetArea(this NurbSurface nurbs)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}

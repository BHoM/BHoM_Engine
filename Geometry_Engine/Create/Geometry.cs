using BH.oM.Geometry;
using System;

namespace BH.Engine.Geometry
{
    public static partial class Create
    {
        /***************************************************/
        /**** Random Geometry                           ****/
        /***************************************************/

        public static IGeometry RandomGeometry(int seed = -1, BoundingBox box = null)
        {
            if (seed == -1)
                seed = m_Random.Next();
            Random rnd = new Random(seed);
            return RandomGeometry(rnd, box);
        }

        /***************************************************/

        public static IGeometry RandomGeometry(Random rnd, BoundingBox box = null)
        {
            int nb = rnd.Next(15);
            switch(nb)
            {
                case 0:
                    return RandomArc(rnd, box);
                case 1:
                    return RandomCircle(rnd, box);
                case 2:
                    return RandomEllipse(rnd, box);
                case 3:
                    return RandomLine(rnd, box);
                case 4:
                    return RandomNurbsCurve(rnd, box);
                case 5:
                    return RandomPolyline(rnd, box);
                case 6:
                    return RandomExtrusion(rnd, box);
                case 7:
                    return RandomLoft(rnd, box);
                case 8:
                    return RandomNurbsSurface(rnd, box);
                case 9:
                    return RandomPipe(rnd, box);
                case 10:
                    return RandomPolySurface(rnd, box);
                case 11:
                    return RandomMesh(rnd, box);
                case 12:
                    return RandomCompositeGeometry(rnd, box);
                case 13:
                    return RandomPlane(rnd, box);
                default:
                    return RandomPoint(rnd, box);
            }
        }

        /***************************************************/

        public static IGeometry RandomGeometry(Type type, Random rnd, BoundingBox box = null)
        {
            if (!typeof(IGeometry).IsAssignableFrom(type))
                return null;

            switch (type.Name)
            {
                case "Arc":
                    return RandomArc(rnd, box);
                case "BoundingBox":
                    return RandomBoundingBox(rnd, box);
                case "Circle":
                    return RandomCircle(rnd, box);
                case "CompositeGeometry":
                    return RandomCompositeGeometry(rnd, box);
                case "Ellipse":
                    return RandomEllipse(rnd, box);
                case "Extrusion":
                    return RandomExtrusion(rnd, box);
                case "ICurve":
                    return RandomCurve(rnd, box);
                case "IGeometry":
                    return RandomGeometry(rnd, box);
                case "ISurface":
                    return RandomSurface(rnd, box);
                case "Line":
                    return RandomLine(rnd, box);
                case "Loft":
                    return RandomLoft(rnd, box);
                case "Mesh":
                    return RandomMesh(rnd, box);
                case "NurbsCurve":
                    return RandomNurbsCurve(rnd, box);
                case "NurbsSurface":
                    return RandomNurbsSurface(rnd, box);
                case "Pipe":
                    return RandomPipe(rnd, box);
                case "Plane":
                    return RandomPlane(rnd, box);
                case "Point":
                    return RandomPoint(rnd, box);
                case "PolyCurve":
                    return RandomPolyCurve(rnd, box);
                case "Polyline":
                    return RandomPolyline(rnd, box);
                case "Quaternion":
                    return RandomQuaternion(rnd);
                case "PolySurface":
                    return RandomPolySurface(rnd, box);
                case "TransformMatrix":
                    return RandomMatrix(rnd);
                case "Vector":
                    return RandomVector(rnd, box);
                default:
                    return null;
            }
        }
        
        /***************************************************/
    }
}

using BH.oM.Geometry;
using System;

namespace BH.Engine.Geometry
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static TransformMatrix TransformMatrix(Quaternion q)
        {
            return new TransformMatrix
            {
                Matrix = new double[,]
                {
                    {  1 - 2*Math.Pow(q.Y,2) - 2*Math.Pow(q.Z, 2), 2 * q.X*q.Y - 2 * q.W*q.Z, 2 * q.X*q.Z + 2 * q.Y*q.W,   0   },
                    {  2 * q.X*q.Y + 2 * q.W*q.Z,  1 - 2*Math.Pow(q.X,2) - 2*Math.Pow(q.Z, 2), 2 * q.Y*q.Z - 2 * q.W*q.X,  0   },
                    {  2 * q.X*q.Z - 2 * q.W*q.Y,  2 * q.Y*q.Z + 2 * q.W*q.X,  1 - 2*Math.Pow(q.X,2) - 2*Math.Pow(q.Y, 2), 0   },
                    {  0,                          0,                          0,                                          1   }
                }
            };
        }

        /***************************************************/

        public static TransformMatrix TranslationMatrix(Vector vector)
        {
            return new TransformMatrix
            {
                Matrix = new double[,]
                {
                    {  1,  0,  0,  vector.X   },
                    {  0,  1,  0,  vector.Y   },
                    {  0,  0,  1,  vector.Z   },
                    {  0,  0,  0,  1   }
                }
            };
        }

        /***************************************************/

        public static TransformMatrix IdentityMatrix()
        {
            return new TransformMatrix
            {
                Matrix = new double[,]
                {
                    {  1,  0,  0,  0   },
                    {  0,  1,  0,  0   },
                    {  0,  0,  1,  0   },
                    {  0,  0,  0,  1   }
                }
            };
        }

        /***************************************************/

        public static TransformMatrix RotationMatrix(Point centre, Vector axis, double angle)
        {
            TransformMatrix rotation = TransformMatrix(Quaternion(axis, angle));
            TransformMatrix t1 = TranslationMatrix(centre - oM.Geometry.Point.Origin);
            TransformMatrix t2 = TranslationMatrix(oM.Geometry.Point.Origin - centre);

            return t1 * rotation * t2;
        }

        /***************************************************/

        public static TransformMatrix ScaleMatrix(Point refPoint, Vector scaleVector)
        {
            TransformMatrix move1 = TranslationMatrix(oM.Geometry.Point.Origin - refPoint);
            TransformMatrix move2 = TranslationMatrix(refPoint - oM.Geometry.Point.Origin);
            TransformMatrix scale = new TransformMatrix
            {
                Matrix = new double[,]
                {
                    {  scaleVector.X,  0,  0,  0   },
                    {  0, scaleVector.Y,  0,  0   },
                    {  0,  0, scaleVector.Z,  0   },
                    {  0,  0,  0,  1   }
                }
            };

            return move2 * scale * move1;
        }

        /***************************************************/
    }
}

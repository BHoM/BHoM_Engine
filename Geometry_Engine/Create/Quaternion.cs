using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Geometry
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Quaternion CreateQuaternionRotationNormal(Vector axis, double angle)
        {
            double sin = Math.Sin(angle / 2);
            return new Quaternion(axis.X * sin, axis.Y * sin, axis.Z * sin, Math.Cos(angle / 2)).GetNormalised();
        }
    }
}

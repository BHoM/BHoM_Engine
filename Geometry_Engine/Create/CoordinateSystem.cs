using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Geometry;
using System.ComponentModel;

namespace BH.Engine.Geometry
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates a Cartesian CoordinateSystem. x and y will be unitised. If x and why are non-orthogonal, y will be made orthogonal to x, while x will be kept")]
        public static CoordinateSystem CoordinateSystem(Point origin, Vector x, Vector y)
        {
            x = x.Normalise();
            y = y.Normalise();

            double dot = x.DotProduct(y);

            if (Math.Abs(1 - dot) < Tolerance.Angle)    //Not exactly angle tolerance, but close
                throw new ArgumentException("Can not create coordinate system from parallell vectors");

            Vector z = x.CrossProduct(y);
            z.Normalise();

            if (Math.Abs(dot) > Tolerance.Angle)    //Not exactly angle tolerance, but close
            {
                Reflection.Compute.RecordWarning("x and y are not orthogonal. y will be made othogonal to x and z");
                y = z.CrossProduct(x).Normalise();
            }

            return new CoordinateSystem { X = x, Y = y, Z = z, Origin = origin };
        }

        /***************************************************/

        public static CoordinateSystem RandomCoordinateSystem(int seed = -1, BoundingBox box = null)
        {
            if (seed == -1)
                seed = m_Random.Next();
            Random rnd = new Random(seed);
            return RandomCoordinateSystem(rnd, box);
        }

        /***************************************************/

        public static CoordinateSystem RandomCoordinateSystem(Random rnd, BoundingBox box = null)
        {
            Vector x = RandomVector(rnd, box);
            Vector y = RandomVector(rnd, box);

            if (x.IsParallel(y) != 0)
                return RandomCoordinateSystem(rnd, box);

            Point orgin = RandomPoint(rnd, box);

            return CoordinateSystem(orgin, x, y);
        }

        /***************************************************/
    }
}

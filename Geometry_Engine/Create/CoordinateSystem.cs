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
        public static CoordinateSystem CoordinateSystem(Point orgin, Vector x, Vector y)
        {
            x = x.Normalise();
            y = y.Normalise();

            double dot = x.DotProduct(y);

            double dotTolerance = Math.Cos(Tolerance.Angle);

            if (Math.Abs(1 - dot) < dotTolerance)
                throw new ArgumentException("Can not create coordinate system from parallell vectors");

            Vector z = x.CrossProduct(y);
            z.Normalise();

            if (Math.Abs(dot) > dotTolerance)
            {
                Reflection.Compute.RecordWarning("x and y are not orthogonal. y will be made othogonal to x and z");
                y = z.CrossProduct(x).Normalise();
            }

            return new CoordinateSystem(x, y, z, orgin);
        }

        /***************************************************/
    }
}

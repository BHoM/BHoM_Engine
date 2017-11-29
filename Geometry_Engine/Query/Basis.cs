using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Geometry;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {

        /***************************************************/
        //TODO: Testing needed!!
        private static double GetBasisFunction(this NurbCurve curve, int i, int n, double t)
        {
            if (n > 0 && t >= curve.Knots[i] && t < curve.Knots[i + n + 1])
            {
                double result = 0;
                if (curve.Knots[i + n] - curve.Knots[i] > 0)
                {
                    result += GetBasisFunction(curve, i, n - 1, t) * (t - curve.Knots[i]) / (curve.Knots[i + n] - curve.Knots[i]);
                }
                if (curve.Knots[i + n + 1] - curve.Knots[i + 1] > 0)
                {
                    result += GetBasisFunction(curve, i + 1, n - 1, t) * (curve.Knots[i + n + 1] - t) / (curve.Knots[i + n + 1] - curve.Knots[i + 1]);
                }

                return result;
            }
            else
            {
                return t >= curve.Knots[i] && t < curve.Knots[i + 1] ? 1 : 0;
            }
        }

        /***************************************************/
        //TODO: Testing needed!!
        private static double DerivativeFunction(this NurbCurve curve, int i, int n, double t)
        {
            if (n > 0)
            {
                double result = 0;
                if (i + n < curve.Knots.Count && curve.Knots[i + n] - curve.Knots[i] > 0)
                {
                    result += GetBasisFunction(curve, i, n - 1, t) * n / (curve.Knots[i + n] - curve.Knots[i]);
                }
                if (i + n + 1 < curve.Knots.Count && curve.Knots[i + n + 1] - curve.Knots[i + 1] > 0)
                {
                    result -= GetBasisFunction(curve, i + 1, n - 1, t) * n / (curve.Knots[i + n + 1] - curve.Knots[i + 1]);
                }
                return result;
            }

            return 0;
        }
    }
}

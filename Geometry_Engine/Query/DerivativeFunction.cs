using BH.oM.Geometry;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        //TODO: Testing needed!!
        public static double DerivativeFunction(this NurbCurve curve, int i, int n, double t)
        {
            if (n > 0)
            {
                double result = 0;
                if (i + n < curve.Knots.Count && curve.Knots[i + n] - curve.Knots[i] > 0)
                    result += BasisFunction(curve, i, n - 1, t) * n / (curve.Knots[i + n] - curve.Knots[i]);

                if (i + n + 1 < curve.Knots.Count && curve.Knots[i + n + 1] - curve.Knots[i + 1] > 0)
                    result -= BasisFunction(curve, i + 1, n - 1, t) * n / (curve.Knots[i + n + 1] - curve.Knots[i + 1]);

                return result;
            }

            return 0;
        }

        /***************************************************/
    }
}

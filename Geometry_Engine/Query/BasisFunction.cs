using BH.oM.Geometry;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/
        
        public static double BasisFunction(this NurbsCurve curve, int i, int n, double t)  //TODO: Testing needed!!
        {
            if (n > 0 && t >= curve.Knots[i] && t < curve.Knots[i + n + 1])
            {
                double result = 0;
                if (curve.Knots[i + n] - curve.Knots[i] > 0)
                    result += BasisFunction(curve, i, n - 1, t) * (t - curve.Knots[i]) / (curve.Knots[i + n] - curve.Knots[i]);
                if (curve.Knots[i + n + 1] - curve.Knots[i + 1] > 0)
                    result += BasisFunction(curve, i + 1, n - 1, t) * (curve.Knots[i + n + 1] - t) / (curve.Knots[i + n + 1] - curve.Knots[i + 1]);

                return result;
            }
            else
                return t >= curve.Knots[i] && t < curve.Knots[i + 1] ? 1 : 0;
        }

        /***************************************************/
    }
}

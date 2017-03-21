using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine_Explore.BHoM.Geometry
{

    // Test NURBS curve using array of double instead of list of Point
    public class NurbCurveB : ICurve
    {
        /***************************************************/
        /**** Properties                                ****/
        /***************************************************/

        public double[] ControlPoints { get; set; } = new double[0];

        public List<double> Weights { get; set; } = new List<double>();

        public List<double> Knots { get; set; } = new List<double>();


        /***************************************************/
        /**** Constructors                              ****/
        /***************************************************/

        public NurbCurveB() { }

        /***************************************************/

        public NurbCurveB(IEnumerable<Point> controlPoints, int degree = 3)
        {
            int n = controlPoints.Count();
            int d = degree - 1;
            List<Point> points = controlPoints.ToList();

            ControlPoints = new double[n*3];
            for (int i = 0; i < n; i++)
            {
                ControlPoints[3 * i] = points[i].X;
                ControlPoints[3 * i + 1] = points[i].Y;
                ControlPoints[3 * i + 2] = points[i].Z;
            }

            Weights = Enumerable.Repeat(1.0, n).ToList();
            Knots = Enumerable.Repeat(0, d).Concat(Enumerable.Range(0, n - d).Concat(Enumerable.Repeat(n - d - 1, d))).Select(x => (double)x).ToList();
        }

        /***************************************************/

        public NurbCurveB(IEnumerable<Point> controlPoints, IEnumerable<double> weights, int degree = 3)
        {
            int n = controlPoints.Count();
            int d = degree - 1;
            List<Point> points = controlPoints.ToList();

            ControlPoints = new double[n * 3];
            for (int i = 0; i < n; i++)
            {
                ControlPoints[3 * i] = points[i].X;
                ControlPoints[3 * i + 1] = points[i].Y;
                ControlPoints[3 * i + 2] = points[i].Z;
            }

            Weights = weights.ToList();
            Knots = Enumerable.Repeat(0, d).Concat(Enumerable.Range(0, n - d).Concat(Enumerable.Repeat(n - d - 1, d))).Select(x => (double)x).ToList();
        }

        /***************************************************/

        public NurbCurveB(IEnumerable<Point> controlPoints, IEnumerable<double> weights, IEnumerable<double> knots)
        {
            int n = controlPoints.Count();
            List<Point> points = controlPoints.ToList();

            ControlPoints = new double[n * 3];
            for (int i = 0; i < n; i++)
            {
                ControlPoints[3 * i] = points[i].X;
                ControlPoints[3 * i + 1] = points[i].Y;
                ControlPoints[3 * i + 2] = points[i].Z;
            }

            Weights = weights.ToList();
            Knots = knots.ToList();
        }


        /***************************************************/
        /**** Local Methods                             ****/
        /***************************************************/

        public int GetDegree()
        {
            return 1 + Knots.Count - ControlPoints.Length/3;
        }
    }
}

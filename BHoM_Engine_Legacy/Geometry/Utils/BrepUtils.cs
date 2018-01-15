using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.oM.Geometry
{
    public static class BrepUtils
    {
        public static bool TryJoin(Brep b1, Brep b2, out Brep result)
        {
            List<Curve> c1 = CurveUtils.Join(b1.GetExternalEdges());
            List<Curve> c2 = CurveUtils.Join(b2.GetExternalEdges());

            List<Curve> nakedEdges = new List<Curve>();
            List<Curve> overlappedEdges = new List<Curve>();
            List<Point> pts = new List<Point>();

            List<Curve> updatedN = new List<Curve>();
            List<Curve> updatedI = new List<Curve>();

            for (int i = 0; i < c1.Count; i++)
            {
                for (int j = 0; j < c2.Count; j++)
                {
                    if (Intersect.CurveCurve(c1[i], c2[j], 0.001, out pts, out overlappedEdges, out nakedEdges))
                    {
                        updatedN.AddRange(nakedEdges);
                        updatedN.AddRange(overlappedEdges);
                    }
                }
            }
            if (updatedN.Count > 0)
            {
                Group<Brep> polySurface = new Group<Brep>();

                if (b1 is PolySurface)
                {
                    polySurface.AddRange((b1 as PolySurface).Surfaces);
                }
                else
                {
                    polySurface.Add(b1);
                }

                if (b2 is PolySurface)
                {
                    polySurface.AddRange((b2 as PolySurface).Surfaces);
                }
                else
                {
                    polySurface.Add(b2);
                }
                result = new PolySurface(polySurface);
                result.GetExternalEdges().AddRange(updatedN);
                result.GetInternalEdges().AddRange(updatedI);
                return true;
            }
            else
            {
                result = null;
                return false;
            }
        }

        public static List<Brep> Join(List<Brep> breps)
        {
            Brep joined = null;
            List<Brep> result = new List<Brep>();
            for (int i = 0; i < breps.Count; i++)
            {
                result.Add(breps[i]);
            }
            //Join by overlapping naked edges

            int counter = 0;

            while (counter < result.Count)
            {
                List<Curve> b1Curves = CurveUtils.Join(result[counter].GetExternalEdges());
                for (int j = counter + 1; j < result.Count; j++)
                {

                    if (TryJoin(result[counter], result[j], out joined))
                    {
                        result[j] = joined;
                        result.RemoveAt(counter--);
                        break;
                    }
                }
            }

            return result;
        }


    }

}

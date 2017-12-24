using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Fix
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static PolyCurve GetWithSortedCurves(this PolyCurve curve)
        {
            if (curve.Curves.Count < 2)
                return curve.GetClone() as PolyCurve;

            List<ICurve> pending = curve.Curves.Select(x => x.IGetClone()).ToList();
            PolyCurve result = new PolyCurve(new List<ICurve> { pending[0] });
            pending.RemoveAt(0);

            while (pending.Count > 0)
            {
                Point start1 = result.GetStartPoint();
                Point end1 = result.GetEndPoint();
                bool foundNext = false;

                for (int i = 0; i < pending.Count; i++)
                {
                    Point start2 = pending[i].IGetStartPoint();
                    Point end2 = pending[i].IGetEndPoint();
 
                    if (end1.GetSquareDistance(start2) < Tolerance.SqrtDist)
                    {
                        result.Curves.Add(pending[i]);
                        pending.RemoveAt(i);
                        foundNext = true;
                        break;
                    }
                    else if (end1.GetSquareDistance(end2) < Tolerance.SqrtDist)
                    {
                        result.Curves.Add(pending[i].IGetFlipped());
                        pending.RemoveAt(i);
                        foundNext = true;
                        break;
                    }
                    else if (start1.GetSquareDistance(end2) < Tolerance.SqrtDist)
                    {
                        result.Curves.Insert(0, pending[i]);
                        pending.RemoveAt(i);
                        foundNext = true;
                        break;
                    }
                    else if (start1.GetSquareDistance(start2) < Tolerance.SqrtDist)
                    {
                        result.Curves.Insert(0, pending[i].IGetFlipped());
                        pending.RemoveAt(i);
                        foundNext = true;
                        break;
                    }
                }

                if (!foundNext)
                    throw new Exception("PolyCurve with unconnected subcurves cannot have them sorted");
            }
            return result;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Structure.Results;

namespace BH.Engine.Results
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<BarForce> AbsoluteMaxForces(IEnumerable<BarForce> forces)
        {

            List<BarForce> maxForces = new List<BarForce>();
            maxForces.Add(forces.MaxBy(x => Math.Abs(x.FX)));
            maxForces.Add(forces.MaxBy(x => Math.Abs(x.FY)));
            maxForces.Add(forces.MaxBy(x => Math.Abs(x.FZ)));
            maxForces.Add(forces.MaxBy(x => Math.Abs(x.MX)));
            maxForces.Add(forces.MaxBy(x => Math.Abs(x.MY)));
            maxForces.Add(forces.MaxBy(x => Math.Abs(x.MZ)));

            return maxForces;
        }

        /***************************************************/

        public static List<BarForce> AbsoluteMaxEnvelopeByCase(IEnumerable<BarForce> forces)
        {
            return forces.GroupByCase().Select(x => x.AbsoluteMaxEnvelope(false, true)).ToList();
        }

        /***************************************************/

        public static List<BarForce> AbsoluteMaxEnvelopeByObject(IEnumerable<BarForce> forces)
        {
            return forces.GroupByObjectId().Select(x => x.AbsoluteMaxEnvelope(true, false)).ToList();
        }

        /***************************************************/

        public static BarForce AbsoluteMaxEnvelope(this IEnumerable<BarForce> forces, bool idFromFirst = false, bool caseFromFirst = false)
        {
            return new BarForce()
            {
                ObjectId = idFromFirst ? forces.First().ObjectId : "",
                ResultCase = caseFromFirst ? forces.First().ResultCase : "",
                FX = forces.AbsoluteMax(x => x.FX),
                FY = forces.AbsoluteMax(x => x.FY),
                FZ = forces.AbsoluteMax(x => x.FZ),
                MX = forces.AbsoluteMax(x => x.MX),
                MY = forces.AbsoluteMax(x => x.MY),
                MZ = forces.AbsoluteMax(x => x.MZ),
            };
        }

        /***************************************************/

        public static double AbsoluteMax<T>(this IEnumerable<T> source, Func<T, double> selector)
        {
            if (source == null) throw new ArgumentNullException("source");
            bool first = true;
            double absMax = 0;
            double signedAbsMax = 0;
            foreach (var item in source)
            {
                if (first)
                {
                    signedAbsMax = selector(item);
                    absMax = Math.Abs(signedAbsMax);

                    first = false;
                }
                else
                {
                    double currentVal = selector(item);
                    double currentAbs = Math.Abs(currentVal);
                    if (currentAbs > absMax)
                    {
                        absMax = currentAbs;
                        signedAbsMax = currentVal;
                    }
                }
            }
            if (first) throw new InvalidOperationException("Sequence is empty.");
            return signedAbsMax;
        }

        /***************************************************/
    }
}

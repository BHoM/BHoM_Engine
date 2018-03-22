using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Structural.Results;

namespace BH.Engine.Results
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<BarForce> MaxForces(IEnumerable<BarForce> forces)
        {

            List<BarForce> maxForces = new List<BarForce>();
            maxForces.Add(forces.MaxBy(x => x.FX));
            maxForces.Add(forces.MaxBy(x => x.FY));
            maxForces.Add(forces.MaxBy(x => x.FZ));
            maxForces.Add(forces.MaxBy(x => x.MX));
            maxForces.Add(forces.MaxBy(x => x.MY));
            maxForces.Add(forces.MaxBy(x => x.MZ));

            return maxForces;
        }

        /***************************************************/

        public static List<BarForce> MaxEnvelopByCase(IEnumerable<BarForce> forces)
        {
            return forces.GroupByCase().Select(x => x.MaxEnvelop(false, true)).ToList();
        }

        /***************************************************/

        public static List<BarForce> MaxEnvelopByObject(IEnumerable<BarForce> forces)
        {
            return forces.GroupByObjectId().Select(x => x.MaxEnvelop(true, false)).ToList();
        }

        /***************************************************/

        public static BarForce MaxEnvelop(this IEnumerable<BarForce> forces, bool idFromFirst = false, bool caseFromFirst = false)
        {
            return new BarForce()
            {
                ObjectId = idFromFirst ? forces.First().ObjectId : "",
                Case = caseFromFirst ? forces.First().Case : "",
                FX = forces.Max(x => x.FX),
                FY = forces.Max(x => x.FY),
                FZ = forces.Max(x => x.FZ),
                MX = forces.Max(x => x.MX),
                MY = forces.Max(x => x.MY),
                MZ = forces.Max(x => x.MZ),
            };
        }

        /***************************************************/

        //TODO: Move these generic methods somewhere else
        public static T MaxBy<T, U>(this IEnumerable<T> source, Func<T, U> selector) where U : IComparable<U>
        {
            if (source == null) throw new ArgumentNullException("source");
            bool first = true;
            T maxObj = default(T);
            U maxKey = default(U);
            foreach (var item in source)
            {
                if (first)
                {
                    maxObj = item;
                    maxKey = selector(maxObj);
                    first = false;
                }
                else
                {
                    U currentKey = selector(item);
                    if (currentKey.CompareTo(maxKey) > 0)
                    {
                        maxKey = currentKey;
                        maxObj = item;
                    }
                }
            }
            if (first) throw new InvalidOperationException("Sequence is empty.");
            return maxObj;
        }

        /***************************************************/
    }
}

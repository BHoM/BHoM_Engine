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

        public static List<BarForce> MinForces(IEnumerable<BarForce> forces)
        {

            List<BarForce> minForces = new List<BarForce>();
            minForces.Add(forces.MinBy(x => x.FX));
            minForces.Add(forces.MinBy(x => x.FY));
            minForces.Add(forces.MinBy(x => x.FZ));
            minForces.Add(forces.MinBy(x => x.MX));
            minForces.Add(forces.MinBy(x => x.MY));
            minForces.Add(forces.MinBy(x => x.MZ));

            return minForces;
        }

        /***************************************************/

        public static List<BarForce> MinEnvelopByCase(IEnumerable<BarForce> forces)
        {
            return forces.GroupByCase().Select(x => x.MinEnvelop(false, true)).ToList();
        }

        /***************************************************/

        public static List<BarForce> MinEnvelopByObject(IEnumerable<BarForce> forces)
        {
            return forces.GroupByObjectId().Select(x => x.MinEnvelop(true, false)).ToList();
        }

        /***************************************************/

        public static BarForce MinEnvelop(this IEnumerable<BarForce> forces, bool idFromFirst = false, bool caseFromFirst = false)
        {
            return new BarForce()
            {
                ObjectId = idFromFirst ? forces.First().ObjectId : "",
                Case = caseFromFirst ? forces.First().Case : "",
                FX = forces.Min(x => x.FX),
                FY = forces.Min(x => x.FY),
                FZ = forces.Min(x => x.FZ),
                MX = forces.Min(x => x.MX),
                MY = forces.Min(x => x.MY),
                MZ = forces.Min(x => x.MZ),
            };
        }

        /***************************************************/

        //TODO: Move these generic methods somewhere else
        public static T MinBy<T, U>(this IEnumerable<T> source, Func<T, U> selector) where U : IComparable<U>
        {
            if (source == null) throw new ArgumentNullException("source");
            bool first = true;
            T minObj = default(T);
            U minKey = default(U);
            foreach (var item in source)
            {
                if (first)
                {
                    minObj = item;
                    minKey = selector(minObj);
                    first = false;
                }
                else
                {
                    U currentKey = selector(item);
                    if (currentKey.CompareTo(minKey) < 0)
                    {
                        minKey = currentKey;
                        minObj = item;
                    }
                }
            }
            if (first) throw new InvalidOperationException("Sequence is empty.");
            return minObj;
        }

        /***************************************************/
    }
}

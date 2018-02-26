using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Engine_Explore.Engine.Sets
{
    public static class Stitch
    {
        public static T A2B<T>(T a, T b)
        {
            foreach (PropertyInfo info in typeof(T).GetProperties())
            {
                dynamic ap = info.GetValue(a);
                dynamic bp = info.GetValue(b);

                if (ap is IEnumerable)
                    info.SetValue(a, As2Bs(ap, bp));
                else if (Engine.Sets.Compare.Value(ap, bp))
                    info.SetValue(a, bp);
            }

            return a;
        }

        /***************************************************/

        public static IEnumerable<T> As2Bs<T>(IEnumerable<T> a, IEnumerable<T> b)
        {
            foreach (dynamic ae in a)
            {
                foreach (dynamic be in b)
                {
                    if (Engine.Sets.Compare.Value(ae, be))
                    {
                        foreach (PropertyInfo info in typeof(T).GetProperties())
                            info.SetValue(ae, info.GetValue(be));
                        break;
                    }
                }
            }

            return a;
        }

    }
}

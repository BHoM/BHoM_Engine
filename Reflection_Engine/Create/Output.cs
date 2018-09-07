using System;
using BH.oM.Reflection;

namespace BH.Engine.Reflection
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Output<T> Output<T>(T item1)
        {
            return new Output<T> { Item1 = item1 };
        }

        /***************************************************/

        public static Output<T1, T2> Output<T1, T2>(T1 item1, T2 item2)
        {
            return new Output<T1, T2> { Item1 = item1, Item2 = item2 };
        }

        /***************************************************/

        public static Output<T1, T2, T3> Output<T1, T2, T3>(T1 item1, T2 item2, T3 item3)
        {
            return new Output<T1, T2, T3> { Item1 = item1, Item2 = item2, Item3 = item3 };
        }

        /***************************************************/

        public static Output<T1, T2, T3, T4> Output<T1, T2, T3, T4>(T1 item1, T2 item2, T3 item3, T4 item4)
        {
            return new Output<T1, T2, T3, T4> { Item1 = item1, Item2 = item2, Item3 = item3, Item4 = item4 };
        }

        /***************************************************/

        public static Output<T1, T2, T3, T4, T5> Output<T1, T2, T3, T4, T5>(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5)
        {
            return new Output<T1, T2, T3, T4, T5> { Item1 = item1, Item2 = item2, Item3 = item3, Item4 = item4, Item5 = item5 };
        }


        /***************************************************/
    }
}

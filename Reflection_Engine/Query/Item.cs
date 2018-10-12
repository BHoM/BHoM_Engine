using BH.oM.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Reflection
{
    public static partial class Query
    {
        /***************************************************/
        /**** Interface Methods                         ****/
        /***************************************************/

        public static object IItem(this object obj, int index)
        {
            return Item(obj as dynamic, index);
        }

        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static object Item<T>(this List<T> list, int index)
        {
            return list[index];
        }

        /***************************************************/

        public static object Item<T>(this Output<T> output, int index)
        {
            if (index == 0)
                return output.Item1;
            else
                return null;
        }

        /*************************************/

        public static object Item<T1, T2>(this Output<T1, T2> output, int index)
        {
            switch (index)
            {
                case 0:
                    return output.Item1;
                case 1:
                    return output.Item2;
                default:
                    return null;
            }
        }

        /*************************************/

        public static object Item<T1, T2, T3>(this Output<T1, T2, T3> output, int index)
        {
            switch (index)
            {
                case 0:
                    return output.Item1;
                case 1:
                    return output.Item2;
                case 2:
                    return output.Item3;
                default:
                    return null;
            }
        }

        /*************************************/

        public static object Item<T1, T2, T3, T4>(this Output<T1, T2, T3, T4> output, int index)
        {
            switch (index)
            {
                case 0:
                    return output.Item1;
                case 1:
                    return output.Item2;
                case 2:
                    return output.Item3;
                case 3:
                    return output.Item4;
                default:
                    return null;
            }
        }

        /*************************************/

        public static object Item<T1, T2, T3, T4, T5>(this Output<T1, T2, T3, T4, T5> output, int index)
        {
            switch (index)
            {
                case 0:
                    return output.Item1;
                case 1:
                    return output.Item2;
                case 2:
                    return output.Item3;
                case 3:
                    return output.Item4;
                case 4:
                    return output.Item5;
                default:
                    return null;
            }
        }

        /***************************************************/
    }
}

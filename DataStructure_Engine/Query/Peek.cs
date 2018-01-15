using BH.oM.DataStructure;
using System;

namespace BH.Engine.DataStructure
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static T Peek<T>(this PriorityQueue<T> queue) where T : IComparable<T>
        {
            T frontItem = queue.Data[0];
            return frontItem;
        }

        /***************************************************/
    }
}

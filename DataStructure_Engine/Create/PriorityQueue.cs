using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.DataStructure;

namespace BH.Engine.DataStructure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static PriorityQueue<T> PriorityQueue<T>(List<T> data = null) where T : IComparable<T>
        {
            PriorityQueue<T> queue = new PriorityQueue<T>();
            if (data != null)
            {
                queue.Data = data.ToList();
                queue.Data.Sort();
            }

            return queue;
        }

        /***************************************************/
    }
}

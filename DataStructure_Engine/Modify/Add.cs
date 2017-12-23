using BH.oM.DataStructure;
using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.DataStructure
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static void Add<T>(this PriorityQueue<T> queue, T item) where T : IComparable<T>
        {
            List<T> data = queue.Data;
            data.Add(item);
            int ci = data.Count - 1; // child index; start at end
            while (ci > 0)
            {
                int pi = (ci - 1) / 2; // parent index
                if (data[ci].CompareTo(data[pi]) >= 0) break; // child item is larger than (or equal) parent so we're done
                T tmp = data[ci]; data[ci] = data[pi]; data[pi] = tmp;
                ci = pi;
            }
        }

        /***************************************/

        public static void Add<T>(this PointMatrix<T> matrix, Point position, T data = default(T))
        {
            Dictionary<DiscreetPoint, List<LocalData<T>>> cells = matrix.Data;

            DiscreetPoint key = Create.DiscreetPoint(position, matrix.CellSize);
            if (!cells.ContainsKey(key))
                cells[key] = new List<LocalData<T>>();
            cells[key].Add(new LocalData<T> { Position = position, Data = data });
        }

        /***************************************/
    }
}

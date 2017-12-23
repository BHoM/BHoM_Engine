using BH.oM.DataStructure;
using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.DataStructure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static PointMatrix<T> PointMatrix<T>(double cellSize)
        {
            return new PointMatrix<T> { CellSize = cellSize };
        }

        /***************************************************/

        public static PointMatrix<T> PointMatrix<T>(List<LocalData<T>> data, double cellSize = 1.0)
        {
            PointMatrix<T> matrix = new PointMatrix<T> { CellSize = cellSize };
            Dictionary<DiscreetPoint, List<LocalData<T>>> cells = matrix.Data;

            foreach (LocalData<T> d in data)
            {
                DiscreetPoint key = Create.DiscreetPoint(d.Position, cellSize);
                if (!cells.ContainsKey(key))
                    cells[key] = new List<LocalData<T>>();
                cells[key].Add(d);
            }
            

            return matrix;
        }

        /***************************************************/
    }
}

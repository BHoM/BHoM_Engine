using BH.oM.DataStructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.DataStructure
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<LocalData<T>> SubMatrixData<T>(this PointMatrix<T> matrix, DiscreetPoint minCell, DiscreetPoint maxCell)
        {
            Dictionary<DiscreetPoint, List<LocalData<T>>> data = matrix.Data;

            List<LocalData<T>> inCells = new List<LocalData<T>>();
            for (int x = minCell.X; x <= maxCell.X; x++)
            {
                for (int y = minCell.Y; y <= maxCell.Y; y++)
                {
                    for (int z = minCell.Z; z <= maxCell.Z; z++)
                    {
                        DiscreetPoint key = new DiscreetPoint { X = x, Y = y, Z = z };
                        if (data.ContainsKey(key))
                        {
                            foreach (LocalData<T> comp in data[key])
                                inCells.Add(comp);
                        }
                    }
                }
            }

            return inCells;
        }

        /***************************************************/
    }
}

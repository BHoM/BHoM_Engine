using BH.Engine.Geometry;
using BH.oM.DataStructure;
using BH.oM.Geometry;
using System.Collections.Generic;

namespace BH.Engine.DataStructure
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<LocalData<T>> CloseToPoint<T>(this PointMatrix<T> matrix, Point refPt, double maxDist)
        {
            // Collect all the points within cells in range
            Vector range = new Vector { X = maxDist, Y = maxDist, Z = maxDist };
            List<LocalData<T>> inCells = matrix.SubMatrixData<T>(Create.DiscreetPoint(refPt - range, matrix.CellSize), Create.DiscreetPoint(refPt + range, matrix.CellSize));

            // Keep only points within maxDist distance of refPt
            List<LocalData<T>> result = new List<LocalData<T>>();
            foreach (LocalData<T> tuple in inCells)
            {
                if (tuple.Position.Distance(refPt) < maxDist)
                    result.Add(tuple);
            }

            // Return final result
            return result;
        }

        /***************************************************/
    }
}

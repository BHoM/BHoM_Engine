using BH.Engine.Geometry;
using BH.oM.DataStructure;
using System;
using System.Collections.Generic;

namespace BH.Engine.DataStructure
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<Tuple<LocalData<T>, LocalData<T>>> RelatedPairs<T>(this PointMatrix<T> matrix, double minDist, double maxDist)
        {
            int range = (int)Math.Ceiling(maxDist / matrix.CellSize);

            List<Tuple<LocalData<T>, LocalData<T>>> result = new List<Tuple<LocalData<T>, LocalData<T>>>();
            foreach (KeyValuePair<DiscreetPoint, List<LocalData<T>>> kvp in matrix.Data)
            {
                DiscreetPoint k = kvp.Key;
                List<LocalData<T>> closePoints = matrix.SubMatrixData<T>(
                    new DiscreetPoint { X = k.X - range, Y = k.Y - range, Z = k.Z - range },
                    new DiscreetPoint { X = k.X + range, Y = k.Y + range, Z = k.Z + range });

                foreach (LocalData<T> value in kvp.Value)
                {
                    foreach (LocalData<T> other in closePoints)
                    {
                        double dist = value.Position.Distance(other.Position);
                        if (dist < maxDist && dist > minDist)
                            result.Add(new Tuple<LocalData<T>, LocalData<T>>(value, other));
                    }
                }
            }

            return result;
        }

        /***************************************************/
    }
}

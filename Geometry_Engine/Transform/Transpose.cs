using BH.oM.Geometry;

namespace BH.Engine.Geometry
{
    public static partial class Transform
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static TransformMatrix GetTransposed(this TransformMatrix transform)
        {
            double[,] matrix = transform.Matrix;
            double[,] transpose = new double[4,4];

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                    transpose[i,j] = matrix[j,i];
            }
            return new TransformMatrix(transpose);
        }
    }
}

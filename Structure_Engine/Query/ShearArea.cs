using BH.oM.Geometry;
using System;
using System.Collections.Generic;

namespace BH.Engine.Structure
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static double ShearArea(List<IntegrationSlice> slices, double momentOfInertia, double centroid)
        {
            double sy = 0;
            double b = 0;
            double sum = 0;

            foreach (IntegrationSlice slice in slices)
            {
                sy += slice.Length * slice.Width * (centroid - slice.Centre);
                b = slice.Length;
                if (b > 0)
                {
                    sum += Math.Pow(sy, 2) / b * slice.Width;
                }

            }
            return Math.Pow(momentOfInertia, 2) / sum;
        }

        /***************************************************/
    }
}

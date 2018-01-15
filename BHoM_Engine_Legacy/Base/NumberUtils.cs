using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.oM.Base
{
    public static class NumberUtils
    {

        public static bool InRange(double value, double upper, double lower, double tolerance)
        {
            return value < upper + tolerance && value > lower - tolerance;
        }

        public static bool InBetween(double value, double b1, double b2, double tolerance)
        {
            if (b1 > b2)
            {
                return InRange(value, b1, b2, tolerance);
            }
            else
            {
                return InRange(value, b2, b1, tolerance);
            }
        }

        public static bool NearEqual(double value1, double value2, double eps)
        {
            return value1 < value2 + eps && value1 > value2 - eps;
        }
    }
}

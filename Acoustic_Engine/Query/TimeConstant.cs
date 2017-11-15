using BH.oM.Acoustic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Acoustic
{
    public static partial class Query
    {
        public static double TimeConstant(double revTime)
        {
            return revTime / Constants.SabineConstant;
        }

        public static double TimeConstant(this RT60 revTime)
        {
            return revTime.Value / Constants.SabineConstant;
        }
    }
}

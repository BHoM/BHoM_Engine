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
        public static  double GetTimeConstant(this Room room, double revTime)
        {
            // TODO : Acoustics - Investigate Sabine constant
            const double sabineConstant = 13.8155;
            return revTime / sabineConstant;
        }
    }
}

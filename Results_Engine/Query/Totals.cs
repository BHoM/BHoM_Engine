using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Structure.Results;

namespace BH.Engine.Results
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static double FTotal(this GlobalReactions reaction)
        {
            return Math.Sqrt(reaction.FX * reaction.FX + reaction.FY * reaction.FY + reaction.FZ * reaction.FZ);
        }

        /***************************************************/

        public static double MTotal(this GlobalReactions reaction)
        {
            return Math.Sqrt(reaction.MX * reaction.MX + reaction.MY * reaction.MY + reaction.MZ * reaction.MZ);
        }

        /***************************************************/

        public static double TotalDisplacement(NodeDisplacement disp)
        {
            return Math.Sqrt(disp.UX * disp.UX + disp.UY * disp.UY + disp.UZ * disp.UZ);
        }

        /***************************************************/
    }
}

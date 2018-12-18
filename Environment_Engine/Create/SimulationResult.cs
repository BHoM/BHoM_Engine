using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.oM.Environment.Elements;
using BH.oM.Environment.Results;

namespace BH.Engine.Environment
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static SimulationResult SimulationResult(SimulationResultType type, List<ProfileResult> results)
        {
            return new oM.Environment.Results.SimulationResult
            {
                SimulationResultType = type,
                SimulationResults = results,
            };
        }

        public static SimulationResult SimulationResult(SimulationResultType type, ProfileResult result)
        {
            return SimulationResult(type, new List<ProfileResult> { result });
        }

        /***************************************************/
    }
}

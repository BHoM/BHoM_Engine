using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.oM.Environment.Results;
using BH.oM.Geometry;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<SimulationResult> ResultsByType(this List<SimulationResult> results, SimulationResultType type)
        {
            return results.Where(x => x.SimulationResultType == type).ToList();
        }

        public static List<SimulationResult> ResultsByUnit(this List<SimulationResult> results, ProfileResultUnits unit)
        {
            List<SimulationResult> resultList = new List<SimulationResult>();

            foreach(SimulationResult sr in results)
            {
                List<ProfileResult> pResults = sr.SimulationResults.Where(x => x.ResultUnit == unit).ToList();
                if (pResults.Count > 0)
                    resultList.Add(Create.SimulationResult(sr.SimulationResultType, pResults));
            }

            return resultList;
        }

        public static List<SimulationResult> ResultsByResultType(this List<SimulationResult> results, ProfileResultType resultType)
        {
            List<SimulationResult> resultList = new List<SimulationResult>();

            foreach(SimulationResult sr in results)
            {
                List<ProfileResult> pResults = sr.SimulationResults.Where(x => x.ResultType == resultType).ToList();
                if (pResults.Count > 0)
                    resultList.Add(Create.SimulationResult(sr.SimulationResultType, pResults));
            }

            return resultList;
        }

        public static List<SimulationResult> ResultsByTypeByUnit(this List<SimulationResult> results, SimulationResultType simulationType, ProfileResultUnits unitType)
        {
            results = results.ResultsByType(simulationType);
            return results.ResultsByUnit(unitType);
        }

        public static List<SimulationResult> ResultsByTypeByUnitByResultType(this List<SimulationResult> results, SimulationResultType simulationType, ProfileResultUnits unitType, ProfileResultType resultType)
        {
            results = results.ResultsByTypeByUnit(simulationType, unitType);
            return results.ResultsByResultType(resultType);
        }

        /***************************************************/
    }
}

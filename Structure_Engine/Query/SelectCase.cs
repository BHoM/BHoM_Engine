using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Common;
using BH.oM.Structure.Loads;

namespace BH.Engine.Structure
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<T> SelectCase<T>(this List<T> results, object loadCase) where T : IResult
        {
            if (loadCase != null)
            {
                string loadCaseId = null;

                if (loadCase is string)
                    loadCaseId = loadCase as string;
                else if (loadCase is ICase)
                    loadCaseId = (loadCase as ICase).Number.ToString();
                else if (loadCase is int || loadCase is double)
                    loadCaseId = loadCase.ToString();

                if (!string.IsNullOrWhiteSpace(loadCaseId))
                    return results.Where(x => x.ResultCase == loadCaseId).ToList();
            }

            return results;
        }

        /***************************************************/
    }
}

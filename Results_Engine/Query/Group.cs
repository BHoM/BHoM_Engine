using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Common;

namespace BH.Engine.Results
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<List<T>> GroupByCase<T>(this IEnumerable<T> results) where T : IResult
        {
            return results.GroupBy(x => x.Case).Select(x => x.ToList()).ToList();
        }

        /***************************************************/

        public static List<List<T>> GroupByObjectId<T>(this IEnumerable<T> results) where T : IResult
        {
            return results.GroupBy(x => x.ObjectId).Select(x => x.ToList()).ToList();
        }

        /***************************************************/

        public static List<List<T>> GroupByTimeStep<T>(this IEnumerable<T> results) where T : IResult
        {
            return results.GroupBy(x => x.TimeStep).Select(x => x.ToList()).ToList();
        }

        /***************************************************/
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

using BH.oM.Reflection.Testing;
using BH.oM.Planning;

namespace BH.Engine.Reflection
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static TestResult TestResult(MethodBase method, List<bool> results = null, Issue issue = null)
        {
            return new TestResult
            {
                Method = method,
                Results = results ?? new List<bool>(),
                Issue = issue
            };
        }

        /***************************************************/
    }
}

using BH.Engine.Reflection;
using BH.Engine.Serialiser;
using BH.Engine.Test;
using BH.oM.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Test.Serialiser
{
    public static partial class Helpers
    {
        /*************************************/
        /**** Public Methods              ****/
        /*************************************/

        public static List<Type> TypesToTest()
        {
            Engine.Reflection.Compute.LoadAllAssemblies();

            // It feels like the BHoMTypeList method should already return a clean list of Type but it doesn't at the moment
            return Engine.Reflection.Query.BHoMTypeList().Where(x => {
                return typeof(IObject).IsAssignableFrom(x)
                  && !x.IsAbstract
                  && !x.IsDeprecated();
            }).ToList();
        }

        /*************************************/
    }
}

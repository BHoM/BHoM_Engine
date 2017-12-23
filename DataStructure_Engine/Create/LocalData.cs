using BH.oM.DataStructure;
using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.DataStructure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static LocalData<T> LocalData<T>(Point position, T data)
        {
            return new LocalData<T> { Position = position, Data = data };
        }

        /***************************************************/
    }
}

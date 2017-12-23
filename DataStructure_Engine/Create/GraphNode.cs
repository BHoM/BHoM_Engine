using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.DataStructure;
using BH.Engine.DataStructure;

namespace BH.Engine.DataStructure 
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static GraphNode<T> GraphNode<T>(T value = default(T))
        {
            return new GraphNode<T>
            {
                Value = value
            };
        }

        /***************************************************/
    }
}

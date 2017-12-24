using BH.oM.DataStructure;
using BH.oM.Geometry;

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

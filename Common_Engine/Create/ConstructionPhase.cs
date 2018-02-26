using BH.oM.Common.Planning;
using System;

namespace BH.Engine.Common
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static ConstructionPhase ConstructionPhase(string name, DateTime startTime, DateTime endTime)
        {
            return new ConstructionPhase
            {
                Name = name,
                StartTime = startTime,
                EndTime = endTime
            };
        }

        /***************************************************/
    }
}

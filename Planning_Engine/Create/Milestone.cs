using BH.oM.Planning;
using System;

namespace BH.Engine.Planning
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Milestone Milestone(string name, DateTimeOffset dueOn, ItemState state)
        {
            return new Milestone
            {
                Name = name,
                DueOn = dueOn,
                State = state
            };
        }

        /***************************************************/
    }
}
